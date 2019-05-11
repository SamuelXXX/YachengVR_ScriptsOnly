using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Electrics.Module;
using Electrics.Connectivity;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(LowVoltageModule))]
public class LowVoltageModuleEditor : CompositeModuleEditor<LowVoltageModule>
{

}
#endif

public class LowVoltageModule : CompositeModule
{
    [ExposedPort]
    public DigitalInput inputRemoteSwitch = new DigitalInput();
    [ExposedPort]
    public ElectricalInput inputPower = new ElectricalInput();
    [ExposedPort]
    public ElectricalOutput outputPower = new ElectricalOutput();
    [ExposedPort]
    public DigitalOutput outputSwitchState = new DigitalOutput();
}