using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    public class ShowCircle : FsmStateAction
    {
        public FsmGameObject circle;
        public FsmFloat distance = 2f;

        // Use this for initialization
        public override void OnEnter()
        {
            if (circle != null)
            {
                Vector3 circleXZTarget = Vector3.Normalize(Vector3.ProjectOnPlane(VRPlayer.Instance.Head.transform.forward, Vector3.up)) * distance.Value;
                circle.Value.transform.position = new Vector3(circleXZTarget.x, circle.Value.transform.position.y, circleXZTarget.z);
                circle.Value.SetActive(true);
            }
            Finish();
        }

    }
}
