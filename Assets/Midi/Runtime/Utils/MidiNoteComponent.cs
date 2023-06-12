using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidiNoteComponent : MonoBehaviour
{
    public byte note = 255;

    public virtual void SetValue(byte value = 0) { }
}
