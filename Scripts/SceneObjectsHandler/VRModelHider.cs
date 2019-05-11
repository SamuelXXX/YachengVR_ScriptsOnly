using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VRInteractable))]
public class VRModelHider : MonoBehaviour
{

    protected VRInteractable mInteractable;

    private void Awake()
    {
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

    }

    void OnStartInteracting(VRInteractor interactor)
    {
        if (!VRPlayer.Instance.IsUsingSimulator)
            foreach (var m in interactor.GetComponentsInChildren<MeshRenderer>())
            {
                m.enabled = false;
            }
    }

    void OnStopInteracting(VRInteractor interactor)
    {
        foreach (var m in interactor.GetComponentsInChildren<MeshRenderer>())
        {
            m.enabled = true;
        }
    }
}
