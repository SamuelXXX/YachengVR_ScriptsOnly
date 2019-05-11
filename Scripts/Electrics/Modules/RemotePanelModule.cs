using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Electrics.Module;
using Electrics.Connectivity;

public class RemotePanelModule : BaseModule
{
    #region Settings
    [ExposedPort,Header("Input")]
    public DigitalInput inputHVIndicator = new DigitalInput();
    [ExposedPort]
    public DigitalInput inputLVIndicator = new DigitalInput();
    [ExposedPort]
    public DigitalInput inputPMLoad1Indicator = new DigitalInput();
    [ExposedPort]
    public DigitalInput inputPMLoad2Indicator = new DigitalInput();
    [ExposedPort]
    public DigitalInput inputPMLoad3Indicator = new DigitalInput();


    [ExposedPort, Header("Output")]
    public DigitalOutput outputHVTrigger = new DigitalOutput();
    [ExposedPort]
    public DigitalOutput outputLVTrigger = new DigitalOutput();
    [ExposedPort]
    public DigitalOutput outputPMLoad1Trigger = new DigitalOutput();
    [ExposedPort]
    public DigitalOutput outputPMLoad2Trigger = new DigitalOutput();
    [ExposedPort]
    public DigitalOutput outputPMLoad3Trigger = new DigitalOutput();

    [Header("OpenClose Pictures")]
    public GameObject HVClose;
    public GameObject LVClose;
    public GameObject Load1Open;
    public GameObject Load2Open;
    public GameObject Load3Open;
    #endregion

    #region External Control
    public void CloseHVTrigger()
    {
        StartCoroutine(PulseRoutine(outputHVTrigger, 0.4f));
    }

    public void OpenHVTrigger()
    {
        StartCoroutine(PulseRoutine(outputHVTrigger, 0.2f));
    }

    public void CloseLVTrigger()
    {
        StartCoroutine(PulseRoutine(outputLVTrigger, 0.2f));
    }

    public void OpenLVTrigger()
    {
        StartCoroutine(PulseRoutine(outputLVTrigger, 0.4f));
    }

    public void CloseLoad1Trigger()
    {
        StartCoroutine(PulseRoutine(outputPMLoad1Trigger, 0.2f));
    }

    public void OpenLoad1Trigger()
    {
        StartCoroutine(PulseRoutine(outputPMLoad1Trigger, 0.4f));
    }

    public void CloseLoad2Trigger()
    {
        StartCoroutine(PulseRoutine(outputPMLoad2Trigger, 0.2f));
    }

    public void OpenLoad2Trigger()
    {
        StartCoroutine(PulseRoutine(outputPMLoad2Trigger, 0.4f));
    }

    public void CloseLoad3Trigger()
    {
        StartCoroutine(PulseRoutine(outputPMLoad3Trigger, 0.2f));
    }

    public void OpenLoad3Trigger()
    {
        StartCoroutine(PulseRoutine(outputPMLoad3Trigger, 0.4f));
    }
    #endregion

    IEnumerator PulseRoutine(DigitalOutput output,float width)
    {
        output.value = true;
        yield return new WaitForSeconds(width);
        output.value = false;
    }

    protected override void UpdateModule()
    {
        HVClose.SetActive(inputHVIndicator.value);
        LVClose.SetActive(inputLVIndicator.value);
        Load1Open.SetActive(!inputPMLoad1Indicator.value);
        Load2Open.SetActive(!inputPMLoad2Indicator.value);
        Load3Open.SetActive(!inputPMLoad3Indicator.value);
    }

}
