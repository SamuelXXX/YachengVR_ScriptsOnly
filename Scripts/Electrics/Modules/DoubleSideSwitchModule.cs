using Electrics.Module;
using Electrics.Connectivity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleSideSwitchModule : BaseModule
{
    [ExposedPort]
    public DigitalOutput output = new DigitalOutput();

    public void OnSwitchOff()
    {
        output.value = false;
    }

    public void OnSwitchOn()
    {
        output.value = true;
    }
}
