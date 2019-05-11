using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    public class ShowLogo : FsmStateAction
    {

        public FsmGameObject logo;

        // Use this for initialization
        public override void OnEnter()
        {
            logo.Value.SetActive(true);
            Finish();
        }
    }
}
