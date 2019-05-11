using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class TransformSnapshot
{
    public string snapshotName;
    public Vector3 position;
    public Quaternion rotation;
    public bool allowBuild;
    public bool editorFolded;

    public void Build(Transform t)
    {
        if (t == null)
            return;

        if (allowBuild)
        {
            position = t.localPosition;
            rotation = t.localRotation;
            allowBuild = false;
        }
    }

    public void Apply(Transform t)
    {
        if (t == null)
            return;

        t.localPosition = position;
        t.localRotation = rotation;
    }

#if UNITY_EDITOR
    public bool DoEditorLayout(Transform t)
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button(editorFolded ? "+" : "-", GUILayout.Height(15f), GUILayout.Width(18f)))
        {
            editorFolded = !editorFolded;
        }

        EditorGUILayout.LabelField(snapshotName);
        if (!editorFolded)
        {
            if (GUILayout.Button("del", GUILayout.Height(15f), GUILayout.Width(48f)))
            {
                return true;
            }
        }
        EditorGUILayout.EndHorizontal();
        if (!editorFolded)
        {
            snapshotName = EditorGUILayout.TextField("name", snapshotName);
            EditorGUILayout.LabelField("Position:" + position.ToString());
            EditorGUILayout.LabelField("Rotation:" + rotation.ToString());
            allowBuild = EditorGUILayout.Toggle("Allow Build", allowBuild);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Build"))
            {
                Build(t);
            }

            if (GUILayout.Button("Apply"))
            {
                Apply(t);
            }

            EditorGUILayout.EndHorizontal();
        }
        return false;
    }
#endif
}

public class Rotator : MonoBehaviour
{
    public List<TransformSnapshot> transformSnapshots = new List<TransformSnapshot>();
    public string rotateSnapShot;
    public float lerpTime;

    protected Dictionary<string, TransformSnapshot> mapper = new Dictionary<string, TransformSnapshot>();
    public void Rotate(string snapShotName, float lerpTime)
    {
        if (!mapper.ContainsKey(snapShotName))
            return;

        TransformSnapshot t = mapper[snapShotName];
        StopAllCoroutines();
        StartCoroutine(RotateRoutine(transform.localPosition, transform.localRotation, t.position, t.rotation, lerpTime));
    }

    [ContextMenu("Rotate")]
    public void Rotate()
    {
        Rotate(rotateSnapShot);
    }

    public void Rotate(string snapShotName)
    {
        Rotate(snapShotName, lerpTime);
    }

    public void RotateBetween(string start, string stop, float lerp)
    {
        lerp = Mathf.Clamp(lerp, 0, 1f);
        if (!mapper.ContainsKey(start) || !mapper.ContainsKey(stop))
            return;

        TransformSnapshot startPos = mapper[start];
        TransformSnapshot stopPos = mapper[stop];
        transform.localPosition = Vector3.Lerp(startPos.position, stopPos.position, lerp);
        transform.localRotation = Quaternion.Lerp(startPos.rotation, stopPos.rotation, lerp);
    }


    IEnumerator RotateRoutine(Vector3 op, Quaternion or, Vector3 tp, Quaternion tr, float lerpTime)
    {
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime / lerpTime;
            if (timer >= 1f)
            {
                transform.localPosition = tp;
                transform.localRotation = tr;
                break;
            }

            transform.localPosition = Vector3.Lerp(op, tp, timer);
            transform.localRotation = Quaternion.Lerp(or, tr, timer);
            yield return null;
        }
    }

    // Use this for initialization
    void Start()
    {
        mapper = transformSnapshots.ToDictionary<TransformSnapshot, string, TransformSnapshot>(p => p.snapshotName, p => p);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Rotator))]
public class RotatorEditor : Editor
{
    public Rotator Target
    {
        get
        {
            return target as Rotator;
        }
    }

    public override void OnInspectorGUI()
    {
        List<TransformSnapshot> removeShots = new List<TransformSnapshot>();
        foreach (var c in Target.transformSnapshots)
        {
            if (c.DoEditorLayout(Target.transform))
            {
                removeShots.Add(c);
            }
        }

        foreach (var r in removeShots)
        {
            Target.transformSnapshots.Remove(r);
        }

        if (GUILayout.Button("Add"))
        {
            var t = new TransformSnapshot();
            t.snapshotName = "EmptySnapShotName";
            Target.transformSnapshots.Add(t);
        }


        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Test Bench");
        Target.rotateSnapShot = EditorGUILayout.TextField("Target", Target.rotateSnapShot);
        Target.lerpTime = EditorGUILayout.FloatField("Lerp Time", Target.lerpTime);
        if (Application.isPlaying)
        {
            if (GUILayout.Button("Rotate"))
            {
                Target.Rotate(Target.rotateSnapShot, Target.lerpTime);
            }
        }


        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

    }
}

#endif
