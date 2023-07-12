using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;
using UnityEngine.Events;
using System;

public class MidiVFXManager : MonoBehaviour, IMidiInput
{
    Dictionary<byte, MidiCCPropertyBinder> binders;
    Dictionary<byte, VFXMidiEventBinder> components;

    MidiOutput midiOutput;
    void Start()
    {
        var ccvfxs = GameObject.FindObjectsOfType<VFXPropertyBinder>();
        binders = new Dictionary<byte, MidiCCPropertyBinder>();
        foreach (var v in ccvfxs)
        {
            var binder = v.GetPropertyBinders<MidiCCPropertyBinder>();
            foreach (var b in binder)
            {
                binders.Add(b.CCNumber, b);
            }
        }

        var eventvfx = GameObject.FindObjectsOfType<VFXMidiEventBinder>();
        components = new Dictionary<byte, VFXMidiEventBinder>();
        foreach (var c in eventvfx)
        {
            components.Add(c.note, c);
        }

        midiOutput = FindObjectOfType<MidiOutput>();
        if (midiOutput != null)
        {
            midiOutput.OnOpen += OnOpen;
            midiOutput.OnClose += OnClose;
        }
    }

    private void OnOpen()
    {
        foreach (var c in components)
        {
            midiOutput.SendNoteOn(0, c.Key, (int)c.Value.color, MidiOutSetting.Launchpad);
        }
    }

    private void OnClose()
    {
        foreach (var c in components)
        {
            midiOutput.SendNoteOff(0, c.Key, MidiOutSetting.Launchpad);
        }

        midiOutput.OnOpen -= OnOpen;
        midiOutput.OnClose -= OnClose;
    }

    public void OnMidiControlChange(byte channel, byte number, byte value)
    {
        if (binders.TryGetValue(number, out MidiCCPropertyBinder binder))
        {
            binder.SetValue(value);
        }
    }

    public void OnMidiNoteOff(byte channel, byte note)
    {
        if (components.TryGetValue(note, out VFXMidiEventBinder binder))
        {
            binder.SetValue();
        }
    }

    public void OnMidiNoteOn(byte channel, byte note, byte velocity)
    {
        if (components.TryGetValue(note, out VFXMidiEventBinder binder))
        {
            binder.SetValue(velocity);
            if (binder.type == MidiBehaviorType.Toggle)
            {
                midiOutput.SendNoteOn(0, note, !binder.bToggle ? (int)binder.color : (int)LaunchPadOutColor.Red, MidiOutSetting.Launchpad);
            }
        }
    }
}
