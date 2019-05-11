using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayMakerFSM))]
public class RemotePanelHandler : MonoBehaviour
{
    protected PlayMakerFSM effectFsm;
    public Vector3 relativePosition = new Vector3(0, 0, 0.5f);

    #region Life Circle
    private void Start()
    {
        effectFsm = GetComponent<PlayMakerFSM>();
    }

    private void OnEnable()
    {
        GlobalEventManager.RegisterHandler("RemotePanel.Show", Show);
        GlobalEventManager.RegisterHandler("RemotePanel.Hide", Hide);
        GlobalEventManager.RegisterHandler("internal_VRPlayer.Repositioned", ResetPosition);
    }

    private void OnDisable()
    {
        GlobalEventManager.UnregisterHandler("RemotePanel.Show", Show);
        GlobalEventManager.UnregisterHandler("RemotePanel.Hide", Hide);
        GlobalEventManager.UnregisterHandler("internal_VRPlayer.Repositioned", ResetPosition);
    }
    #endregion

    #region External Control
    public void Show()
    {
        if (effectFsm != null)
        {
            effectFsm.SendEvent("Show");
        }

        ResetPosition();
    }

    public void ResetPosition()
    {
        transform.position = VRPlayer.Instance.Head.position + relativePosition;

        Vector3 dir = VRPlayer.Instance.Head.position - transform.position;
        dir.y = 0f;
        transform.LookAt(transform.position + dir);
    }

    public void Hide()
    {
        if (effectFsm != null)
        {
            effectFsm.SendEvent("Hide");
        }
    }
    #endregion
}
