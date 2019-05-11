using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


public class VRPlayer : Singleton<VRPlayer>
{
    //Play in PC Simulator or VR or automatically check
    public enum PlayMode
    {
        Auto = 0,//when vr is ready, use vr, or use pc simulator
        PCSimulator,
        VR
    }

    //Interact with hand or head sight
    public enum InteractType
    {
        Hand = 0,
        HeadSight
    }

    #region Basic Settings
    [SerializeField]
    protected PlayMode playMode = PlayMode.Auto;
    public PlayMode Mode
    {
        get
        {
            return playMode;
        }
    }


    [Header("PC Simulator Settings")]
    [SerializeField]
    protected Transform PCSimulatorRig;
    [SerializeField]
    protected Transform PCSimulatorHead;
    [SerializeField]
    protected Transform PCSimulatorLeftHand;
    [SerializeField]
    protected Transform PCSimulatorRightHand;

    [Header("VR Settings")]
    [SerializeField]
    protected Transform VRBodyRig;
    [SerializeField]
    protected Transform VRHead;
    [SerializeField]
    protected Transform VRLeftHand;
    [SerializeField]
    protected Transform VRRightHand;


    [Header("Camera Mask Settings")]
    public GameObject CameraMask;
    #endregion

    #region Run-time data
    Vector3 originalPosition;
    [SerializeField]
    bool isUsingSimulator = false;
    public bool IsUsingSimulator
    {
        get
        {
            return isUsingSimulator;
        }
    }

    public Transform Body
    {
        get
        {
            if (isUsingSimulator)
                return PCSimulatorRig;
            else
                return VRBodyRig;
        }
    }

    public Transform Head
    {
        get
        {
            if (isUsingSimulator)
                return PCSimulatorHead;
            else
                return VRHead;
        }
    }

    public Transform LeftHand
    {
        get
        {
            if (isUsingSimulator)
                return PCSimulatorLeftHand;
            else
                return VRLeftHand;
        }
    }

    public Transform RightHand
    {
        get
        {
            if (isUsingSimulator)
                return PCSimulatorRightHand;
            else
                return VRRightHand;
        }
    }



    public Camera UICamera
    {
        get
        {
            if (isUsingSimulator)
            {
                return PCSimulatorHead.GetComponent<Camera>();
            }
            else
            {
                return RightHand.GetComponent<Camera>();
            }
        }
    }

    public Camera HeadCamera
    {
        get
        {
            return Head.GetComponent<Camera>();
        }
    }

    public InteractType InteractingType
    {
        get
        {
            if (isUsingSimulator)
            {
                return InteractType.HeadSight;
            }
            else
            {
                return InteractType.Hand;
            }
        }
    }



    #endregion

    #region Ext-Control
    /// <summary>
    /// Transport body but keep base root in a plane
    /// </summary>
    /// <param name="targetPosition"></param>
    public void TransportBody(Vector3 targetPosition)
    {
        Vector3 curPos = Head.position;
        curPos.y = 0;
        targetPosition.y = 0;
        Vector3 diff = targetPosition - curPos;
        Body.position += diff;
        GlobalEventManager.SendEvent("internal_VRPlayer.Repositioned");
    }

    /// <summary>
    /// Transport body but keep base root in a plane
    /// </summary>
    /// <param name="targetPosition"></param>
    public void TransportBody(Transform targetPoint)
    {
        TransportBody(targetPoint.position);
        Body.rotation = targetPoint.rotation;
    }

    /// <summary>
    /// Enable movement and transportation both in vr and pc simulator
    /// </summary>
    public void EnableMovement()
    {
        if (isUsingSimulator)
        {
            Body.GetComponent<PCSimulatorBody>().UnfreezeMovement();
        }

        foreach (var t in GetComponentsInChildren<Teleporter>(true))
        {
            t.transportingEnabled = true;
        }
    }

    /// <summary>
    /// Disable movement and transportation both in vr and pc simulator
    /// </summary>
    public void DisableMovement()
    {
        if (isUsingSimulator)
        {
            Body.GetComponent<PCSimulatorBody>().FreezeMovement();
        }

        foreach (var t in GetComponentsInChildren<Teleporter>(true))
        {
            t.transportingEnabled = false;
        }
    }

    public void EnableInteraction()
    {
        foreach (var t in GetComponentsInChildren<VRInteractor>(true))
        {
            t.interactingEnabled = true;
        }
    }

    public void DisableInteraction()
    {
        foreach (var t in GetComponentsInChildren<VRInteractor>(true))
        {
            t.interactingEnabled = false;
        }
    }


    #endregion

