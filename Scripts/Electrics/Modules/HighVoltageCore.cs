using Electrics.Module;
using Electrics.Connectivity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Electrics.Utility;

public class HighVoltageCore : BaseModule
{

    #region Settings
    [ExposedPort]
    public ElectricalInput inputPower = new ElectricalInput();
    [ExposedPort]
    public ElectricalOutput outputPower = new ElectricalOutput();
    [ExposedPort]
    public DigitalInput inputLocalRemote = new DigitalInput();
    [ExposedPort]
    public DigitalInput inputSwitch = new DigitalInput();
    [ExposedPort]
    public DigitalInput inputGround = new DigitalInput();
    [ExposedPort]
    public DigitalOutput outputOpenIndicator = new DigitalOutput();
    [ExposedPort]
    public DigitalOutput outputCloseIndicator = new DigitalOutput();
    [ExposedPort]
    public DigitalOutput outputSwitchState = new DigitalOutput();
    [ExposedPort]
    public DigitalOutput outputGroundOpenIndicator = new DigitalOutput();
    [ExposedPort]
    public DigitalOutput outputGroundCloseIndicator = new DigitalOutput();
    [ExposedPort]
    public DigitalInput inputRemoteSwitch = new DigitalInput();

    [Header("Meters")]
    public TextMesh triphaseMeter1;
    public TextMesh triphaseMeter2;
    public TextMesh triphaseMeter3;
    public GameObject safeScreen;
    public TextMesh inputVoltageMeter;


    [Header("Effect")]
    public PlayMakerFSM closeEffect;
    public List<Rotator> rotateEffects = new List<Rotator>();
    #endregion


    #region Run-time data
    const float phaseToWire = 1.732f;
    bool receivedSwitchPulseSignal;//internal wire signal
    bool isOn = false;//Switch status on or off
    bool groundClosed = false;

    PulseReader switchSignalReader = new PulseReader();
    #endregion

    #region Module Life Circle
    protected override void UpdateModule()
    {
        bool hasPower = !inputPower.voltage.IsZero();

        if (hasPower && !groundClosed)//Process switching only when powered on
        {
            if (!inputLocalRemote.value)
            {
                receivedSwitchPulseSignal = inputRemoteSwitch.value;
            }
            else
            {
                receivedSwitchPulseSignal = inputSwitch.value;
            }


            float width = switchSignalReader.ReadWidth(receivedSwitchPulseSignal, Time.deltaTime);
            if (width > 0.1f && width < 0.3f)
            {
                StartCoroutine("OpenRoutine");
            }
            else if (width >= 0.3f)
            {
                StartCoroutine("CloseRoutine");
            }
            safeScreen.SetActive(true);
        }
        else if (!hasPower)
        {
            safeScreen.SetActive(false);
        }

        ElectricalPort.Transfer(inputPower, outputPower, isOn);
        outputSwitchState.value = isOn;

        if (hasPower)
        {
            outputCloseIndicator.value = isOn;
            outputOpenIndicator.value = !isOn;

            outputGroundCloseIndicator.value = groundClosed;
            outputGroundOpenIndicator.value = !groundClosed;
        }
        else
        {
            outputCloseIndicator.value = false;
            outputOpenIndicator.value = false;

            outputGroundCloseIndicator.value = false;
            outputGroundOpenIndicator.value = false;
        }

        if (inputVoltageMeter != null)
        {
            float volDisplay = inputPower.voltage.magnitude * phaseToWire;
            if (volDisplay > 1000)
            {
                inputVoltageMeter.text = (volDisplay / 1000f).ToString("f1") + "kV";
            }
            else
            {
                inputVoltageMeter.text = volDisplay.ToString("f0") + "V";
            }
        }

        if (hasPower)
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


    #endregion

    #region OpenClose Action
    bool routineRunning = false;
    IEnumerator OpenRoutine()
    {
        if (!routineRunning && isOn)
        {
            routineRunning = true;
            isOn = false;
            closeEffect.SendEvent("Open");
            foreach (var e in rotateEffects)
            {
                e.Rotate("Open",0.05f);
            }          
            //yield return new WaitForSeconds(0.05f);
            //foreach (var e in rotateEffects)
            //{
            //    e.Rotate("Open", 9.95f);
            //}
            yield return new WaitForSeconds(10f);
            closeEffect.SendEvent("StopOpen");
            routineRunning = false;
        }
    }

    void DirectOpen()
    {
        isOn = false;
        closeEffect.SendEvent("DirectOpen");
        foreach (var e in rotateEffects)
        {
            e.Rotate("Open", 0.05f);          
        }
    }

    IEnumerator CloseRoutine()
    {
        if (!routineRunning && !isOn)
        {
            routineRunning = true;
            closeEffect.SendEvent("StartClose");
            //foreach (var e in rotateEffects)
            //{
            //    e.Rotate("Close", 12f);
            //}
            yield return new WaitForSeconds(10f);
            foreach (var e in rotateEffects)
            {
                e.Rotate("Close", 0.05f);
            }
            closeEffect.SendEvent("Close");
            isOn = true;
            routineRunning = false;
        }
    }
    #endregion

    #region
    public void CloseGround()
    {
        groundClosed = true;
        DirectOpen();
    }

    public void OpenGround()
    {
        groundClosed = false;
    }
    #endregion
}
