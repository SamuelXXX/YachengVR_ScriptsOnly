using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Electrics.Module;
using Electrics.Utility;
using System.Linq;
using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Electrics.Connectivity
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ExposedPortAttribute : Attribute
    {
        public string portName;
        public ExposedPortAttribute(string name)
        {
            portName = name;
        }

        public ExposedPortAttribute()
        {
            portName = null;
        }
    }

    public abstract class InoutPort
    {
        [HideInInspector]
        public string name;
        [HideInInspector]
        public uint portId;
    }

    public abstract class DigitalPort : InoutPort
    {
        public bool value;
    }

    public abstract class ElectricalPort : InoutPort
    {
        public ElectricalWireProperty wireProperty = new ElectricalWireProperty();

        #region Properties
        public Complex voltage
        {
            get
            {
                return wireProperty.voltage;
            }
            set
            {
                wireProperty.voltage = value;
            }
        }

        public float frequency
        {
            get
            {
                return wireProperty.frequency;
            }
            set
            {
                wireProperty.frequency = value;
            }
        }

        public float maxPower
        {
            get
            {
                return wireProperty.maxPower;
            }
            set
            {
                wireProperty.maxPower = value;
            }
        }



        public Complex current
        {
            get
            {
                return wireProperty.current;
            }
            set
            {
                wireProperty.current = value;
            }
        }
        #endregion

        public static void Transfer(ElectricalPort source, ElectricalPort target, bool connected = true)
        {
            if (source == null || target == null)
                return;

            ElectricalWireProperty.Transfer(source.wireProperty, target.wireProperty, connected);
        }
    }


    [System.Serializable]
    public class DigitalInput : DigitalPort
    {

    }

    [System.Serializable]
    public class DigitalOutput : DigitalPort
    {
    }

    [System.Serializable]
    public class ElectricalWireProperty
    {
        //Basic Information
        public float frequency;

        //Forward Information
        public Complex voltage;

        public float maxPower;

        //Backward Information
        public Complex current;

        public void PowerOff()
        {
            frequency = 0f;

            voltage.SetZero();
            current.SetZero();

            maxPower = 0f;
        }

        public static void Transfer(ElectricalWireProperty source, ElectricalWireProperty target, bool connected = true)
        {
            if (source == null || target == null)
                return;

            if (connected)
            {
                target.voltage = source.voltage;
                target.frequency = source.frequency;
                target.maxPower = source.maxPower;

                source.current = target.current;
            }
            else
            {
                target.voltage.magnitude = 0;
                target.frequency = 0;
                target.maxPower = 0;

                source.current.magnitude = 0;
            }

        }

        public static void Transfer(ElectricalWireProperty source, List<ElectricalWireProperty> targets, bool connected = true)
        {
            if (source == null || targets == null)
                return;

            source.current.magnitude = 0;


            if (connected)
            {
                foreach (var target in targets)
                {
                    target.voltage = source.voltage;
                    target.frequency = source.frequency;
                    target.maxPower = source.maxPower;

                    source.current += target.current;
                }
            }
            else
            {
                foreach (var target in targets)
                {
                    target.voltage.magnitude = 0;
                    target.frequency = 0;
                    target.maxPower = 0;
                }
            }

        }
    }

    [System.Serializable]
    public class ElectricalInput : ElectricalPort
    {
    }

    [System.Serializable]
    public class ElectricalOutput : ElectricalPort
    {
    }

    [System.Serializable]
    public class ModulePortDescriptor
    {
        public Electrics.Module.BaseModule module;
        [SerializeField]
        protected uint portId;//Use id to reference module's port to avoid reference missing

        public InoutPort port
        {
            get
            {
                if (module == null)
                    return null;

                return module.GetPortById<InoutPort>(portId);
            }
            set
            {
                portId = value.portId;
            }
        }

#if UNITY_EDITOR
        public bool DoConnectionLayout(List<BaseModule> modules)
        {
            var preModule = module;
            var prePort = port;
            //Module selection
            if (modules == null || modules.Count == 0)
            {
                EditorGUILayout.LabelField("Modules are empty");
                return true;
            }


            if (module == null)
            {
                module = modules[0];
            }

            if (!modules.Exists(p => p == module))
            {
                module = modules[0];
            }

            List<string> names = new List<string>(from m in modules select m.moduleName);
            List<int> indexs = new List<int>();
            for (int i = 0; i < modules.Count; i++)
            {
                indexs.Add(i);
            }

            int selected = modules.FindIndex(p => p == module);
            selected = EditorGUILayout.IntPopup(selected, names.ToArray(), indexs.ToArray());
            module = modules[selected];


            //Ports selection
            List<InoutPort> ports = module.GetPortsByType<InoutPort>();
            if (ports == null || ports.Count == 0)
            {
                return true;
            }

            if (port == null)
            {
                port = ports[0];
            }

            if (!ports.Exists(p => p == port))
            {
                port = ports[0];
            }

            indexs.Clear();
            names = new List<string>(from p in ports select p.name);
            for (int i = 0; i < ports.Count; i++)
            {
                indexs.Add(i);
            }
            selected = ports.FindIndex(p => p == port);
            selected = EditorGUILayout.IntPopup(selected, names.ToArray(), indexs.ToArray());
            port = ports[selected];

            //Value changed
            if (module != preModule || port != prePort)
            {
                return true;
            }

            return false;
        }
#endif
    }

    [System.Serializable]
    public class Wire
    {
        #region Definations
        public enum WireType
        {
            None = 0,//No Connection
            Illegal,//Multi-input or digtial-electrical mixed
            Digital,//Pure digital
            Electrical//Pure electrical
        }
        #endregion

        #region Basic Settings
        public string wireName;
        [SerializeField]
        protected uint wireId;

        public uint WireID
        {
            get
            {
                return wireId;
            }
        }

        #endregion

        #region Run-time data
        public List<ModulePortDescriptor> connectedModulePorts = new List<ModulePortDescriptor>();
        [SerializeField]
        protected WireType type = WireType.None;//Check if this wire is valid
        public WireType Type
        {
            get
            {
                return type;
            }
        }

        protected List<DigitalInput> digitalInputs = new List<DigitalInput>();
        protected List<DigitalOutput> digitalOutputs = new List<DigitalOutput>();
        protected List<ElectricalInput> electricalInputs = new List<ElectricalInput>();
        protected List<ElectricalOutput> electricalOutputs = new List<ElectricalOutput>();
        #endregion

        #region Data Validation
        public void AcquirePortInstances()
        {
            digitalInputs = GetPortsByType<DigitalInput>();
            digitalOutputs = GetPortsByType<DigitalOutput>();
            electricalInputs = GetPortsByType<ElectricalInput>();
            electricalOutputs = GetPortsByType<ElectricalOutput>();
        }

        public void Validate()//Only run in Editor
        {
            if (Application.isPlaying)
            {
                Debug.Log("Cannot validate wire data while playing");
                return;
            }

            AcquirePortInstances();

            if (connectedModulePorts.Count == 0)
            {
                type = WireType.None;
                return;
            }

            if (electricalOutputs.Count + digitalOutputs.Count != 1)//No input or multi-input for this wire
            {
                type = WireType.Illegal;
                Debug.LogError("No input or multi-input found on wire " + wireName);
                return;
            }

            bool isElectricalInput = electricalOutputs.Count == 1;
            bool hasDigitalOutput = digitalInputs.Count != 0;

            if (isElectricalInput && hasDigitalOutput)
            {
                type = WireType.Illegal;
                Debug.LogError("Eletrical input drive digital input on wire " + wireName);
                return;
            }

            if (isElectricalInput)
            {
                type = WireType.Electrical;
            }
            else
            {
                type = WireType.Digital;
            }
        }
        #endregion

        #region  Life Circle
        public Wire(string name)
        {
            wireName = name;
            wireId = BaseModule.GenerateGuid();
        }

        public void Initialize()
        {
            AcquirePortInstances();
        }

        public void Update()
        {
            switch (type)
            {
                case WireType.None:
                case WireType.Illegal: break;
                case WireType.Digital:
                    DigitalOutput dout = digitalOutputs[0];
                    foreach (var p in digitalInputs)
                    {
                        p.value = dout.value;
                    }
                    break;
                case WireType.Electrical:
                    ElectricalOutput eout = electricalOutputs[0];
                    ElectricalWireProperty.Transfer(eout.wireProperty, new List<ElectricalWireProperty>(from i in electricalInputs select i.wireProperty));
                    break;
                default: break;
            }
        }
        #endregion

        #region Port Information Getter
        /// <summary>
        /// This function must be fulfiled to make port information easy to get
        /// </summary>
        /// <returns></returns>
        protected List<InoutPort> GetAllInoutPorts()
        {
            return new List<InoutPort>(from p in connectedModulePorts select p.port);
        }

        public List<T> GetPortsByType<T>() where T : InoutPort
        {
            List<InoutPort> inoutPorts = GetAllInoutPorts();
            List<T> TPorts = new List<T>();
            if (inoutPorts != null)
            {
                foreach (var p in inoutPorts)
                {
                    if (p is T)
                    {
                        TPorts.Add(p as T);
                    }
                }
            }
            return TPorts;
        }

        public T GetPortsById<T>(uint portId) where T : InoutPort
        {
            List<T> ports = GetPortsByType<T>();
            return ports.Find(p => p.portId == portId);
        }

        public T GetPortsByName<T>(string name) where T : InoutPort
        {
            List<T> ports = GetPortsByType<T>();
            return ports.Find(p => p.name == name);
        }
        #endregion

        #region Editor Layout
#if UNITY_EDITOR
        public bool editorFolded = true;
        /// <summary>
        /// Do a editor layout of wire
        /// </summary>
        /// <returns>Should Remove or Not,if true ,should remove</returns>
        public bool DoWireEditingLayout(List<BaseModule> modules)
        {
            EditorGUILayout.BeginHorizontal();
            bool collapseButtonClicked = GUILayout.Button(editorFolded ? "+" : "-", GUILayout.Height(15f), GUILayout.Width(15f));

            if (collapseButtonClicked)
            {
                editorFolded = !editorFolded;
            }

            EditorGUILayout.LabelField(wireName + ":" + Type.ToString() + "----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            if (!editorFolded)
            {
                if (GUILayout.Button("del", GUILayout.Height(15f), GUILayout.Width(48f)))
                {
                    return true;
                }
            }
            EditorGUILayout.EndHorizontal();

            if (!editorFolded)
            {
                wireName = EditorGUILayout.TextField("Wire Name", wireName);
                EditorGUILayout.LabelField("WireId:" + wireId.ToString());

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Connected Ports");
                List<ModulePortDescriptor> removeConnections = new List<ModulePortDescriptor>();
                foreach (var c in connectedModulePorts)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (c.DoConnectionLayout(modules))
                    {
                        Validate();
                    }
                    if (GUILayout.Button("delete", GUILayout.Height(15f)))
                    {
                        removeConnections.Add(c);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Add", GUILayout.Height(15f)))
                {
                    connectedModulePorts.Add(new ModulePortDescriptor());
                    Validate();
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                foreach (var r in removeConnections)
                {
                    connectedModulePorts.Remove(r);
                }
            }

            return false;
        }
#endif
        #endregion
    }

    [System.Serializable]
    public class InoutMapper
    {
        #region Definations
        public enum MappingType
        {
            None = 0,//No mapping
            Mismatch,//Match wrong
            DigitalInput,//Pure digital input
            DigitalOutput,
            ElectricalInput,//Pure electrical input
            ElectricalOutput
        }
        #endregion

        #region Settings
        public string mapName;
        public BaseModule sourceModule;
        [SerializeField]
        protected uint sourceId;
        public InoutPort source
        {
            get
            {
                if (sourceModule == null)
                    return null;

                return sourceModule.GetPortById<InoutPort>(sourceId);
            }
            set
            {
                sourceId = value.portId;
            }
        }

        public ModulePortDescriptor target;
        #endregion

        #region Validating
        [SerializeField]
        protected MappingType type = MappingType.None;
        public MappingType Type
        {
            get
            {
                return type;
            }
        }

        public void Validate()
        {
            if (Application.isPlaying)
            {
                Debug.Log("Cannot validate mapping data while playing");
                return;
            }

            if (source == null || target == null || target.port == null)
            {
                type = MappingType.None;
                return;
            }

            if (source.GetType() != target.port.GetType())
            {
                type = MappingType.Mismatch;
                return;
            }

            if (source is DigitalInput)
            {
                type = MappingType.DigitalInput;
                return;
            }

            if (source is DigitalOutput)
            {
                type = MappingType.DigitalOutput;
                return;
            }

            if (source is ElectricalInput)
            {
                type = MappingType.ElectricalInput;
                return;
            }

            if (source is ElectricalOutput)
            {
                type = MappingType.ElectricalOutput;
                return;
            }
        }
        #endregion

        #region Life Circle
        public InoutMapper(BaseModule sourceModule, InoutPort source)
        {
            this.sourceModule = sourceModule;
            this.source = source;
            this.mapName = this.source.name;
        }

        public void Initialize()
        {
        }

        public void Update()
        {
            switch (type)
            {
                case MappingType.None:
                case MappingType.Mismatch: break;
                case MappingType.DigitalInput:
                    (target.port as DigitalInput).value = (source as DigitalInput).value;
                    break;
                case MappingType.DigitalOutput:
                    (source as DigitalOutput).value = (target.port as DigitalOutput).value;
                    break;
                case MappingType.ElectricalInput:
                    ElectricalInput.Transfer(source as ElectricalInput, target.port as ElectricalInput);
                    break;
                case MappingType.ElectricalOutput:
                    ElectricalOutput.Transfer(target.port as ElectricalOutput, source as ElectricalOutput);
                    break;
                default: break;
            }
        }
        #endregion

        #region Editor Layout
#if UNITY_EDITOR
        public bool editorFolded = true;
        /// <summary>
        /// Do a editor layout of wire
        /// </summary>
        /// <returns>Should Remove or Not,if true ,should remove</returns>
        public void DoInoutMapperEditingLayout(List<BaseModule> modules)
        {
            EditorGUILayout.BeginHorizontal();
            bool collapseButtonClicked = GUILayout.Button(editorFolded ? "+" : "-", GUILayout.Height(15f), GUILayout.Width(15f));

            if (collapseButtonClicked)
            {
                editorFolded = !editorFolded;
            }

            EditorGUILayout.LabelField(mapName + ":" + Type.ToString());
            EditorGUILayout.EndHorizontal();

            InoutPort prevPort = target.port;
            if (!editorFolded)
            {
                target.DoConnectionLayout(modules);
                if (prevPort != target.port)
                {
                    Validate();
                }
            }
        }
#endif
        #endregion
    }
}


