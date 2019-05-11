// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("GlobalEvent")]
    [Tooltip("Wait global event.")]
    public class SendGlobalEventAdvanced : FsmStateAction
    {
        public FsmString eventName = "";

        public override void OnEnter()
        {
            GlobalEventManager.SendEvent(eventName.Value);
            Finish();
        }
    }
}