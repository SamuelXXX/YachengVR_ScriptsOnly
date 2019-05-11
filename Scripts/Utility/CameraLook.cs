using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour {

    // Use this for initialization
    public Transform target;
    void Update()
    {
        transform.LookAt(target);
    }
}
