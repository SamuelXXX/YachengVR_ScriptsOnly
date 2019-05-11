using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(RSAKeyHolder))]
public class RSAVerifier : MonoBehaviour
{
    #region Singleton
    protected static RSAVerifier instance;
    public static RSAVerifier Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    RSACryptoServiceProvider rsa;
    protected RSAKeyHolder mKeyHolder;
    bool hasPublicKey
    {
        get
        {
            return mKeyHolder != null && !string.IsNullOrEmpty(mKeyHolder.PublicKey);
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        mKeyHolder = GetComponent<RSAKeyHolder>();
        if (hasPublicKey)
        {
            rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(mKeyHolder.PublicKey);//Generate RSA Service Provider
        }
    }
    public string testVerifyCode;
    [ContextMenu("Verify")]
    public void Verify()
    {
        if (Verify(testVerifyCode))
            Debug.Log("Verify passed");
        else
            Debug.Log("Verify failed");
    }

    public bool Verify(string verifyCode)
    {
        try
        {
            byte[] dataToVerify = Encoding.UTF8.GetBytes(SystemInfo.deviceUniqueIdentifier.ToUpper().Trim());
            return rsa.VerifyData(dataToVerify, new SHA1CryptoServiceProvider(), Convert.FromBase64String(verifyCode));
        }
        catch
        {
            return false;
        }
    }
}

