using System;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Post-processing/Custom/DistortionPostProcessVolume")]
    public sealed class DistortionPostProcessVolume : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Controls the intensity of the effect.")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter amout = new ClampedFloatParameter(0.1f,0,1);
        public bool IsActive() => intensity.value > 0f;
        public bool IsTileCompatible() => false;
    }
}
