using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CCIndexAttribute))]
public class CCIndexDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var prop = property.FindPropertyRelative("number");
        EditorGUI.PropertyField(position, property, new GUIContent($"CC {prop.intValue}"), true);
    }

    public override float GetPropertyHeight(SerializedProperty i_property, GUIContent i_label)
    {
        return EditorGUI.GetPropertyHeight(i_property);
    }
}
