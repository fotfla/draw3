using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Post-processing/Custom/CrackShiftPostProcessVolume")]
    public sealed class CrackShiftPostProcessVolume : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Controls the intensity of the effect.")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

        public IntParameter seed = new IntParameter(65535);
        public IntParameter div = new IntParameter(0);

        public bool IsActive() => intensity.value > 0f;
        public bool IsTileCompatible() => false;
    }
}

