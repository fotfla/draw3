using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lasp;
using System;
using static UnityEngine.Rendering.DebugUI;

public class UIControl : MonoBehaviour
{
    [SerializeField]
    Image levelImage;

    [SerializeField]
    Image ThresholdImage;

    [SerializeField]
    Image KickImage;

    [SerializeField]
    AudioLevelTracker tracker;

    [SerializeField]
    KickTrigger trigger;

    void Start()
    {
        trigger.onKickOn += OnKickOn;
        trigger.onKickOff += OnKickOff;
        KickImage.enabled = false;
    }

    private void OnKickOff(float audioLevel)
    {
        KickImage.enabled = false;
    }

    private void OnKickOn(float audioLevel)
    {
        KickImage.enabled = true;
    }

    void Update()
    {
        levelImage.rectTransform.localScale = new Vector3(tracker.normalizedLevel, 1, 1);
        ThresholdImage.rectTransform.localPosition = new Vector3(trigger.GetThreshold() * 320 - 80, 0, 0);
    }


    public void SetThreshold(byte value)
    {
        ThresholdImage.rectTransform.localPosition = new Vector3(value / 127.0f * 320 - 80, 0, 0);
    }
}
