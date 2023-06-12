using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MidiVFX : MonoBehaviour, IMidiInput
{
    [SerializeField]
    private MidiVFXComponent[] component;

    [SerializeField]
    private MidiVFXToggleComponent[] toggleComponents;

    MidiOutput midiOutput;

    void Start()
    {
        midiOutput = FindObjectOfType<MidiOutput>();

        if (midiOutput == null) return;
        midiOutput.OnOpen += OnOpen;
        midiOutput.OnClose += OnClose;
    }

    private void OnOpen()
    {
        foreach (var t in component)
        {
            midiOutput.SendNoteOn(0, t.note, (int)LaunchPadOutColor.Red, MidiOutSetting.Launchpad);
        }

        foreach (var t in toggleComponents)
        {
            midiOutput.SendNoteOn(0, t.note, (int)LaunchPadOutColor.Red, MidiOutSetting.Launchpad);
        }
    }

    private void OnClose()
    {
        foreach (var t in component)
        {
            midiOutput.SendNoteOff(0, t.note, MidiOutSetting.Launchpad);
        }

        foreach (var t in toggleComponents)
        {
            midiOutput.SendNoteOff(0, t.note, MidiOutSetting.Launchpad);
        }
        midiOutput.OnOpen -= OnOpen;
        midiOutput.OnClose -= OnClose;
    }

    public void OnMidiControlChange(byte channel, byte number, byte value)
    {
    }

    public void OnMidiNoteOff(byte channel, byte note)
    {

    }

    public void OnMidiNoteOn(byte channel, byte note, byte velocity)
    {
        foreach (var c in component)
        {
            if (c.note == note)
            {
                c.vfx.SendEvent(c.eventName);
                return;
            }
        }

        foreach (var c in toggleComponents)
        {
            if (c.note == note)
            {
                c.ToggleEvent();
                return;
            }
        }
    }
}

[System.Serializable]
public class MidiVFXComponent : MidiNote
{
    public VisualEffect vfx;
    public string eventName;
}

[System.Serializable]
public class MidiVFXToggleComponent : MidiNote
{
    public VisualEffect vfx;
    public string playEventName;
    public string stopEventName;
    [HideInInspector]
    public bool bToggle = false;

    public void ToggleEvent()
    {
        if (!bToggle)
        {
            vfx.SendEvent(playEventName);
            bToggle = true;
        }
        else
        {
            vfx.SendEvent(stopEventName);
            bToggle = false;
        }
    }
}
