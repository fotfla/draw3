using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityEngine.Rendering.Universal
{
    [RequireComponent(typeof(Volume))]
    public class SlitScanControl : MonoBehaviour, IMidiInput
    {
        [SerializeField]
        byte speedNumber;
        [SerializeField]
        byte splitNumber;

        [SerializeField]
        float minSpeed = 0.01f;
        [SerializeField]
        float maxSpeed = 20;
        SlitScan slitScan;

        void Start()
        {
            var profile = GetComponent<Volume>().profile;
            profile.TryGet(out slitScan);
        }

        public void OnMidiControlChange(byte channel, byte number, byte value)
        {
            if (speedNumber == number) slitScan.speed.value = (value / 128.0f) * (maxSpeed - minSpeed) + minSpeed;
            if (splitNumber == number) slitScan.split.value = value / 128f;
        }

        public void OnMidiNoteOn(byte channel, byte number, byte value) { }

        public void OnMidiNoteOff(byte channel, byte number) { }
    }
}
