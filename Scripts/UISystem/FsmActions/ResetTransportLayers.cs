using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("UISystem")]
    public class ResetTransportLayers : FsmStateAction
    {
        public GameObject transportObject;
        public GameObject destroyObject;

        public override void OnEnter()
        {
            transportObject.layer = LayerMask.NameToLayer("TransportingGround");
            MonoBehaviour.Destroy(destroyObject);
            Finish();
        }
    }
}
