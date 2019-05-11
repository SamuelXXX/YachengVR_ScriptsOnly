using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppActivator : MonoBehaviour
{
    public Text deviceIdentifier;
    public InputField verifyCode;
    public Text log;

    public PlayMakerFSM verifyPassedFsm;
    // Use this for initialization
    void Start()
    {
        deviceIdentifier.text = SystemInfo.deviceUniqueIdentifier.ToUpper().Trim();
        if (PlayerPrefs.HasKey("LicenseCode"))
        {
            if (Verify(PlayerPrefs.GetString("LicenseCode")))
            {
                GetComponent<Canvas>().enabled = false;
                verifyPassedFsm.SendEvent("DirectLoad");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Activate()
    {
        string vc = verifyCode.text;
        if (Verify(vc))
        {
            PlayerPrefs.SetString("LicenseCode", vc);
            verifyPassedFsm.SendEvent("ActivateSucceed");
        }
        else
        {
            Log("验证未通过");
        }
    }

    [ContextMenu("Delete License")]
    public void DeleteCache()
    {
        PlayerPrefs.DeleteKey("LicenseCode");
    }

    bool Verify(string veriCode)
    {
        return RSAVerifier.Instance.Verify(veriCode);
    }

    public void Clip()
    {
        TextEditor textEditor = new TextEditor();
        textEditor.text = deviceIdentifier.text;
        textEditor.OnFocus();
        textEditor.Copy();
        Log("剪切成功");
    }

    public void Quit()
    {
        Application.Quit();
    }

    void Log(string m)
    {
        log.text = m;
        CancelInvoke("ShutLog");
        Invoke("ShutLog", 3f);
    }

    void ShutLog()
    {
        log.text = "";
    }
}