    #region LifeCircle
    protected override void Awake()
    {
        base.Awake();
        //originalPosition = transform.position;
        originalPosition = Vector3.zero;

        GlobalEventManager.RegisterHandler("VRPlayer.ResetPosition", ResetPosition);

        GlobalEventManager.RegisterHandler("VRPlayer.EnableCamera", EnableCamera);
        GlobalEventManager.RegisterHandler("VRPlayer.DisableCamera", DisableCamera);

        GlobalEventManager.RegisterHandler("VRPlayer.EnableMovement", EnableMovement);
        GlobalEventManager.RegisterHandler("VRPlayer.DisableMovement", DisableMovement);

        GlobalEventManager.RegisterHandler("VRPlayer.EnableInteraction", EnableInteraction);
        GlobalEventManager.RegisterHandler("VRPlayer.DisableInteraction", DisableInteraction);
    }
    // Use this for initialization
    void Start()
    {
        StartCoroutine(CheckVRDeviceRoutine());
    }

    IEnumerator CheckVRDeviceRoutine()
    {
        yield return new WaitForSeconds(0.8f);
        if (playMode == PlayMode.Auto)
        {
            if (!IsVRReadyAndConnected())
            {
                isUsingSimulator = true;
                PCSimulatorRig.gameObject.SetActive(true);
                Destroy(VRBodyRig.gameObject);
                //VRBodyRig.gameObject.SetActive(false);
            }
            else
            {
                isUsingSimulator = false;
                PCSimulatorRig.gameObject.SetActive(false);
                VRBodyRig.gameObject.SetActive(true);
            }
        }
        else if (playMode == PlayMode.PCSimulator)
        {
            isUsingSimulator = true;
            PCSimulatorRig.gameObject.SetActive(true);
            Destroy(VRBodyRig.gameObject);
            //VRBodyRig.gameObject.SetActive(false);
        }
        else
        {
            isUsingSimulator = false;
            PCSimulatorRig.gameObject.SetActive(false);
            VRBodyRig.gameObject.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void LateUpdate()
    {
        if (CameraMask)
        {
            CameraMask.transform.position = HeadCamera.transform.position + HeadCamera.transform.forward * (HeadCamera.nearClipPlane + 0.01f);
            CameraMask.transform.rotation = HeadCamera.transform.rotation;
        }
    }

    protected override void OnDestroy()
    {
        GlobalEventManager.UnregisterHandler("VRPlayer.ResetPosition", ResetPosition);

        GlobalEventManager.UnregisterHandler("VRPlayer.EnableCamera", EnableCamera);
        GlobalEventManager.UnregisterHandler("VRPlayer.DisableCamera", DisableCamera);

        GlobalEventManager.UnregisterHandler("VRPlayer.EnableMovement", EnableMovement);
        GlobalEventManager.UnregisterHandler("VRPlayer.DisableMovement", DisableMovement);

        GlobalEventManager.UnregisterHandler("VRPlayer.EnableInteraction", EnableInteraction);
        GlobalEventManager.UnregisterHandler("VRPlayer.DisableInteraction", DisableInteraction);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 80f, 40f), "退出应用"))
        {
            Application.Quit();
        }
    }
    #endregion

    #region CameraMaskControl
    MeshRenderer maskRenderer;
    Material maskMaterial;
    public void AdjustCameraMask(Color color)
    {
        if (CameraMask == null)
            return;
        if (maskRenderer == null)
        {
            maskRenderer = CameraMask.GetComponent<MeshRenderer>();
        }

        if (maskMaterial == null)
        {
            maskMaterial = maskRenderer.materials[0];
        }

        if (maskMaterial)
        {
            maskMaterial.SetColor("_Color", color);
        }
    }

    public Color GetMaskColor()
    {
        if (CameraMask == null)
            return Color.black;
        if (maskRenderer == null)
        {
            maskRenderer = CameraMask.GetComponent<MeshRenderer>();
        }

        if (maskMaterial == null)
        {
            maskMaterial = maskRenderer.materials[0];
        }

        if (maskMaterial)
        {
            return maskMaterial.GetColor("_Color");
        }

        return Color.black;
    }
    #endregion

    #region Internal Control
    /// <summary>
    /// A trick only used in initialization to check if VR device is ready.
    /// </summary>
    /// <returns></returns>
    bool IsVRReadyAndConnected()
    {
        if (VRHead.localPosition.magnitude > 0.005f)//if not set in zero point,the camera is holded by VR device
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void ResetPosition()
    {
        transform.position = originalPosition;
    }

    void EnableCamera()
    {
        Instance.HeadCamera.enabled = true;
    }

    void DisableCamera()
    {
        Instance.HeadCamera.enabled = false;
    }
    #endregion
}
