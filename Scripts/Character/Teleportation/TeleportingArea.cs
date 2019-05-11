using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportingArea : MonoBehaviour
{
    public void OnTeleport()
    {
        if (GetComponent<PlayMakerFSM>() != null)
        {
            GetComponent<PlayMakerFSM>().SendEvent("Trigger");
        }
    }

    public void OnBeginTeleporting()
    {
        GetComponent<MeshRenderer>().enabled = true;
    }

    public void OnStopTeleporting()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    public void OnEnterArea(Teleporter teleporter)
    {
        if (teleporter != null && teleporter.VRInput != null)
        {
            teleporter.VRInput.TriggerHapticPulse(500);
        }

        if (mRenderer != null)
        {
            var c = tintColor;
            c.a = 1f;
            mRenderer.materials[0].SetColor("_TintColor", c);
        }
    }

    public void OnExitArea()
    {
        if (mRenderer != null)
        {
            var c = tintColor;
            c.a = 0.5f;
            mRenderer.materials[0].SetColor("_TintColor", c);
        }
    }

    Color tintColor;
    MeshRenderer mRenderer;
    private void Awake()
    {
        GlobalEventManager.RegisterHandler("VRPlayer.BeginTeleportingPredict", OnBeginTeleporting);
        GlobalEventManager.RegisterHandler("VRPlayer.StopTeleportingPredict", OnStopTeleporting);
        mRenderer = GetComponent<MeshRenderer>();
        if (mRenderer != null)
        {
            tintColor = mRenderer.materials[0].GetColor("_TintColor");
            var c = tintColor;
            c.a = 0.5f;
            mRenderer.materials[0].SetColor("_TintColor", c);
        }
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnDestroy()
    {
        GlobalEventManager.UnregisterHandler("VRPlayer.BeginTeleportingPredict", OnBeginTeleporting);
        GlobalEventManager.UnregisterHandler("VRPlayer.StopTeleportingPredict", OnStopTeleporting);
    }
}
