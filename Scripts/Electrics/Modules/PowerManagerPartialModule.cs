using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Electrics.Module;
using Electrics.Connectivity;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PowerManagerPartialModule))]
public class PowerManagerPartialModuleEditor : CompositeModuleEditor<PowerManagerPartialModule>
{

}
#endif

public class PowerManagerPartialModule : CompositeModule
{
    [ExposedPort]
    public ElectricalInput inputPower = new ElectricalInput();
    [ExposedPort]
    public ElectricalOutput outputPower = new ElectricalOutput();
    [ExposedPort]
    public DigitalInput inputRemoteSwitch = new DigitalInput();
    [ExposedPort]
    public DigitalOutput outputSwitchState = new DigitalOutput();
}
