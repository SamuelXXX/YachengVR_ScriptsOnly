using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class MeshDivider : MonoBehaviour
{
    public string meshStorePath;
    [HideInInspector]
    public MeshRenderer mRenderer;
    [HideInInspector]
    public MeshFilter mFilter;

    private void OnValidate()
    {
        mRenderer = GetComponent<MeshRenderer>();
        mFilter = GetComponent<MeshFilter>();

        Debug.Log(mFilter.sharedMesh.subMeshCount);

    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

#if UNITY_EDITOR
    void CreateSubMesh()
    {
        Mesh[] subMesh = new Mesh[mFilter.sharedMesh.subMeshCount];

        Mesh sharedMesh = mFilter.sharedMesh;
        Vector3[] vertices = sharedMesh.vertices;
        Vector3[] normals = sharedMesh.normals;
        Vector4[] tangents = sharedMesh.tangents;
        for (int i = 0; i < subMesh.Length; i++)
        {
            subMesh[i] = new Mesh();
            
        }
    }

    void CreateAsset<T>(T asset, string nameWithoutPostfix, int index) where T : Object
    {
        if (asset == null)
            return;
        string relativePath = "";

        relativePath = meshStorePath;

        var assetPath = relativePath + nameWithoutPostfix + index.ToString() + ".asset";
        T t = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        if (t != null)
            AssetDatabase.DeleteAsset(assetPath);

        AssetDatabase.CreateAsset(asset, assetPath);
        //AssetDatabase.SetLabels(asset, new string[1] { generatedAssetLabel });
    }
#endif
}
