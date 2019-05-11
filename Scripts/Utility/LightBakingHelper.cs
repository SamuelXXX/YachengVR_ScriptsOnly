using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBakingHelper : MonoBehaviour
{
    public List<Transform> movingDoors = new List<Transform>();
    

    [System.Serializable]
    public class BakingMorph
    {
        public Transform target;
        public Vector3 position;
        public Quaternion rotation;

        public BakingMorph(Transform t, Vector3 p, Quaternion r)
        {
            target = t;
            position = p;
            rotation = r;
        }
    }

    [SerializeField, HideInInspector]
    protected List<BakingMorph> openMorph = new List<BakingMorph>();
    [SerializeField, HideInInspector]
    protected List<BakingMorph> closeMorph = new List<BakingMorph>();



    #region Life Circle
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion


    #region Context Menus
    public bool allowBuild = false;
    [ContextMenu("Build Open")]
    public void BuildOpenDoorsMorph()
    {
        if (!allowBuild)
            return;

        openMorph.Clear();
        foreach (var c in movingDoors)
        {
            openMorph.Add(new BakingMorph(c.transform, c.transform.localPosition, c.transform.localRotation));
        }
        Debug.Log("Build Open Morph Succeed!");
        allowBuild = false;
    }

    [ContextMenu("Build Close")]
    public void BuildCloseDoorsMorph()
    {
        if (!allowBuild)
            return;

        closeMorph.Clear();
        foreach (var c in movingDoors)
        {
            closeMorph.Add(new BakingMorph(c.transform, c.transform.localPosition, c.transform.localRotation));
        }
        Debug.Log("Build Close Morph Succeed!");
        allowBuild = false;
    }

    [ContextMenu("Apply Open")]
    public void ApplyOpenDoors()
    {
        foreach (var c in openMorph)
        {
            c.target.localPosition = c.position;
            c.target.localRotation = c.rotation;
        }
    }

    [ContextMenu("Apply Close")]
    public void ApplyCloseDoors()
    {
        foreach (var c in closeMorph)
        {
            c.target.localPosition = c.position;
            c.target.localRotation = c.rotation;
        }
    }


    #endregion
}
