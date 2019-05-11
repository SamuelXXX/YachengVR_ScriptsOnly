using Electrics.Module;
using Electrics.Connectivity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalPowerSourceModule : BaseModule
{
    [ExposedPort]
    public ElectricalOutput outPower = new ElectricalOutput();
}
