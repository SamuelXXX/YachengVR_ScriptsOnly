using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(VRInteractable))]
public class PushButton : MonoBehaviour
{
    public PlayMakerFSM triggerEffectFsm;
    public UnityEvent onClick;

    VRInteractable mInteractable;

    private void Awake()
    {
        mInteractable = GetComponent<VRInteractable>();
        if (mInteractable != null)
        {
            mInteractable.OnInteractionStartEvent.AddListener(OnButtonClick);
        }
    }

    private void OnValidate()
    {
        if (triggerEffectFsm == null)
        {
            triggerEffectFsm = GetComponent<PlayMakerFSM>();
        }
    }

    void OnButtonClick(VRInteractor interactor)
    {
        if (triggerEffectFsm != null)
        {
            triggerEffectFsm.SendEvent("Trigger");
        }

        if (onClick != null)
        {
            onClick.Invoke();
        }
    }
}
