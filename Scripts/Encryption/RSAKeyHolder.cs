using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public class RSAKeyHolder : MonoBehaviour
{
    public string keyStorePath;

    #region Key Generation
#if UNITY_EDITOR
    string AbsoluteStorePath
    {
        get
        {
            return Application.dataPath + "/" + keyStorePath;
        }
    }

    string PrivateKeyPath
    {
        get
        {
            return AbsoluteStorePath + "/" + "RSAPrivateKey.xml";
        }
    }

    string PublicKeyPath
    {
        get
        {
            return AbsoluteStorePath + "/" + "RSAPublicKey.xml";
        }
    }

    [ContextMenu("GenerateKey")]
    public void GenerateKey()
    {
        if (!Directory.Exists(AbsoluteStorePath))
        {
            Directory.CreateDirectory(AbsoluteStorePath);
        }

        if (!File.Exists(PrivateKeyPath) || !File.Exists(PublicKeyPath))
        {
            if (File.Exists(PrivateKeyPath))
                File.Delete(PrivateKeyPath);
            if (File.Exists(PublicKeyPath))
                File.Delete(PublicKeyPath);
            StreamWriter privateKeyFile = File.CreateText(PrivateKeyPath);
            StreamWriter publicKeyFile = File.CreateText(PublicKeyPath);

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            string privateKey = rsa.ToXmlString(true);
            string publicKey = rsa.ToXmlString(false);

            privateKeyFile.Write(XMLFormat(privateKey));
            publicKeyFile.Write(XMLFormat(publicKey));

            privateKeyFile.Close();
            publicKeyFile.Close();

            Debug.Log("Key pair created");
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

    public string XMLFormat(string xml)
    {
        System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
        doc.LoadXml(xml);

        System.IO.StringWriter sw = new System.IO.StringWriter();
        using (System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(sw))
        {
            writer.Indentation = 2;  // the Indentation
            writer.Formatting = System.Xml.Formatting.Indented;
            doc.WriteContentTo(writer);
            writer.Close();
        }
        return sw.ToString();
    }
#endif
    #endregion

    [SerializeField, HideInInspector]
    protected string publicKey;
    public string PublicKey
    {
        get
        {
            return publicKey;
        }
    }

    #region Life Circle
    private void Awake()
    {
        
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        //Try Generate Key and get public key in editor
        publicKey = GetPublicKey();
    }
#endif

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(RSAKeyHolder))]
public class RSAKeyHolderEditor : Editor
{
    public RSAKeyHolder Target
    {
        get
        {
            return target as RSAKeyHolder;
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Public Key:\n" + Target.PublicKey, GUILayout.Height(80f));
        EditorGUILayout.Space();
        base.OnInspectorGUI();
    }
}
#endif
