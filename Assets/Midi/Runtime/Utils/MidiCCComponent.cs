using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MidiCCComponent : MonoBehaviour
{
    [SerializeField, CCIndex]
    MidiInputCCTrigger[] ccTriggers;

    public MidiInputCCTrigger[] GetTrigger()
    {
        return ccTriggers;
    }
}
