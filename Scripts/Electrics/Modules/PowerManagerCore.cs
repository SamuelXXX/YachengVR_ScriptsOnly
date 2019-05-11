using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Electrics.Module;
using Electrics.Connectivity;
using Electrics.Utility;

public class PowerManagerCore : BaseModule
{
    #region Settings
    [ExposedPort]
    public ElectricalInput inputPower = new ElectricalInput();
    [ExposedPort]
    public ElectricalOutput outputPower = new ElectricalOutput();

    [ExposedPort]
    public DigitalOutput outputOpenIndicator = new DigitalOutput();
    [ExposedPort]
    public DigitalOutput outputCloseIndicator = new DigitalOutput();
    [ExposedPort]
    public DigitalOutput outputSwitchState = new DigitalOutput();

    [ExposedPort]
    public DigitalInput inputLocalRemote = new DigitalInput();
    [ExposedPort]
    public DigitalInput inputLocalSwitch = new DigitalInput();
    [ExposedPort]
    public DigitalInput inputRemoteSwitch = new DigitalInput();

    [Header("Meters")]
    public TextMesh meter;
    #endregion


    #region Run-time data
    bool receivedSwitchPulseSignal;
    bool isOn = false;

    PulseReader switchSignalReader = new PulseReader();
    #endregion

    protected override void UpdateModule()
    {
        bool hasPower = !inputPower.voltage.IsZero();

        if (hasPower)
        {
            if (!inputLocalRemote.value)
            {
                receivedSwitchPulseSignal = inputRemoteSwitch.value;
            }
            else
            {
                receivedSwitchPulseSignal = inputLocalSwitch.value;
            }

            float width = switchSignalReader.ReadWidth(receivedSwitchPulseSignal, Time.deltaTime);
            if (width > 0.1f && width < 0.3f)
            {
                isOn = true;
            }
            else if (width >= 0.3f)
            {
                isOn = false;
            }
        }

        ElectricalPort.Transfer(inputPower, outputPower, isOn);
        outputSwitchState.value = isOn;
        if (hasPower)
        {
            outputCloseIndicator.value = isOn;
            outputOpenIndicator.value = !isOn;
        }
        else
        {
            outputCloseIndicator.value = false;
            outputOpenIndicator.value = false;
        }

        if (hasPower)
        {
            //Update Meters
            if (meter != null)
            {
                meter.text = ReadingsFormatter.Format(outputPower.current.magnitude);
            }
        }
        else
        {
            //Update Meters
            if (meter != null)
            {
                meter.text = "";
            }
        }
    }
}
