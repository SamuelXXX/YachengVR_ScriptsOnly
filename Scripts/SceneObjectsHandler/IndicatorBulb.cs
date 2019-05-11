using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorBulb : MonoBehaviour
{

    public MeshRenderer targetRenderer;

    protected Color HDRColor;
    protected bool isOn = true;
    private void OnValidate()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<MeshRenderer>();
        }
    }

    private void Awake()
    {
        GlobalEventManager.RegisterHandler("Indicator-" + name + ".On", On);
        GlobalEventManager.RegisterHandler("Indicator-" + name + ".Off", Off);
        GlobalEventManager.RegisterHandler("Indicator-" + name + ".Switch", Switch);
    }

    private void OnDestroy()
    {
        GlobalEventManager.UnregisterHandler("Indicator-" + name + ".On", On);
        GlobalEventManager.UnregisterHandler("Indicator-" + name + ".Off", Off);
        GlobalEventManager.UnregisterHandler("Indicator-" + name + ".Switch", Switch);
    }
    // Use this for initialization
    void Start()
    {
        HDRColor = targetRenderer.materials[0].GetColor("_EmissionColor");
        Off();
    }

    // Update is called once per frame
    void Update()
    {

    }

    [ContextMenu("On")]
    public void On()
    {
        if (isOn)
            return;
        targetRenderer.materials[0].SetColor("_EmissionColor", HDRColor);
        isOn = true;
    }

    [ContextMenu("Off")]
    public void Off()
    {
        if (!isOn)
            return;
        targetRenderer.materials[0].SetColor("_EmissionColor", Color.black);
        isOn = false;
    }

    [ContextMenu("Switch")]
    public void Switch()
    {
        if (isOn)
        {
            Off();
        }
        else
        {
            On();
        }
    }
}
