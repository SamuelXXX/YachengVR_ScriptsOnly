using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRInteractable : MonoBehaviour
{
    #region Interacting Event
    [System.Serializable]
    public class InteractionEvent : UnityEvent<VRInteractor> { }
    public InteractionEvent OnHoveredInEvent;
    public InteractionEvent OnHoveredOutEvent;
    public InteractionEvent OnInteractionStartEvent;
    public InteractionEvent OnInteractionFinishedEvent;

    public void OnHoveredIn(VRInteractor interactor)
    {
        if (OnHoveredInEvent != null)
        {
            OnHoveredInEvent.Invoke(interactor);
        }
    }

    public void OnHoveredOut(VRInteractor interactor)
    {
        if (OnHoveredOutEvent != null)
        {
            OnHoveredOutEvent.Invoke(interactor);
        }
    }

    public void OnInteractionStart(VRInteractor interactor)
    {
        if (OnInteractionStartEvent != null)
        {
            OnInteractionStartEvent.Invoke(interactor);
        }
    }

    public void OnInteractionFinished(VRInteractor interactor)
    {
        if (OnInteractionFinishedEvent != null)
        {
            OnInteractionFinishedEvent.Invoke(interactor);
        }
    }
    #endregion
}
