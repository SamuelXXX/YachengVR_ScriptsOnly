using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(VRInteractable))]
public class LooseSwitch : MonoBehaviour
{
    [SerializeField]
    protected VRSlider mSlider;
    public UnityEvent reachLeftHandler;
    public UnityEvent reachRightHandler;
    public bool allowBuild = false;

    [SerializeField, HideInInspector]
    protected Quaternion centerRotation;
    [SerializeField, HideInInspector]
    protected Quaternion leftRotation;
    [SerializeField, HideInInspector]
    protected Quaternion rightRotation;



    #region Internal Event
    Transform handler = null;
    void OnStartInteracting(VRInteractor interactor)
    {
        if (handler != null)
        {
            return;
        }
        if (mSlider != null)
        {
            mSlider.Show();
            mSlider.StartHandle(interactor.acquirePoint);
            handler = interactor.acquirePoint;
        }
    }

    void OnStopInteracting(VRInteractor interactor)
    {
        if (handler == null || handler != interactor.acquirePoint)
        {
            return;
        }
        if (mSlider != null)
        {
            mSlider.Hide();
            mSlider.StopHandle();
            handler = null;
        }
    }

    void OnReachLeft()
    {
        if (reachLeftHandler != null)
        {
            reachLeftHandler.Invoke();
        }
    }

    void OnReachRight()
    {
        if (reachRightHandler != null)
        {
            reachRightHandler.Invoke();
        }
    }

    void UpdateRotation()
    {
        if (mSlider != null)
        {
            float s = mSlider.SlideValue;
            if (s > 0)
            {
                transform.localRotation = Quaternion.Lerp(centerRotation, rightRotation, s);
            }
            else
            {
                transform.localRotation = Quaternion.Lerp(centerRotation, leftRotation, -s);
            }
        }
    }
    #endregion

    #region Build Methods
    [ContextMenu("Build Center")]
    void BuildCenter()
    {
        if (!allowBuild)
            return;
        centerRotation = transform.localRotation;
        allowBuild = false;
    }

    [ContextMenu("Build Left")]
    void BuildLeft()
    {
        if (!allowBuild)
            return;
        leftRotation = transform.localRotation;
        allowBuild = false;
    }

    [ContextMenu("Build Right")]
    void BuildRight()
    {
        if (!allowBuild)
            return;
        rightRotation = transform.localRotation;
        allowBuild = false;
    }
    #endregion


    #region Life Circle

    protected VRInteractable mInteractable;
    private void Awake()
    {
        if (mSlider != null)
        {
            mSlider.OnReachLeftPolar.AddListener(OnReachLeft);
            mSlider.OnReachRightPolar.AddListener(OnReachRight);
        }
        mInteractable = GetComponent<VRInteractable>();
        mInteractable.OnInteractionStartEvent.AddListener(OnStartInteracting);
        mInteractable.OnInteractionFinishedEvent.AddListener(OnStopInteracting);
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateRotation();
    }
    #endregion
}
