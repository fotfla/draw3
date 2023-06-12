using System;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[VFXBinder("Audio/KickTrigger")]
public class KickTriggerPropertyBinder : VFXBinderBase
{
    [VFXPropertyBinding(nameof(System.Single))]
    public ExposedProperty valueProperty;

    [VFXPropertyBinding(nameof(System.Single))]
    public ExposedProperty audioLevelPorperty;

    public KickTrigger trigger;

    uint seed;
    float audioLevel;

    public float AttackSpeed;

    public override bool IsValid(VisualEffect component)
    {
        return component.HasUInt(valueProperty) || component.HasFloat(audioLevelPorperty);
    }

    public override void UpdateBinding(VisualEffect component)
    {
        audioLevel -= AttackSpeed * Time.deltaTime;
        audioLevel = Mathf.Max(audioLevel, 0);
        
        component.SetUInt(valueProperty, seed);
        component.SetFloat(audioLevelPorperty, audioLevel);
    }

    void Start()
    {
        if (trigger != null)
        {
            trigger.onKickOn += OnKick;
            trigger.onKickOff += OnKickOff;
        }
    }

    private void OnKick(float audioLevel, uint seed)
    {
        this.seed = seed;
        this.audioLevel = audioLevel;
    }

    private void OnKickOff(float audioLevel, uint seed)
    {
        this.audioLevel = 0;
    }


    void OnDestroy()
    {
        if (trigger != null) trigger.onKickOn -= OnKick;
    }
}


