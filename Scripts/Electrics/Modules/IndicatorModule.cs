using Electrics.Module;
using Electrics.Connectivity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IndicatorBulb))]
public class IndicatorModule : BaseModule
{
    public bool reverse = false;
    [ExposedPort]
    public DigitalInput input = new DigitalInput();
    protected IndicatorBulb mBulb;
    protected void Awake()
    {
        mBulb = GetComponent<IndicatorBulb>();
    }

    protected override void UpdateModule()
    {
        bool realInput = reverse ? !input.value : input.value;
        if (realInput)
        {
            mBulb.On();
        }
        else
        {
            mBulb.Off();
        }
    }
}
