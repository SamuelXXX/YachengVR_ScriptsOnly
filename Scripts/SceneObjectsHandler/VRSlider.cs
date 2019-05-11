using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRSlider : MonoBehaviour
{
    [Range(-1f, 1f), SerializeField]
    protected float slideValue = 0f;
    public bool hideWhenStart = true;
    public bool freeLoose = false;
    public Transform leftNode;
    public Transform rightNode;
    public Transform sliderNode;
    public UnityEvent OnReachLeftPolar;
    public UnityEvent OnReachRightPolar;
    public UnityEvent OnLeaveLeftPolar;
    public UnityEvent OnLeaveRightPolar;

    [System.NonSerialized]
    public float stableSlideValue = 0f;
    protected bool reachLeft = false;
    protected bool reachRight = false;
    public float SlideValue
    {
        get
        {
            return slideValue;
        }
        set
        {
            slideValue = Mathf.Clamp(value, -1f, 1f);
            UpdateNode();

            if (slideValue < -0.99f)
            {
                if (!reachLeft)
                {
                    reachLeft = true;
                    leftNode.localScale *= 1.2f;
                    OnReachLeftNode();
                }
            }
            else
            {
                if (reachLeft)
                {
                    reachLeft = false;
                    leftNode.localScale /= 1.2f;
                    OnLeaveLeftNode();
                }
            }

            if (slideValue > 0.99f)
            {
                if (!reachRight)
                {
                    reachRight = true;
                    rightNode.localScale *= 1.2f;
                    OnReachRightNode();
                }
            }
            else
            {
                if (reachRight)
                {
                    reachRight = false;
                    rightNode.localScale /= 1.2f;
                    OnLeaveRightNode();
                }
            }
        }
    }

    #region Life Circle
    // Use this for initialization
    void Start()
    {
        if (hideWhenStart)
        {
            Hide();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        UpdateHandling();
    }

    private void OnValidate()
    {
        UpdateNode();
    }


    #endregion

    #region Internal Events
    void UpdateNode()
    {
        sliderNode.position = Vector3.Lerp(leftNode.position, rightNode.position, (slideValue + 1f) / 2f);
    }

    void OnReachLeftNode()
    {
        if (OnReachLeftPolar != null)
        {
            OnReachLeftPolar.Invoke();
        }
    }

    void OnLeaveLeftNode()
    {
        if (OnLeaveLeftPolar != null)
        {
            OnLeaveLeftPolar.Invoke();
        }
    }

    void OnReachRightNode()
    {
        if (OnReachRightPolar != null)
        {
            OnReachRightPolar.Invoke();
        }
    }

    void OnLeaveRightNode()
    {
        if (OnLeaveRightPolar != null)
        {
            OnLeaveRightPolar.Invoke();
        }
    }
    #endregion

    #region External Control
    Transform handler = null;
    Vector3 startHandlingPosition;
    float startValue = 0f;
    public void StartHandle(Transform handler)
    {
        if (this.handler != null)//Already has a handler
        {
            return;
        }

        this.handler = handler;
        startHandlingPosition = handler.position;
        startValue = SlideValue;
    }

    public void SetStablePoint(float f)
    {
        stableSlideValue = f;
    }

    public void Show()
    {
        foreach (var m in GetComponentsInChildren<MeshRenderer>())
        {
            m.enabled = true;
        }
    }

    public void Hide()
    {
        foreach (var m in GetComponentsInChildren<MeshRenderer>())
        {
            m.enabled = false;
        }
    }

    Vector3 lastTriggerPosition;
    void UpdateHandling()
    {
        if (handler != null)
        {
            VRInteractor interactor = handler.GetComponentInParent<VRInteractor>();
            if (interactor.VRInput != null)
            {
                if (Vector3.Distance(lastTriggerPosition, sliderNode.transform.localPosition) > 0.005f)
                {
                    lastTriggerPosition = sliderNode.transform.localPosition;
                    interactor.VRInput.TriggerHapticPulse((ushort)(1000f * Mathf.Abs(SlideValue)));
                }
            }
            Vector3 dir = (rightNode.position - leftNode.position) / 2;

            Vector3 handleDiff = handler.position - startHandlingPosition;
            float d = Vector3.Dot(dir.normalized, handleDiff);
            d /= dir.magnitude;

            SlideValue = d + startValue;
        }
        else
        {
            if (!freeLoose)
                SlideValue = Mathf.Lerp(SlideValue, stableSlideValue, Time.deltaTime * 8f);
        }
    }

    public void StopHandle()
    {
        handler = null;
    }
    #endregion
}
