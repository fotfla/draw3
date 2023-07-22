using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidiCCControl : MonoBehaviour, IMidiInput
{
    Dictionary<byte, MidiCCComponent> components;

    void Start()
    {
        Init();
    }

    void Init()
    {
        components = new Dictionary<byte, MidiCCComponent>();
        var ccs = FindObjectsOfType<MidiCCComponent>();
        foreach (var cc in ccs)
        {
            components.TryAdd(cc.GetCCNumber(), cc);
        }
    }

    public void OnMidiControlChange(byte channel, byte number, byte value)
    {
        Debug.Log("CC");
        if (components.TryGetValue(number, out MidiCCComponent c))
        {
            c.Event?.Invoke(value);
        }
    }

    public void OnMidiNoteOn(byte channel, byte number, byte value) { }
    public void OnMidiNoteOff(byte channel, byte number) { }
}
