using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MidiCCControl : MonoBehaviour, IMidiInput
{
    Dictionary<byte, UnityEvent<byte>> components;

    void Start()
    {
        Init();
    }

    void Init()
    {
        components = new Dictionary<byte, UnityEvent<byte>>();
        var ccs = FindObjectsOfType<MidiCCComponent>();
        foreach (var cc in ccs)
        {
            foreach (var trigger in cc.GetTrigger())
            {
                components.TryAdd(trigger.number, trigger.Event);
            }
        }
    }

    public void OnMidiControlChange(byte channel, byte number, byte value)
    {
        if (components.TryGetValue(number, out UnityEvent<byte> ccEvent))
        {
            ccEvent?.Invoke(value);
        }
    }

    public void OnMidiNoteOn(byte channel, byte number, byte value) { }
    public void OnMidiNoteOff(byte channel, byte number) { }
}
