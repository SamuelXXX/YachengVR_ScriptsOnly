using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Electrics.Module;
using Electrics.Connectivity;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PowerManagerModule))]
public class PowerManagerModuleEditor : CompositeModuleEditor<PowerManagerModule>
{

}
#endif

public class PowerManagerModule : CompositeModule
{

    [ExposedPort]
    public ElectricalInput inputPower1 = new ElectricalInput();
    [ExposedPort]
    public DigitalInput inputRemoteSwitch1 = new DigitalInput();
    [ExposedPort]
    public ElectricalOutput outputPower1 = new ElectricalOutput();
    [ExposedPort]
    public DigitalOutput outputSwitchState1 = new DigitalOutput();

    [ExposedPort]
    public ElectricalInput inputPower2 = new ElectricalInput();
    [ExposedPort]
    public DigitalInput inputRemoteSwitch2 = new DigitalInput();
    [ExposedPort]
    public ElectricalOutput outputPower2 = new ElectricalOutput();
    [ExposedPort]
    public DigitalOutput outputSwitchState2 = new DigitalOutput();

    [ExposedPort]
    public ElectricalInput inputPower3 = new ElectricalInput();
    [ExposedPort]
    public DigitalInput inputRemoteSwitch3 = new DigitalInput();
    [ExposedPort]
    public ElectricalOutput outputPower3 = new ElectricalOutput();
    [ExposedPort]
    public DigitalOutput outputSwitchState3 = new DigitalOutput();

    [ExposedPort]
    public ElectricalInput inputCompensation = new ElectricalInput();

}
