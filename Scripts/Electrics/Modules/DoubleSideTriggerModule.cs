using Electrics.Module;
using Electrics.Connectivity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleSideTriggerModule : BaseModule
{
    //The output must be a pulse
    [ExposedPort]
    public DigitalOutput output = new DigitalOutput();

    public void OnTriggerRightSide()
    {
        StartCoroutine(PulseRoutine(0.4f));
    }

    public void OnTriggerLeftSide()
    {
        StartCoroutine(PulseRoutine(0.2f));
    }

    IEnumerator PulseRoutine(float width)
    {
        output.value = true;
        yield return new WaitForSeconds(width);
        output.value = false;
    }
}
