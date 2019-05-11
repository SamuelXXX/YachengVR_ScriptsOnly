using UnityEditor;
using Electrics.Connectivity;
using UnityEngine;
using Electrics.Utility;

[CustomPropertyDrawer(typeof(DigitalInput))]
public class DigitalInputDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("DI:" + label.text));

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var idRect = new Rect(position.x, position.y, 100, position.height);
        var valueRect = new Rect(position.x + 100, position.y, 50, position.height);

        EditorGUI.LabelField(idRect, "id:" + property.FindPropertyRelative("portId").longValue);
        EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(DigitalOutput))]
public class DigitalOutputDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("DO:" + label.text));



        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var idRect = new Rect(position.x, position.y, 100, position.height);
        var valueRect = new Rect(position.x + 100, position.y, 50, position.height);

        EditorGUI.LabelField(idRect, "id:" + property.FindPropertyRelative("portId").longValue);
        EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}



