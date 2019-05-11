using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Teleporter : MonoBehaviour
{
    /// <summary>
    /// The teleporter that is in predicting process now, works for multi teleporter scenerio
    /// </summary>
    public static Teleporter WorkingTeleporter { get; private set; }


    [HideInInspector, System.NonSerialized]
    public bool transportingEnabled = true;
    public PredictionLineDrawer LineDrawer { get; private set; }
    public Vector3? AnticipateHitPoint { get; private set; }

    #region LifeCircle
    // Use this for initialization
    public SteamVR_Controller.Device VRInput
    {
        get
        {
            SteamVR_TrackedObject sto = GetComponent<SteamVR_TrackedObject>();
            if (sto == null || sto.index == SteamVR_TrackedObject.EIndex.None)
                return null;

            return SteamVR_Controller.Input((int)sto.index);
        }
    }

    void Awake()
    {
        LineDrawer = GetComponentInChildren<PredictionLineDrawer>();
    }

    void SwitchHitArea(TeleportingArea ta)
    {
        if (ta != hitArea)
        {
            if (ta != null)
            {
                ta.OnEnterArea(this);
            }

            if (hitArea != null)
            {
                hitArea.OnExitArea();
            }

            hitArea = ta;
        }
    }

    void SwitchHitPoint(TeleportingPoint tp)
    {
        if (tp != hitTelepoint)
        {
            if (tp != null)
            {
                tp.OnEnterPoint(this);
            }

            if (hitTelepoint != null)
            {
                hitTelepoint.OnExitPoint();
            }

            hitTelepoint = tp;

            //Modify Target Point
            if (hitTelepoint != null)
            {
                LineDrawer.modifiedTargetPoint = hitTelepoint.transform.position;
            }
            else
            {
                LineDrawer.modifiedTargetPoint = null;
            }
        }
    }
    TeleportingArea hitArea = null;//Current hitting area
    TeleportingPoint hitTelepoint = null;
    // Update is called once per frame
    void Update()
    {
        if (WorkingTeleporter != null && WorkingTeleporter != this)
            return;

        if (!transportingEnabled || inTransporting)
        {
            if (LineDrawer)
            {
                LineDrawer.drawLine = false;
            }
            return;
        }

        if (LineDrawer)
        {
            if (GetTeleportingCommand())
            {
                WorkingTeleporter = this;
                if (!LineDrawer.drawLine)//Enable teleporting predicting
                {
                    GlobalEventManager.SendEvent("VRPlayer.BeginTeleportingPredict");
                    LineDrawer.drawLine = true;
                }

                //Teleporting Area Update
                TeleportingArea ta = null;
                if (LineDrawer.HitCollider != null)
                {
                    ta = LineDrawer.HitCollider.GetComponent<TeleportingArea>();
                }

                AnticipateHitPoint = LineDrawer.AnticipateHitPoint;//Acquire anticipated hit point of predicting line,not the final hit point
                SwitchHitArea(ta);

                //Telepoint Update
                if (AnticipateHitPoint != null)
                {
                    TeleportingPoint tp = TeleportingPoint.FindClosestTelepoints(AnticipateHitPoint.Value);//Check if a telepoint is near the anicipate hit point
                    SwitchHitPoint(tp);

                }
                else
                {
                    SwitchHitPoint(null);
                }
            }
            else
            {
                if (LineDrawer.drawLine)
                {
                    AnticipateHitPoint = null;
                    LineDrawer.drawLine = false;
                    if (hitArea != null)
                    {
                        hitArea.OnTeleport();
                    }

                    if (hitTelepoint != null)
                    {
                        hitTelepoint.OnTeleport();
                    }

                    SwitchHitArea(null);
                    SwitchHitPoint(null);

                    GlobalEventManager.SendEvent("VRPlayer.StopTeleportingPredict");
                    WorkingTeleporter = null;
                    if (LineDrawer.Status == HittingStatus.HitTeleportingArea || LineDrawer.Status == HittingStatus.HitModifiedTarget)
                    {
                        StartBodyTransporting();
                        GlobalEventManager.SendEvent("TransportBody");
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        GlobalEventManager.RegisterHandler("VRPlayer.EnableTransporting", EnableTransporting);
        GlobalEventManager.RegisterHandler("VRPlayer.DisableTransporting", DisableTransporting);
    }

    private void OnDisable()
    {
        GlobalEventManager.UnregisterHandler("VRPlayer.EnableTransporting", EnableTransporting);
        GlobalEventManager.UnregisterHandler("VRPlayer.DisableTransporting", DisableTransporting);
    }

    private void LateUpdate()
    {
    }
    #endregion

    bool GetTeleportingCommand()
    {
        if (VRInput != null)
        {
            return VRInput.GetPress(EVRButtonId.k_EButton_SteamVR_Touchpad);
        }
        else
        {
            return Input.GetMouseButton(1);
        }
    }

    bool inTransporting = false;
    void StartBodyTransporting()
    {
        StartCoroutine(TransportingRoutine(new Color(0f, 0f, 0f, 1f)));
    }

    IEnumerator TransportingRoutine(Color targetColor)
    {
        float timer = 0f;
        inTransporting = true;
        Color originalColor = VRPlayer.Instance.GetMaskColor();

        while (true)
        {
            timer += Time.deltaTime * 4f;
            VRPlayer.Instance.AdjustCameraMask(Color.Lerp(originalColor, targetColor, timer));
            if (timer >= 1f)
            {
                VRPlayer.Instance.AdjustCameraMask(targetColor);
                break;
            }
            yield return null;
        }
        VRPlayer.Instance.TransportBody(LineDrawer.HitPoint);
        timer = 0f;
        while (true)
        {
            timer += Time.deltaTime * 4f;
            VRPlayer.Instance.AdjustCameraMask(Color.Lerp(targetColor, originalColor, timer));
            if (timer >= 1f)
            {
                VRPlayer.Instance.AdjustCameraMask(originalColor);
                break;
            }
            yield return null;
        }
        inTransporting = false;

    }

    #region Global Event Handler
    void EnableTransporting()
    {
        transportingEnabled = true;
    }

    void DisableTransporting()
    {
        transportingEnabled = false;
    }
    #endregion
}
