using System;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Post-processing/Custom/ScanLine")]
    public sealed class ScanLinePostProcessVolume : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Controls the intensity of the effect.")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
        public FloatParameter width = new FloatParameter(2f);
        public FloatParameter Lnegth = new FloatParameter(150);
        public ClampedFloatParameter depth = new ClampedFloatParameter(0, 0, 1);

        public bool IsActive() => intensity.value > 0f;

        public bool IsTileCompatible() => false;
    }
}
