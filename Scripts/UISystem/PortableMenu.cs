using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PortableMenu : MonoBehaviour
{
    public static PortableMenu SceneInstance { get; private set; }
    public bool portableMenuEnabled = false;
    protected PlayMakerFSM effectFsm;

    #region Internal Control
    bool GetCallMenuCommand()
    {
        for (int i = 0; i < 16; i++)
        {
            SteamVR_Controller.Device device = SteamVR_Controller.Input(i);
            if (device != null && device.GetPressDown(EVRButtonId.k_EButton_ApplicationMenu))
            {
                return true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            return true;
        }
        return false;
    }

    void EnablePortableMenu()
    {
        portableMenuEnabled = true;
    }

    void DisablePortableMenu()
    {
        portableMenuEnabled = false;
        KillPortableMenu();
    }

    void SwitchPortableMenu()
    {
        if (!portableMenuEnabled)
            return;
        if (menuOn)
        {
            KillPortableMenu();
        }
        else
        {
            CallPortableMenu();
        }
    }

    public bool menuOn = false;
    void CallPortableMenu()
    {
        if (effectFsm != null)
        {
            effectFsm.SendEvent("Show");
            menuOn = true;
        }
    }

    void KillPortableMenu()
    {
        if (effectFsm != null)
        {
            effectFsm.SendEvent("Hide");
            menuOn = false;
        }
    }
    #endregion


    #region Life Circle
    private void Awake()
    {
        if (SceneInstance != null)
        {
            Destroy(this);
        }
        else if (SceneInstance == null)
        {
            SceneInstance = this;
        }

        GlobalEventManager.RegisterHandler("PortableMenu.Enable", EnablePortableMenu);
        GlobalEventManager.RegisterHandler("PortableMenu.Disable", DisablePortableMenu);

        effectFsm = GetComponent<PlayMakerFSM>();
    }

    private void OnDestroy()
    {
        if (SceneInstance == this)
        {
            SceneInstance = null;
        }

        GlobalEventManager.UnregisterHandler("PortableMenu.Enable", EnablePortableMenu);
        GlobalEventManager.UnregisterHandler("PortableMenu.Disable", DisablePortableMenu);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GetCallMenuCommand())
        {
            SwitchPortableMenu();
        }
    }
    #endregion


}
