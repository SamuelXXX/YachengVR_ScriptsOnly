using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeHoverDescription : MonoBehaviour
{

    public Transform Head
    {
        get
        {
            return VRPlayer.Instance.Head;
        }
    }

    public PlayMakerFSM effectFsm;

    public const float range = 1f;
    public const float angle = 10f;

    #region Life Circle
    private void Awake()
    {

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        StartCoroutine("UpdateRoutine");
    }

    private void OnDisable()
    {
        StopCoroutine("UpdateRoutine");
    }

    IEnumerator UpdateRoutine()
    {
        while (true)
        {
            if (IsInSight())
            {
                Show();
            }
            else
            {
                Hide();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnValidate()
    {
        effectFsm = GetComponentInChildren<PlayMakerFSM>();
    }
    #endregion

    #region Internal Call Back
    void Show()
    {
        if (effectFsm)
        {
            effectFsm.SendEvent("Show");
        }
    }

    void Hide()
    {
        if (effectFsm)
        {
            effectFsm.SendEvent("Hide");
        }
    }

    bool IsInSight()
    {
        bool inRange = false;
        bool inView = false;

        float d = Vector3.Distance(Head.position, transform.position);
        inRange = d < range;

        float a = Vector3.Angle(Head.forward, transform.position - Head.position);
        inView = a < angle;

        return inRange && inView;
    }
    #endregion

}
