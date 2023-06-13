using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[RequireComponent(typeof(VisualEffect))]
public class KickTriggerMidiEvent : MidiNoteComponent
{
    public string EventName = "Event";
    [SerializeField, HideInInspector]
    VFXEventAttribute eventAttribute;

    VisualEffect target;
    private ExposedProperty valueProperty = "value";

    [SerializeField]
    public LaunchPadOutColor color = LaunchPadOutColor.Yellow;

    [SerializeField]
    KickTrigger trigger;

    public bool bToggle = false;

    void Start()
    {
        target = GetComponent<VisualEffect>();
        eventAttribute = target.CreateVFXEventAttribute();

        trigger.onKickOn += OnKickOn;
        trigger.onKickOff += OnKickOff;
    }

    public override void SetValue(byte value = 0)
    {
        if (value < 63) return;
        bToggle = !bToggle;
    }

    private void OnKickOff(float audioLevel)
    {
    }

    private void OnKickOn(float audioLevel)
    {
        if (bToggle)
        {
            if (eventAttribute.HasFloat(valueProperty)) eventAttribute.SetFloat(valueProperty, audioLevel);
            target.SendEvent(EventName, eventAttribute);
        }
    }
}
