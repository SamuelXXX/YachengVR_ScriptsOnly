using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Electrics.Module;
using Electrics.Connectivity;
using Electrics.Utility;

public class BackupPowerModule : BaseModule
{
    [ExposedPort]
    public ElectricalInput inputPower;
    [ExposedPort]
    public ElectricalInput inputBackupPower;
    [ExposedPort]
    public ElectricalOutput outputPower;
    public Rotator rotateSwitch;

    public enum PowerType
    {
        None = 0,
        InputPower,
        BackupPower
    }

    protected PowerType powerType = PowerType.None;
    protected override void UpdateModule()
    {
        switch (powerType)//change power type
        {
            case PowerType.None:
                if (!inputPower.voltage.IsZero())
                    StartCoroutine(SwitchToInputPower(0.5f));
                break;
            case PowerType.InputPower:
                if (inputPower.voltage.IsZero())
                    StartCoroutine(SwitchToBackupPower());
                break;
            case PowerType.BackupPower:
                if (!inputPower.voltage.IsZero())
                    StartCoroutine(SwitchToInputPower(2f));
                break;
            default: break;
        }

        if (powerType == PowerType.InputPower)
        {
            ElectricalPort.Transfer(inputPower, outputPower, !switching);
        }
        else if (powerType == PowerType.BackupPower)
        {
            ElectricalPort.Transfer(inputBackupPower, outputPower, !switching);
        }

    }

    bool switching = false;
    IEnumerator SwitchToInputPower(float time)
    {
        if (!switching)
        {
            switching = true;
            rotateSwitch.Rotate("InputPower", time);
            yield return new WaitForSeconds(time);
            powerType = PowerType.InputPower;
            switching = false;
        }
    }

    IEnumerator SwitchToBackupPower()
    {
        if (!switching)
        {
            switching = true;
            rotateSwitch.Rotate("BackupPower", 2f);
            yield return new WaitForSeconds(2f);
            powerType = PowerType.BackupPower;
            switching = false;
        }
    }
}
