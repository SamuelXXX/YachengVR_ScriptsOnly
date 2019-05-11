using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Electrics.Connectivity;
using Electrics.Module;

public class TransformerModule : BaseModule
{
    [ExposedPort]
    public ElectricalInput inputPower = new ElectricalInput();
    [ExposedPort]
    public ElectricalOutput outputPower = new ElectricalOutput();

    public float transformRatio = 0.5f;

    protected override void UpdateModule()
    {
        outputPower.voltage = inputPower.voltage * transformRatio;
        outputPower.frequency = inputPower.frequency;
        outputPower.maxPower = inputPower.maxPower;

        inputPower.current = outputPower.current * transformRatio;
    }
}
