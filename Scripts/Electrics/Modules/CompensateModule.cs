using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Electrics.Module;
using Electrics.Connectivity;
using Electrics.Utility;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CompensateModule))]
public class CompensateModuleEditor : Editor
{
    public CompensateModule Target
    {
        get
        {
            return target as CompensateModule;
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Compensate Port");
        Target.listeningPort.DoConnectionLayout(new List<BaseModule>(FindObjectsOfType<BaseModule>()));
        EditorGUILayout.Space();

        base.OnInspectorGUI();
    }
}

#endif

public class CompensateModule : BaseModule
{
    #region Settings
    [ExposedPort]
    public ElectricalInput inputPower = new ElectricalInput();

    [ExposedPort]
    public DigitalOutput outputCap1Indicator = new DigitalOutput();
    [ExposedPort]
    public DigitalOutput outputCap2Indicator = new DigitalOutput();
    [ExposedPort]
    public DigitalOutput outputCap3Indicator = new DigitalOutput();

    [HideInInspector]
    public ModulePortDescriptor listeningPort = new ModulePortDescriptor();

    [Header("Meters")]
    public TextMesh meter;

    [Header("Compensate Settings")]
    public float compensateValue = 0.001f;

    [System.Serializable]
    public class TriggerAction : UnityEvent
    {

    }

    public TriggerAction powerOnAction;
    public TriggerAction powerOffAction;
    #endregion

    #region Run-time data
    float compensateFactor = 0.99f;
    //min 0 max 7
    int compensateLevel;
    protected ElectricalOutput targetPort;
    bool hasPower;
    #endregion

    protected override void InitializeModule()
    {
        base.InitializeModule();
        targetPort = listeningPort.port as ElectricalOutput;
        StartCoroutine(CompensateRoutine());
    }

    void OnPowerOn()
    {
        if (powerOnAction != null)
            powerOnAction.Invoke();
    }

    void OnPowerOff()
    {
        if (powerOffAction != null)
            powerOffAction.Invoke();
    }

    protected override void UpdateModule()
    {
        bool p = !inputPower.voltage.IsZero();

        if(hasPower!=p)
        {
            hasPower = p;
            if(hasPower)
            {
                OnPowerOn();
            }
            else
            {
                OnPowerOff();
            }
        }

        if (!hasPower)
        {
            compensateLevel = 0;
            outputCap1Indicator.value = false;
            outputCap2Indicator.value = false;
            outputCap3Indicator.value = false;
            //Calculate input current
            inputPower.current = new Complex(0, compensateLevel * compensateValue) * inputPower.voltage;
            meter.text = "";

            return;
        }

        //Calculate input current
        inputPower.current = new Complex(0, compensateLevel * compensateValue) * inputPower.voltage;

        //Calculate compensate factor
        if (targetPort.current.IsZero())
            compensateFactor = 0.99f;
        else
            compensateFactor = Mathf.Cos((targetPort.voltage / targetPort.current).argument);

        meter.text = ReadingsFormatter.Format(compensateFactor);

        //Lit Cap Indicator
        int n = compensateLevel;
        outputCap1Indicator.value = n % 2 == 1;
        n /= 2;
        outputCap2Indicator.value = n % 2 == 1;
        n /= 2;
        outputCap3Indicator.value = n % 2 == 1;
    }

    IEnumerator CompensateRoutine()
    {
        while (true)
        {
            Compensate();
            yield return new WaitForSeconds(1.5f);
        }
    }

    void Compensate()
    {
        if (targetPort.voltage.IsZero())
        {
            compensateLevel = 0;
            return;
        }

        Complex loadCurrent = targetPort.current - inputPower.current;
        Complex project = loadCurrent.Project(targetPort.voltage);
        //Target compensate current
        Complex target = project - loadCurrent;
        if (target.IsZero())//no need to compensate
        {
            compensateLevel = 0;
        }
        else
        {
            float t = target.magnitude;
            float v = targetPort.voltage.magnitude;
            float tc = t / v;

            compensateLevel = Mathf.FloorToInt(tc / compensateValue);

            if (compensateLevel > 7)
            {
                compensateLevel = 7;
            }
        }
    }
}
