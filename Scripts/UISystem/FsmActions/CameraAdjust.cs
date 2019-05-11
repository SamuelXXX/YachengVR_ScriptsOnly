// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("UISystem")]
    public class CameraAdjust : FsmStateAction
    {
        [RequiredField]
        public FsmFloat time;
        public FsmColor targetColor;
        public FsmEvent finishEvent;

        private float timer;
        Color originalColor;

        void BuildTarget()
        {
            originalColor = VRPlayer.Instance.GetMaskColor();
        }

        void SetColor(float timer)
        {
            VRPlayer.Instance.AdjustCameraMask(Color.Lerp(originalColor, targetColor.Value, timer));
        }


        public override void Reset()
        {
            time = 1f;
            finishEvent = null;
        }

        public override void OnEnter()
        {

            BuildTarget();

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
