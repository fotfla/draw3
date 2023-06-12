using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPad : MonoBehaviour
{
    [SerializeField]
    MidiOutput midiOutput;

    [SerializeField]
    IMidiInput[] midiInput;
}
