using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeshData
{
    public string meshName;
    public MeshFilter target;
    public long trisCount;

    public MeshData(MeshFilter meshFilter)
    {
        if (meshFilter == null)
            return;

        target = meshFilter;
        meshName = target.name;
        Mesh mesh = target.sharedMesh;
        //trisCount = 0;

        trisCount = mesh.triangles.Length;

        meshName += "\t\t\t" + ((float)trisCount / 1000f).ToString() + "k";
    }
}

public class MeshStatistic : MonoBehaviour
{
    public List<MeshData> target = new List<MeshData>();
    public long sum = 0;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [ContextMenu("Get Sub Mesh")]
    public void GetSubMeshStatus()
    {
        target.Clear();
        sum = 0;
        MeshFilter[] mfs = GetComponentsInChildren<MeshFilter>();

        foreach (var m in mfs)
        {
            var t = new MeshData(m);
            target.Add(t);
            sum += t.trisCount;


        }
    }
}
