using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[RequireComponent(typeof(VisualEffect))]
public class VFXMidiEventBinder : MidiNoteComponent
{
    public string EventName = "Event";
    public string EventNameOff = "Event";
    [SerializeField, HideInInspector]
    VFXEventAttribute eventAttribute;

    VisualEffect target;
    private ExposedProperty valueProperty = "value";

    [SerializeField]
    public LaunchPadOutColor color = LaunchPadOutColor.Yellow;

    [SerializeField]
    public MidiBehaviorType type = MidiBehaviorType.Button;

    [HideInInspector]
    public bool bToggle = false;

    void Start()
    {
        target = GetComponent<VisualEffect>();
        eventAttribute = target.CreateVFXEventAttribute();
    }

    public override void SetValue(byte value = 0)
    {
        if (target != null && value > 63)
        {
            switch (type)
            {
                case MidiBehaviorType.Button:
                    if (eventAttribute.HasFloat(valueProperty)) eventAttribute.SetFloat(valueProperty, value / 127.0f);
                    target.SendEvent(EventName, eventAttribute);
                    break;
                case MidiBehaviorType.Toggle:
                    if (!bToggle)
                    {
                        if (eventAttribute.HasFloat(valueProperty)) eventAttribute.SetFloat(valueProperty, value / 127.0f);
                        target.SendEvent(EventName, eventAttribute);
                        bToggle = true;
                    }
                    else
                    {
                        target.SendEvent(EventNameOff, eventAttribute);
                        bToggle = false;
                    }
                    break;
            }
        }
    }
}

public enum MidiBehaviorType
{
    Button, Toggle
}

