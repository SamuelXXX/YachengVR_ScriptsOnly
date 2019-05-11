using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VRInteractable))]
public class DoorHandler : MonoBehaviour
{
    public enum HandlePositionStatus
    {
        InsideLegalRange = 0,
        BelowClosePlane,
        UpOpenPlane,
        Illegal
    }

    public Transform door;
    public Vector3 doorOpenDirection = Vector3.down;
    public bool allowBuild = false;

    protected Transform target;
    protected VRInteractable mInteractable;


    #region Door Operation
    void HandleDoor()
    {
        Vector3 targetNormal = Vector3.Cross(TargetPosition - door.position, doorOpenDirection);
        targetNormal = targetNormal.normalized;
        Vector3 currentNormal = Vector3.Cross(transform.position - door.position, doorOpenDirection);
        currentNormal = currentNormal.normalized;


        Quaternion rotate = Quaternion.FromToRotation(currentNormal, targetNormal);
        door.rotation = rotate * door.rotation;
    }

    void UpdateRotation()
    {
        if (target != null && door != null)
        {
            HandleDoor();
        }
    }
    #endregion

    #region Runtime Target Position
    protected Vector3 fullOpenDirection;
    protected Vector3 fullCloseDirection;
    Vector3 targetPosition;
    public Vector3 TargetPosition
    {
        get
        {
            switch (HandleState(target.position))
            {
                case HandlePositionStatus.InsideLegalRange:
                    targetPosition = target.position;
                    break;
                case HandlePositionStatus.BelowClosePlane:
                    targetPosition = fullClosePosition;
                    break;
                case HandlePositionStatus.UpOpenPlane:
                    targetPosition = fullOpenPosition;
                    break;
                case HandlePositionStatus.Illegal:
                    break;
                default:
                    break;
            }

            return targetPosition;

        }
    }

    HandlePositionStatus HandleState(Vector3 position)
    {
        bool upClosePlaneArea = Vector3.Dot(position - fullClosePosition, fullCloseDirection) > 0;
        bool belowOpenPlaneArea = Vector3.Dot(position - fullOpenPosition, fullOpenDirection) < 0;
        if (upClosePlaneArea && belowOpenPlaneArea)
        {
            return HandlePositionStatus.InsideLegalRange;
        }
        else if (upClosePlaneArea && !belowOpenPlaneArea)
        {
            return HandlePositionStatus.UpOpenPlane;
        }
        else if (!upClosePlaneArea && belowOpenPlaneArea)
        {
            return HandlePositionStatus.BelowClosePlane;
        }
        else
        {
            return HandlePositionStatus.Illegal;
        }
    }
    #endregion

    #region Build Methods
    [SerializeField, HideInInspector]
    protected Vector3 fullOpenPosition;

    [SerializeField, HideInInspector]
    protected Vector3 fullClosePosition;

    [SerializeField, HideInInspector]
    protected Quaternion fullCloseRotation;

    [ContextMenu("Build Full Open")]
    void BuildFullOpen()
    {
        if (!allowBuild)
            return;
        fullOpenPosition = transform.position;
        allowBuild = false;
    }

    [ContextMenu("Build Full Close")]
    void BuildFullClose()
    {
        if (!allowBuild)
            return;
        fullClosePosition = transform.position;
        fullCloseRotation = door.rotation;
        allowBuild = false;
    }

    [ContextMenu("Apply Full Close")]
    void ApplyFullClose()
    {
        door.rotation = fullCloseRotation;
    }
    #endregion

    #region Life Circle
    private void Awake()
    {
        mInteractable = GetComponent<VRInteractable>();
        if (mInteractable != null)
        {
            mInteractable.OnInteractionStartEvent.AddListener(OnStartInteracting);
            mInteractable.OnInteractionFinishedEvent.AddListener(OnStopInteracting);
        }

        fullOpenDirection = Vector3.Cross(fullOpenPosition - door.position, doorOpenDirection);
        fullCloseDirection = Vector3.Cross(fullClosePosition - door.position, doorOpenDirection);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateRotation();
        UpdateVRHandler();
    }

    private void OnDrawGizmos()
    {
        if (target != null)
            Gizmos.DrawSphere(TargetPosition, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(fullClosePosition, 0.01f);
        Gizmos.DrawLine(fullClosePosition, door.position);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(fullOpenPosition, 0.01f);
        Gizmos.DrawLine(fullOpenPosition, door.position);

        //Gizmos.DrawLine(fullOpenPosition, fullOpenPosition + fullOpenDirection);
        //Gizmos.DrawLine(fullClosePosition, fullClosePosition + fullCloseDirection);
        //Gizmos.DrawLine(door.position, doorOpenDirection + door.position);
    }
    #endregion

    #region Interaction
    VRInteractor currentInteractor;
    void OnStartInteracting(VRInteractor interactor)
    {
        if (currentInteractor == null)
        {
            currentInteractor = interactor;
            target = interactor.acquirePoint;
        }
    }

    void OnStopInteracting(VRInteractor interactor)
    {
        if (currentInteractor == interactor)
        {
            currentInteractor = null;
            target = null;
        }
    }

    Vector3 lastTriggerPosition;
    void UpdateVRHandler()
    {
        if (currentInteractor != null && currentInteractor.VRInput != null)
        {
            if (Vector3.Distance(lastTriggerPosition, transform.position) > 0.02f)
            {
                lastTriggerPosition = transform.position;
                currentInteractor.VRInput.TriggerHapticPulse(500);
            }
        }
        else
        {
            lastTriggerPosition = transform.position;
        }
    }
    #endregion
}
