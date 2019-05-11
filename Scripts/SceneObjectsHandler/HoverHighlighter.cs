using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[RequireComponent(typeof(VRInteractable))]
public class HoverHighlighter : MonoBehaviour
{
    VRInteractable mInteractable;
    public Material highlightMaterial;
    public Transform highlightTarget;

    Dictionary<MeshRenderer, Material[]> materialCache = new Dictionary<MeshRenderer, Material[]>();
    MeshRenderer[] renderers = null;
    void Awake()
    {
        if (highlightTarget == null)
            highlightTarget = transform;
        mInteractable = GetComponent<VRInteractable>();
        mInteractable.OnHoveredInEvent.AddListener(OnHoverIn);
        mInteractable.OnHoveredOutEvent.AddListener(OnHoverOut);
        mInteractable.OnInteractionStartEvent.AddListener(InteractingStart);
        mInteractable.OnInteractionFinishedEvent.AddListener(InteractingStop);

        renderers = highlightTarget.GetComponentsInChildren<MeshRenderer>(true);
        materialCache = renderers.ToDictionary<MeshRenderer, MeshRenderer, Material[]>(r => r, r => r.materials);
    }

    void Highlight()
    {
        foreach (var r in renderers)
        {
            List<Material> replaceMaterials = new List<Material>();
            var i = r.materials.Length;
            while (i-- != 0)
            {
                replaceMaterials.Add(highlightMaterial);
            }
            r.materials = replaceMaterials.ToArray();
        }
    }

    void Dehighlight()
    {
        foreach (var r in materialCache)
        {
            r.Key.materials = r.Value;
        }
    }

    private void OnValidate()
    {
        if (highlightMaterial == null)
        {
            highlightMaterial = Resources.Load<Material>("Highlight");
        }
    }


    VRInteractor hoverInteractor = null;
    private void InteractingStart(VRInteractor interactor)
    {
        if (interactor != hoverInteractor)
            return;

        //Dehighlight
        Dehighlight();
    }

    private void InteractingStop(VRInteractor interactor)
    {
        if (interactor != hoverInteractor)
            return;

        //Highlight
        Highlight();
    }

    private void OnHoverIn(VRInteractor interactor)
    {
        if (hoverInteractor != null)
            return;

        hoverInteractor = interactor;
        //Highlight
        Highlight();

    }

    private void OnHoverOut(VRInteractor interactor)
    {
        if (interactor != hoverInteractor)
        {
            return;
        }

        hoverInteractor = null;
        //Dehighlight
        Dehighlight();
    }
}
