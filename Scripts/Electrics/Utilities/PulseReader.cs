using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Electrics.Utility
{
    public class PulseReader
    {
        float timer = 0f;
        // Update is called once per frame
        public float ReadWidth(bool pulseValue, float deltaTime)
        {
            if (pulseValue)
            {
                timer += deltaTime;
                return 0f;
            }
            else
            {
                float ret = timer;
                timer = 0f;
                return ret;
            }
        }
    }
}

