// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("UISystem")]
    public class AlphaTurning : FsmStateAction
    {
        [RequiredField]
        public FsmFloat time;
        public FsmFloat targetAlpha;

        public FsmEvent finishEvent;
        public FsmGameObject targetGameObject;


        private float timer;
        private UIAlphaDataHolder uiDataHolder;



        void SetColor(float timer)
        {

            if (uiDataHolder)
            {
                uiDataHolder.LerpAlpha(targetAlpha.Value, timer);
            }
        }


        public override void Reset()
        {
            time = 1f;
            finishEvent = null;
        }

        public override void OnEnter()
        {
            if (targetGameObject == null || targetGameObject.Value == null)
            {
                Finish();
                return;
            }

            uiDataHolder = targetGameObject.Value.GetComponent<UIAlphaDataHolder>();
            if (uiDataHolder == null)
            {
                uiDataHolder = targetGameObject.Value.AddComponent<UIAlphaDataHolder>();
            }
            uiDataHolder.BuildTarget();

            if (time.Value <= 0)
            {
                Fsm.Event(finishEvent);
                SetColor(1f);
                Finish();
                return;
            }

            timer = 0f;
        }

        public override void OnUpdate()
        {
            // update time
            timer += Time.deltaTime;

            if (timer >= time.Value)
            {
                SetColor(1f);
                Finish();
                if (finishEvent != null)
                {
                    Fsm.Event(finishEvent);
                }
            }
            else
            {
                float normalize = timer / time.Value;
                SetColor(normalize);
            }
        }

    }
}
