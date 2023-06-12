using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RtMidi.LowLevel;
using UnityEngine.Events;

[DefaultExecutionOrder(-10)]
public class MidiOutput : MonoBehaviour
{
    MidiProbe _probe;
    List<MidiOutPort> _ports = new List<MidiOutPort>();

    public string[] outputDeviceName;

    private int currentDiveceCount;
    private int currentAllDeviceCount;

    public Action OnOpen;
    public Action OnClose;

    bool IsRealPort(string name)
    {
        return !name.Contains("Through") && !name.Contains("RtMidi");
    }

    void ScanPorts()
    {
        for (var i = 0; i < _probe.PortCount; i++)
        {
            var name = _probe.GetPortName(i);

            if (outputDeviceName.Length > 0)
            {
                foreach (var s in outputDeviceName)
                {
                    if (name.IndexOf(s) != -1) AddPort(name, i);
                }
            }
            else
            {
                AddPort(name, i);
            }
        }

        currentAllDeviceCount = _probe.PortCount;
        currentDiveceCount = _ports.Count;
    }

    void AddPort(string name, int i)
    {
        Debug.Log("Midi-out port found: " + name);
        _ports.Add(IsRealPort(name) ? new MidiOutPort(i, name) : null);
    }

    void DisposePorts()
    {
        foreach (var p in _ports) p?.Dispose();
        _ports.Clear();
    }

    void Start()
    {
        _probe = new MidiProbe(MidiProbe.Mode.Out);
    }

    void Update()
    {
        // Rescan when the number of ports changed.
        if (_ports.Count != currentDiveceCount || _probe.PortCount != currentAllDeviceCount)
        {
            DisposePorts();
            ScanPorts();
            OnOpen?.Invoke();
        }
    }

    void OnDestroy()
    {
        OnClose?.Invoke();
        _probe?.Dispose();
        DisposePorts();
    }

    public void SendNoteOn(int channel, int note, int velocity)
    {
        foreach (var p in _ports)
        {
            p?.SendNoteOn(channel, note, velocity);
        }
    }

    public void SendNoteOn(int channel, int note, int velocity, string deviceName)
    {
        foreach (var p in _ports)
        {
            if (p.GetPortName().IndexOf(deviceName) != -1)
            {
                p?.SendNoteOn(channel, note, velocity);
            }
        }
    }

    public void SendNoteOff(int channel, int note)
    {
        foreach (var p in _ports)
        {
            p?.SendNoteOff(channel, note);
        }
    }

    public void SendNoteOff(int channel, int note, string deviceName)
    {
        foreach (var p in _ports)
        {
            if (p.GetPortName().IndexOf(deviceName) != -1)
            {
                p?.SendNoteOff(channel, note);
            }
        }
    }

    public void SendControlChange(int channel, int number, int value)
    {
        foreach (var p in _ports)
        {
            p?.SendControlChange(channel, number, value);
        }
    }

    public void SendControlChange(int channel, int number, int value, string deviceName)
    {
        foreach (var p in _ports)
        {
            if (p.GetPortName().IndexOf(deviceName) != -1)
            {
                p?.SendControlChange(channel, number, value);
            }
        }
    }
}
