using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VRInteractable))]
public class VRPickable : MonoBehaviour
{

    protected VRInteractable mInteractable;
    protected Vector3 startPosition;
    protected Quaternion startRotation;
    private void Awake()
    {
        mInteractable = GetComponent<VRInteractable>();
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
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

    private void LateUpdate()
    {
        if (mInteractor == null)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, Time.deltaTime * 8f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, startRotation, Time.deltaTime * 8f);
        }
        else
        {
            transform.localRotation = mInteractor.acquirePoint.transform.rotation * relativeRotation;
            transform.localPosition = mInteractor.acquirePoint.transform.TransformPoint(relativePosition);
        }
    }

    VRInteractor mInteractor = null;
    Vector3 relativePosition;
    Quaternion relativeRotation;
    void OnStartInteracting(VRInteractor interactor)
    {
        if (mInteractor != null)
            return;

        relativePosition = interactor.acquirePoint.InverseTransformPoint(transform.position);
        relativeRotation = Quaternion.Inverse(interactor.acquirePoint.rotation) * transform.rotation;
        mInteractor = interactor;
    }

    void OnStopInteracting(VRInteractor interactor)
    {
        if (mInteractor != interactor)
            return;
        mInteractor = null;
    }
}
