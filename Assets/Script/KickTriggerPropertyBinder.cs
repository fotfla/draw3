using System;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;
using Random = Unity.Mathematics.Random;

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
    Random random;

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
            random.InitState(65536);
            trigger.onKickOn += OnKick;
            trigger.onKickOff += OnKickOff;
        }
    }

    private void OnKick(float audioLevel)
    {
        this.seed = random.NextUInt();
        this.audioLevel = audioLevel;
    }

    private void OnKickOff(float audioLevel)
    {
        this.audioLevel = 0;
    }


    void OnDestroy()
    {
        if (trigger != null) trigger.onKickOn -= OnKick;
    }
}


