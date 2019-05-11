using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Electrics.Connectivity;
using System;
using System.Reflection;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Electrics.Module
{
    public abstract class BaseModule : MonoBehaviour
    {
        public string moduleName;
        [HideInInspector]
        public uint moduleId;

        #region MonoBehaviour Life Circle
        // Use this for initialization
        protected void Start()
        {
            InitializeModule();
        }

        // Update is called once per frame
        protected void Update()
        {
            UpdateModule();
        }

        protected void OnValidate()
        {
            ModuleDefination();
        }

        protected void Reset()
        {
            ModuleDefination();
        }
        #endregion

        #region Module Life Circle(Overridable)
        protected virtual void InitializeModule()
        {

        }

        protected virtual void UpdateModule()
        {

        }
        #endregion

        #region Module Info Automation
        [ContextMenu("Module Defination")]
        /// <summary>
        /// Give module a defination
        /// </summary>
        protected void ModuleDefination()
        {
            if (string.IsNullOrEmpty(moduleName))
                moduleName = GetType().Name;
            if (moduleId == 0)
            {
                moduleId = GenerateGuid();
            }

            FieldInfo[] declaredFields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var f in declaredFields)
            {
                //Processing InoutPort Members
                if (f.IsDefined(typeof(ExposedPortAttribute), true) && f.FieldType.IsSubclassOf(typeof(InoutPort)))
                {
                    foreach (var c in f.GetCustomAttributes(true))
                    {
                        if (typeof(ExposedPortAttribute).IsInstanceOfType(c))
                        {
                            var a = c as ExposedPortAttribute;
                            if (!string.IsNullOrEmpty(a.portName))
                            {
                                ((InoutPort)f.GetValue(this)).name = a.portName;
                            }
                            break;
                        }
                    }
                    if (string.IsNullOrEmpty(((InoutPort)f.GetValue(this)).name))//Use parameters name
                    {
                        ((InoutPort)f.GetValue(this)).name = f.Name;
                    }

                    if (((InoutPort)f.GetValue(this)).portId == 0)
                    {
                        ((InoutPort)f.GetValue(this)).portId = GenerateGuid();
                    }
                }
            }
            DefineModule();
        }

        protected virtual void DefineModule()
        {

        }
        #endregion

        #region Port Information Getter
        /// <summary>
        /// This function must be fulfiled to make port information easy to get,only called in Editor,So fulfiled by Reflection
        /// </summary>
        /// <returns></returns>
        protected List<InoutPort> GetAllInoutPorts()
        {
            List<InoutPort> ports = new List<InoutPort>();
            FieldInfo[] declaredFields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var f in declaredFields)
            {
                if (f.IsDefined(typeof(ExposedPortAttribute), true) && f.FieldType.IsSubclassOf(typeof(InoutPort)))
                {
                    InoutPort ins = (InoutPort)f.GetValue(this);
                    ports.Add(ins);
                }
            }
            return ports;
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

        public T GetPortById<T>(uint portId) where T : InoutPort
        {
            List<T> ports = GetPortsByType<T>();
            return ports.Find(p => p.portId == portId);
        }

        public T GetPortByName<T>(string name) where T : InoutPort
        {
            List<T> ports = GetPortsByType<T>();
            return ports.Find(p => p.name == name);
        }
        #endregion

        #region Tool Methods
        public static uint GenerateGuid()
        {
            return BitConverter.ToUInt32(Guid.NewGuid().ToByteArray(), 0);
        }
        #endregion
    }

    public abstract class CompositeModule : BaseModule
    {
        #region Structure Infomation
        public List<BaseModule> subModules = new List<BaseModule>();
        public List<Wire> connectionWires = new List<Wire>();
        public List<InoutMapper> inoutMappers = new List<InoutMapper>();
        #endregion

        #region Tool Methods
        public List<string> GetAllModuleNames()
        {
            return new List<string>(from m in subModules select m.moduleName);
        }

        public static CompositeModule FindOwner(BaseModule module)
        {
            if (module == null)
                return null;

            if (module is CompositeModule)
            {
                return module.transform.parent.GetComponentInParent<CompositeModule>();
            }
            else
            {
                return module.GetComponentInParent<CompositeModule>();
            }
        }
        #endregion

        #region Life Circle
        protected override void InitializeModule()
        {
            foreach (var m in inoutMappers)
            {
                m.Initialize();
            }

            foreach (var w in connectionWires)
            {
                w.Initialize();
            }
        }

        protected override void UpdateModule()
        {
            foreach (var m in inoutMappers)
            {
                m.Update();
            }

            foreach (var w in connectionWires)
            {
                w.Update();
            }
        }

        protected sealed override void DefineModule()
        {
            //Refresh all sub modules
            subModules.Clear();

            foreach (var c in GetComponentsInChildren<BaseModule>())
            {
                if (c == this)
                {
                    continue;
                }
                if (FindOwner(c) == this)//Direct owned by this module
                {
                    subModules.Add(c);
                }
            }

            //Validate connectivities
            foreach (var w in connectionWires)
            {
                w.connectedModulePorts.RemoveAll(p => p.module == null);
            }

            List<InoutPort> inoutPorts = GetAllInoutPorts();

            inoutMappers.RemoveAll(p => !inoutPorts.Contains(p.source));//Remove all invalid IO mappers 

            foreach (var i in inoutPorts)
            {
                if (!inoutMappers.Exists(p => p.source == i))
                    inoutMappers.Add(new InoutMapper(this, i));
            }
        }
        #endregion
    }

#if UNITY_EDITOR
    public class CompositeModuleEditor<T> : Editor where T : CompositeModule
    {
        public T Target
        {
            get
            {
                return target as T;
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            Target.moduleName = EditorGUILayout.TextField("Module Name", Target.moduleName);
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("IO Ports");
            foreach (var i in Target.GetPortsByType<DigitalInput>())
            {
                EditorGUILayout.LabelField("DI:" + i.name);
            }

            foreach (var i in Target.GetPortsByType<DigitalOutput>())
            {
                EditorGUILayout.LabelField("DO:" + i.name);
            }

            foreach (var i in Target.GetPortsByType<ElectricalInput>())
            {
                EditorGUILayout.LabelField("EI:" + i.name);
            }

            foreach (var i in Target.GetPortsByType<ElectricalOutput>())
            {
                EditorGUILayout.LabelField("EO:" + i.name);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();


            EditorGUILayout.LabelField("Sub Modules");
            foreach (var m in Target.subModules)
            {
                EditorGUILayout.LabelField("Module:" + m.moduleName);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Internal Connectivities");
            List<Wire> removeWires = new List<Wire>();
            List<BaseModule> modules = new List<BaseModule>();
            modules.AddRange(Target.subModules);

            foreach (var w in Target.connectionWires)
            {
                if (w.DoWireEditingLayout(modules))
                {
                    removeWires.Add(w);
                }
            }

            if (GUILayout.Button("Add Wire"))
            {
                Wire w = new Wire("EmptyWireName");
                Target.connectionWires.Add(w);
            }

            foreach (var r in removeWires)
            {
                Target.connectionWires.Remove(r);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Inout Mapping");
            foreach (var m in Target.inoutMappers)
            {
                m.DoInoutMapperEditingLayout(modules);
            }

            if (GUI.changed)
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
#endif
}


