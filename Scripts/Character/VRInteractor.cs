using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Valve.VR;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(VRInteractor))]
public class VRInteractorEditor : Editor
{
    public VRInteractor Target
    {
        get
        {
            return target as VRInteractor;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        switch (Target.acquireTargetType)
        {
            case VRInteractor.AcquireHoverTargetType.RayCast:
                Target.rayDistance = EditorGUILayout.FloatField("RayLength", Target.rayDistance);
                break;
            case VRInteractor.AcquireHoverTargetType.Contact:
                Target.contackSkinWidth = EditorGUILayout.FloatField("ContactRadius", Target.contackSkinWidth);
                break;
            default: break;
        }

    }

}

#endif

/// <summary>
/// Used to define a "Hand" that can interact,how to acquire a VRInteractable and give interaction
/// </summary>
public class VRInteractor : MonoBehaviour
{

    public enum AcquireHoverTargetType
    {
        RayCast = 0,
        Contact
    }

    #region Basic Settings
    public AcquireHoverTargetType acquireTargetType = AcquireHoverTargetType.RayCast;
    public Transform acquirePoint;
    public LayerMask ignoreLayer;

    //Parameters for RayCast Type
    [HideInInspector]
    public float rayDistance = 2f;


    //Parameters for Contact Type
    [HideInInspector]
    public float contackSkinWidth;
    #endregion

    #region Interacting
    protected VRInteractable currentHoveringObject = null;
    protected VRInteractable currentInteractingObject = null;

    [HideInInspector, NonSerialized]
    public bool interactingEnabled = true;


    /// <summary>
    /// Switch to another hovering object which can be null
    /// </summary>
    /// <param name="target"></param>
    void SwitchHoveringObject(VRInteractable target)
    {
        if (target != currentHoveringObject)
        {
            if (currentHoveringObject != null)
            {
                currentHoveringObject.OnHoveredOut(this);
            }
            currentHoveringObject = target;
            if (currentHoveringObject != null)
            {
                currentHoveringObject.OnHoveredIn(this);
            }
        }
    }

    void SwitchInteractingObject(VRInteractable target)
    {
        if (target != currentInteractingObject)
        {
            if (currentInteractingObject != null)
            {
                OnStopInteractWith(currentInteractingObject);
                currentInteractingObject.OnInteractionFinished(this);            
            }
            currentInteractingObject = target;
            if (currentInteractingObject != null)
            {
                OnStartInteractWith(currentInteractingObject);
                currentInteractingObject.OnInteractionStart(this);              
            }
        }
    }

    void OnStartInteractWith(VRInteractable interactable)
    {
        if (acquireTargetType == AcquireHoverTargetType.RayCast && VRPlayer.Instance.IsUsingSimulator)
        {
            acquirePoint.localPosition = acquirePoint.parent.InverseTransformPoint(interactable.transform.position);
        }
    }

    void OnStopInteractWith(VRInteractable interactable)
    {
        if (acquireTargetType == AcquireHoverTargetType.RayCast && VRPlayer.Instance.IsUsingSimulator)
        {
            acquirePoint.localPosition = Vector3.zero;
        }
    }

    void UpdateInteractable()
    {
        if (interactingEnabled == false)
        {
            SwitchHoveringObject(null);
            SwitchInteractingObject(null);
            return;
        }

        VRInteractable hoverTarget = GetHoveringObject();
        bool isTriggering = GetTriggering();
        if (!isTriggering)
        {
            SwitchInteractingObject(null);
            SwitchHoveringObject(hoverTarget);
        }
        else
        {
            SwitchInteractingObject(currentHoveringObject);
        }



    }

    VRInteractable GetHoveringObject()
    {
        if (acquirePoint == null)
            acquirePoint = transform;
        switch (acquireTargetType)
        {
            case AcquireHoverTargetType.RayCast:
                Ray ray = new Ray(acquirePoint.position, acquirePoint.forward);
                RaycastHit hit = new RaycastHit();
                VRInteractable hitInteractable = null;
                if (Physics.Raycast(ray, out hit, rayDistance, ~ignoreLayer))
                {
                    Collider col = hit.collider;
                    hitInteractable = col.GetComponent<VRInteractable>();
                }

                return hitInteractable;
            case AcquireHoverTargetType.Contact:
                Collider[] cols = Physics.OverlapSphere(acquirePoint.position, contackSkinWidth);
                List<VRInteractable> overlappedInteractables = new List<VRInteractable>();
                //use Linq to access all VRInteractables
                overlappedInteractables.AddRange(from c in cols where c.GetComponent<VRInteractable>() != null select c.GetComponent<VRInteractable>());
                float distance = 1000;
                return overlappedInteractables.FindLast(i =>//Find the one closest to acquire point
                {
                    var dist = Vector3.Distance(i.transform.position, acquirePoint.position);

                    if (dist < distance)
                    {
                        distance = dist;
                        return true;
                    }
                    return false;
                });
            default: return null;
        }
    }

    #region Input Remapping
    public SteamVR_Controller.Device VRInput
    {
        get
        {
            SteamVR_TrackedObject sto = GetComponent<SteamVR_TrackedObject>();
            if (sto == null || sto.index == SteamVR_TrackedObject.EIndex.None)
                return null;

            return SteamVR_Controller.Input((int)sto.index);
        }
    }

    public bool GetTriggerDown()
    {
        if (VRInput != null)
        {
            return VRInput.GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger);
        }
        else
        {
            return Input.GetMouseButtonDown(0);
        }
    }

    public bool GetTriggerUp()
    {
        if (VRInput != null)
        {
            return VRInput.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger);
        }
        else
        {
            return Input.GetMouseButtonUp(0);
        }
    }

    public bool GetTriggering()
    {
        if (VRInput != null)
        {
            return VRInput.GetPress(EVRButtonId.k_EButton_SteamVR_Trigger);
        }
        else
        {
            return Input.GetMouseButton(0);
        }
    }
    #endregion
    #endregion

    #region Life Circle
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInteractable();
    }

    private void OnValidate()
    {
        if (acquirePoint == null)
            acquirePoint = transform;
    }

    private void OnDrawGizmos()
    {
        if (acquireTargetType == AcquireHoverTargetType.RayCast)
        {
            Vector3 startPoint = acquirePoint.position;
            Vector3 stopPoint = acquirePoint.position + acquirePoint.forward * rayDistance;
            Gizmos.DrawLine(startPoint, stopPoint);
        }

        if (acquireTargetType == AcquireHoverTargetType.Contact)
        {
            Gizmos.DrawSphere(acquirePoint.position, contackSkinWidth);
        }
    }
    #endregion
}
