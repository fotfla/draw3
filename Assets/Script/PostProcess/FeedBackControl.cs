using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEngine.Rendering.Universal
{
    [RequireComponent(typeof(Volume))]
    public class FeedBackControl : MonoBehaviour, IMidiInput
    {
        [SerializeField]
        byte ccNumber;
        [SerializeField]
        byte speedNumber;
        [SerializeField]
        byte scaleNumber;

        FeedBack feedBack;

        void Start()
        {
            GetComponent<Volume>().profile.TryGet(out feedBack);
        }

        public void OnMidiControlChange(byte channel, byte number, byte value)
        {
            if (ccNumber == number) feedBack.intensity.value = (value / 128.0f);
            if (speedNumber == number) feedBack.speed.value = (value / 127.0f);
            if (scaleNumber == number) feedBack.scale.value = (value / 128.0f);
        }

        public void OnMidiNoteOn(byte channel, byte number, byte value) { }

        public void OnMidiNoteOff(byte channel, byte number) { }
    }
}
