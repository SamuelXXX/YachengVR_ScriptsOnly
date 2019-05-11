using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Electrics.Module;
using Electrics.Connectivity;
using Electrics.Utility;

public class PowerLoadModule : BaseModule
{
    #region Settings
    [ExposedPort]
    public ElectricalInput inputPower = new ElectricalInput();

    public Complex resistence = new Complex(300, 0);
    #endregion

    protected override void UpdateModule()
    {
        inputPower.current = inputPower.voltage / resistence;
    }
}
