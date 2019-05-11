using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(VRInteractable))]
public class VRButton : MonoBehaviour
{
    public Color normalColor;
    public Color hoveredColor;
    public Color clickedColor;

    public float transitionTime = 0.2f;

    [System.Serializable]
    public class HandClickEvent : UnityEvent { }
    public HandClickEvent onHandClick;

    Image mImage;
    VRInteractable mInteractable;
    Color targetColor;
    bool isHovering = false;
    bool isClicking = false;

    void Awake()
    {
        mImage = GetComponent<Image>();
        mInteractable = GetComponent<VRInteractable>();
        mImage.color = normalColor;
        mInteractable.OnHoveredInEvent.AddListener(OnHoverIn);
        mInteractable.OnHoveredOutEvent.AddListener(OnHoverOut);
        mInteractable.OnInteractionStartEvent.AddListener(InteractingStart);
        mInteractable.OnInteractionFinishedEvent.AddListener(InteractionStop);
    }

    private void OnButtonClick()
    {
        onHandClick.Invoke();
    }

    VRInteractor interactor = null;
    private void InteractingStart(VRInteractor interactor)
    {
        this.interactor = interactor;
        isClicking = true;
    }

    private void InteractionStop(VRInteractor interactor)
    {
        isClicking = false;
        OnButtonClick();
        this.interactor = null;
    }

    private void OnHoverIn(VRInteractor interactor)
    {
        if (interactor.VRInput != null)
        {
            interactor.VRInput.TriggerHapticPulse(1000);
        }
        isHovering = true;
    }

    private void OnHoverOut(VRInteractor interactor)
    {
        isHovering = false;

    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isHovering)
        {
            if (isClicking)
            {
                targetColor = clickedColor;
            }
            else
            {
                targetColor = hoveredColor;
            }
        }
        else
        {
            targetColor = normalColor;
        }

        UpdateColor();
    }

    void UpdateColor()
    {
        var color = mImage.color;
        color = Color.Lerp(color, targetColor, Time.deltaTime * (1 / transitionTime));
        mImage.color = color;
    }
}
