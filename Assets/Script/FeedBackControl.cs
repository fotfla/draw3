using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class FeedBackControl : MonoBehaviour, IMidiInput
{
    [SerializeField]
    byte ccNumber;

    FeedBack feedBack;

    void Start()
    {
        var profile = GetComponent<Volume>().profile;
        profile.TryGet<FeedBack>(out feedBack);
    }

    public void OnMidiControlChange(byte channel, byte number, byte value)
    {
        if (ccNumber == number) feedBack.intensity.value = (value / 128.0f);
    }

    public void OnMidiNoteOn(byte channel, byte number, byte value) { }

    public void OnMidiNoteOff(byte channel, byte number) { }
}
