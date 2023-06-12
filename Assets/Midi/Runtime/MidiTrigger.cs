using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public class MidiTrigger : MonoBehaviour, IMidiInput
{
    [SerializeField, NoteIndex]
    MidiInputNoteTrigger[] noteTriggers;
    [SerializeField, CCIndex]
    MidiInputCCTrigger[] ccTriggers;

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
        foreach (var t in noteTriggers)
        {
            midiOutput.SendNoteOn(0, t.note, (int)t.color, MidiOutSetting.Launchpad);
        }
    }

    private void OnClose()
    {
        foreach (var t in noteTriggers)
        {
            midiOutput.SendNoteOff(0, t.note, MidiOutSetting.Launchpad);
        }
        midiOutput.OnOpen -= OnOpen;
        midiOutput.OnClose -= OnClose;
    }

    public void OnMidiControlChange(byte channel, byte number, byte value)
    {
        foreach (var t in ccTriggers)
        {
            if (t.number == number) t.Event?.Invoke(value);
        }
    }

    public void OnMidiNoteOff(byte channel, byte note)
    {
        foreach (var t in noteTriggers)
        {
            if (t.note == note) t.Event?.Invoke(0);
        }
    }

    public void OnMidiNoteOn(byte channel, byte note, byte velocity)
    {
        foreach (var t in noteTriggers)
        {
            if (t.note == note) t.Event?.Invoke(velocity);
        }
    }
}

[System.Serializable]
public class MidiInputNoteTrigger : MidiNote
{
    public LaunchPadOutColor color = LaunchPadOutColor.Yellow;
    public UnityEvent<byte> Event;

    MidiInputNoteTrigger()
    {
        color = LaunchPadOutColor.Yellow;
    }
}

[System.Serializable]
public class MidiInputCCTrigger : MidiCC
{
    public UnityEvent<byte> Event;
}
