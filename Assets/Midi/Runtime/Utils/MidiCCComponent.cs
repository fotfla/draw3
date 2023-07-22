using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MidiCCComponent : MonoBehaviour
{
    [SerializeField]
    byte number;

    public UnityEvent<byte> Event;

    public byte GetCCNumber()
    {
        return number;
    }
}
