using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VFXMidiEventBinder))]
public class VFXMidiEventBinderInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var vfx = target as VFXMidiEventBinder;

        if (GUILayout.Button("Event Invoke"))
        {
            vfx.SetValue(127);
        }
    }
}
