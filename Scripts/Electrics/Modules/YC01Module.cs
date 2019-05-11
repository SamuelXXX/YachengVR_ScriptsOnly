using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Electrics.Module;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(YC01Module))]
public class YC01ModuleEditor : CompositeModuleEditor<YC01Module>
{

}

#endif

public class YC01Module : CompositeModule
{
}
