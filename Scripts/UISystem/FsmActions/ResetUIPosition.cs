// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("UISystem")]
    public class ResetUIPosition : FsmStateAction
    {
        [RequiredField]
        public FsmGameObject targetObject;
        public FsmFloat distance = 2f;


        public override void OnEnter()
        {

            Vector3 forward = VRPlayer.Instance.Head.transform.forward;
            forward.y = 0f;
            forward = forward.normalized;
            Vector3 position = forward * distance.Value + VRPlayer.Instance.Head.transform.position;
            if (targetObject.Value != null)
            {
                targetObject.Value.transform.position = position;
                targetObject.Value.transform.LookAt(2 * position - VRPlayer.Instance.Head.transform.position);
            }
            Finish();
        }


    }
}
