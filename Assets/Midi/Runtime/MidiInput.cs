using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RtMidi.LowLevel;

[DefaultExecutionOrder(-1)]
public class MidiInput : MonoBehaviour
{
    MidiProbe _probe;
    List<MidiInPort> _ports = new List<MidiInPort>();

    List<IMidiInput> _inputs = new List<IMidiInput>();

    [SerializeField]
    private string[] inputDeviceNames;

    private int currentDiveceCount;
    private int currentAllDeviceCount;

    [SerializeField]
    bool isDebug;

    bool IsRealPort(string name)
    {
        return !name.Contains("Through") && !name.Contains("RtMidi");
    }

    void ScanPorts()
    {
        for (var i = 0; i < _probe.PortCount; i++)
        {
            var name = _probe.GetPortName(i);

            if (inputDeviceNames.Length > 0)
            {
                for (var j = 0; j < inputDeviceNames.Length; j++)
                {
                    if (name.IndexOf(inputDeviceNames[j]) != -1)
                    {

                        AddPort(name, i);
                    }
                }
            }
            else
            {
                AddPort(name, i);
            }
        }

        currentDiveceCount = _ports.Count;
        currentAllDeviceCount = _probe.PortCount;
    }

    private void AddPort(string name, int i)
    {
        Debug.Log("Midi-in port found: " + name);
        _ports.Add(IsRealPort(name) ? new MidiInPort(i)
        {
            OnNoteOn = OnMidiNoteOn,

            OnNoteOff = OnMidiNoteOff,

            OnControlChange = OnControlChange,
        } : null);
    }

    void DisposePorts()
    {
        foreach (var p in _ports) p?.Dispose();
        _ports.Clear();
    }

    void GetAllMidiInput()
    {
        foreach (var m in GameObject.FindObjectsOfType<Component>())
        {
            var c = m as IMidiInput;
            if (c != null) _inputs.Add(c);
        }
    }

    void Awake()
    {
        _probe = new MidiProbe(MidiProbe.Mode.In);
        GetAllMidiInput();
    }

    void Update()
    {
        if (_ports.Count != currentDiveceCount || currentAllDeviceCount != _probe.PortCount)
        {
            DisposePorts();
            ScanPorts();
        }

        foreach (var p in _ports) p?.ProcessMessages();
    }

    void OnDestroy()
    {
        _probe?.Dispose();
        DisposePorts();
        _inputs?.Clear();
    }

    void OnMidiNoteOn(byte channel, byte note, byte velocity)
    {
        foreach (var i in _inputs)
        {
            i.OnMidiNoteOn(channel, note, velocity);
        }
        if (isDebug) Debug.Log($"Channel : {channel} Note : {note} Velocity : {velocity}");
    }

    void OnMidiNoteOff(byte channel, byte note)
    {
        foreach (var i in _inputs)
        {
            i.OnMidiNoteOff(channel, note);
        }
        if (isDebug) Debug.Log($"Channel : {channel} Note : {note} Velocity : {0}");
    }

    void OnControlChange(byte channel, byte number, byte value)
    {
        foreach (var i in _inputs)
        {
            i.OnMidiControlChange(channel, number, value);
        }
        if (isDebug) Debug.Log($"Channel : {channel} Number : {number} Value : {value}");
    }
}

