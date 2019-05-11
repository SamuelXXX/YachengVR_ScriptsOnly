using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBillBoard : MonoBehaviour
{


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = VRPlayer.Instance.Head.transform.position - transform.position;
        direction.y = 0f;
        transform.LookAt(transform.position + direction);
    }
}
