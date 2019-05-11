using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class RSARegister : MonoBehaviour
{
    public string keyStorePath;
    public string verifyCodeStorePath;


    public string inputDeviceIdentifier;

    #region Key Access
    string PrivateKeyPath
    {
        get
        {
            return keyStorePath + "/" + "RSAPrivateKey.xml";
        }
    }

    string PublicKeyPath
    {
        get
        {
            return keyStorePath + "/" + "RSAPublicKey.xml";
        }
    }

    public string GetPublicKey()
    {
        if (File.Exists(PublicKeyPath))
        {
            StreamReader publicKeyReader = File.OpenText(PublicKeyPath);
            string ret = publicKeyReader.ReadToEnd();
            ret.Trim();
            publicKeyReader.Close();
            return ret;
        }
        return null;
    }

    public string GetPrivateKey()
    {
        if (File.Exists(PrivateKeyPath))
        {
            StreamReader PrivateKeyReader = File.OpenText(PrivateKeyPath);
            string ret = PrivateKeyReader.ReadToEnd();
            ret.Trim();
            PrivateKeyReader.Close();
            return ret;
        }
        return null;
    }
    #endregion

    #region Verify Code File Generation
    [ContextMenu("Generate")]
    public bool GenerateVerifyFile()
    {
        if (string.IsNullOrEmpty(keyStorePath) || string.IsNullOrEmpty(verifyCodeStorePath) || string.IsNullOrEmpty(inputDeviceIdentifier))
        {
            return false;
        }
        string verifyPath = verifyCodeStorePath + "/" + "License.txt";
        if (!Directory.Exists(verifyCodeStorePath))
        {
            Directory.CreateDirectory(verifyCodeStorePath);
        }

        if (File.Exists(verifyPath))
        {
            File.Delete(verifyPath);
        }

        StreamWriter veriCodeWriter = File.CreateText(verifyPath);
        var rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(GetPrivateKey());//Generate RSA Service Provider
        byte[] signedBytes = rsa.SignData(Encoding.UTF8.GetBytes(inputDeviceIdentifier.ToUpper().Trim()), new SHA1CryptoServiceProvider()); ;
        veriCodeWriter.WriteLine(Convert.ToBase64String(signedBytes));
        veriCodeWriter.Close();
        return true;
    }
    #endregion

    #region Singleton
    protected static RSARegister instance;
    public static RSARegister Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    #region Life Circle
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
    }

    // Use this for initialization
    void Start()
    {
        inputDeviceIdentifier = SystemInfo.deviceUniqueIdentifier;
    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion
}
