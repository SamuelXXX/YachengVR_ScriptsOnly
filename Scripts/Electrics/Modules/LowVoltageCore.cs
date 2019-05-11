using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Electrics.Module;
using Electrics.Connectivity;
using Electrics.Utility;

public class LowVoltageCore : BaseModule
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
    public TextMesh displayMode;
    public TextMesh triphaseMeter1;
    public TextMesh triphaseMeter2;
    public TextMesh triphaseMeter3;
    #endregion

    #region Runtime data
    bool receivedSwitchPulseSignal;//internal wire signal
    bool isOn = false;//Switch status on or off
    bool showVoltage = false;

    PulseReader switchSignalReader = new PulseReader();
    #endregion

    const float phaseToWire = 1.732f;

    public void ShowVoltage()
    {
        showVoltage = true;
        displayMode.text = "三相电压";
    }

    public void ShowCurrent()
    {
        showVoltage = false;
        displayMode.text = "三相电流";
    }

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
            if (!showVoltage)
            {
                //Update Meters
                if (triphaseMeter1 != null)
                {
                    triphaseMeter1.text = ReadingsFormatter.Format(outputPower.current.magnitude);
                }
                if (triphaseMeter2 != null)
                {
                    triphaseMeter2.text = ReadingsFormatter.Format(outputPower.current.magnitude);
                }
                if (triphaseMeter3 != null)
                {
                    triphaseMeter3.text = ReadingsFormatter.Format(outputPower.current.magnitude);
                }
            }
            else
            {
                //Update Meters
                if (triphaseMeter1 != null)
                {
                    triphaseMeter1.text = ReadingsFormatter.Format(inputPower.voltage.magnitude * phaseToWire);
                }
                if (triphaseMeter2 != null)
                {
                    triphaseMeter2.text = ReadingsFormatter.Format(inputPower.voltage.magnitude * phaseToWire);
                }
                if (triphaseMeter3 != null)
                {
                    triphaseMeter3.text = ReadingsFormatter.Format(inputPower.voltage.magnitude * phaseToWire);
                }
            }

        }
        else
        {
            //Update Meters
            if (triphaseMeter1 != null)
            {
                triphaseMeter1.text = "";
            }
            if (triphaseMeter2 != null)
            {
                triphaseMeter2.text = "";
            }
            if (triphaseMeter3 != null)
            {
                triphaseMeter3.text = "";
            }
        }
    }
}
