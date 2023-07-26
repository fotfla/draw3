using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class EdgeDetectionControl : MonoBehaviour, IMidiInput
{
    [SerializeField]
    byte ccNumber;

    EdgeDetection edgeDetection;

    private void Start()
    {
        var profile = GetComponent<Volume>().profile;
        profile.TryGet(out edgeDetection);
    }

    public void OnMidiControlChange(byte channel, byte number, byte value)
    {
        if (ccNumber == number) edgeDetection.intensity.value = (value / 127.0f);
    }

    public void OnMidiNoteOff(byte channel, byte note)
    {
    }

    public void OnMidiNoteOn(byte channel, byte note, byte velocity)
    {
    }
}
