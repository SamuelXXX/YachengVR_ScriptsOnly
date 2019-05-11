using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TeleportingPoint : MonoBehaviour
{
    public static List<TeleportingPoint> teleportingPoints = new List<TeleportingPoint>();

    public void OnTeleport()
    {
        if (GetComponent<PlayMakerFSM>() != null)
        {
            GetComponent<PlayMakerFSM>().SendEvent("Trigger");
        }
    }

    public static TeleportingPoint FindClosestTelepoints(Vector3 p)
    {
        float dist = 100f;
        TeleportingPoint closest = null;
        foreach (var tp in teleportingPoints)
        {
            var d = Vector3.Distance(tp.transform.position, p);
            if (d < dist)
            {
                dist = d;
                closest = tp;
            }
        }

        if (dist < suckingRadius)
        {
            return closest;
        }
        else
        {
            return null;
        }
    }
    public static float suckingRadius = 0.4f;

    public void OnBeginTeleporting()
    {
        EnableAllSubRenderers();
    }

    public void OnStopTeleporting()
    {
        DisableAllSubRenderers();
    }

    public void OnEnterPoint(Teleporter teleporter)
    {
        if (teleporter != null && teleporter.VRInput != null)
        {
            teleporter.VRInput.TriggerHapticPulse(1000);
        }
        HighlightAllSubRenderers();
    }

    public void OnExitPoint()
    {
        DelightAllSubRenderers();
    }

    private void Awake()
    {
        GlobalEventManager.RegisterHandler("VRPlayer.BeginTeleportingPredict", OnBeginTeleporting);
        GlobalEventManager.RegisterHandler("VRPlayer.StopTeleportingPredict", OnStopTeleporting);
        allSubRenderers = GetComponentsInChildren<MeshRenderer>();
        List<MeshRenderer> rs = new List<MeshRenderer>();
        rs.AddRange(allSubRenderers);
        rs.RemoveAll(m => m.GetComponent<TextMesh>() != null);

        tintColors = rs.ToDictionary(r => r, r => r.materials[0].GetColor("_TintColor"));
        DelightAllSubRenderers();
        DisableAllSubRenderers();
        teleportingPoints.Add(this);
    }

    Dictionary<MeshRenderer, Color> tintColors = new Dictionary<MeshRenderer, Color>();
    MeshRenderer[] allSubRenderers = null;
    void DelightAllSubRenderers()
    {
        foreach (var m in tintColors)
        {
            var c = m.Value;
            c.a = 0.2f;
            m.Key.materials[0].SetColor("_TintColor", c);
        }
    }

    void HighlightAllSubRenderers()
    {
        foreach (var m in tintColors)
        {
            var c = m.Value;
            c.a = 1f;
            m.Key.materials[0].SetColor("_TintColor", c);
        }
    }

    void DisableAllSubRenderers()
    {
        foreach (var m in allSubRenderers)
        {
            m.enabled = false;
        }
    }

    void EnableAllSubRenderers()
    {
        foreach (var m in allSubRenderers)
        {
            m.enabled = true;
        }
    }

    private void Update()
    {

    }

    private void OnDestroy()
    {
        GlobalEventManager.UnregisterHandler("VRPlayer.BeginTeleportingPredict", OnBeginTeleporting);
        GlobalEventManager.UnregisterHandler("VRPlayer.StopTeleportingPredict", OnStopTeleporting);
        teleportingPoints.Remove(this);
    }
}
