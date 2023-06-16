using System;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Post-processing/Custom/Fluid")]
    public class FluidPostProcessVolume : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Controls the intensity of the effect.")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

        public ClampedFloatParameter clamp = new ClampedFloatParameter(0.05f, 0f, 0.2f);
        public bool IsActive() => intensity.value > 0f;
        public bool IsTileCompatible() => false;
    }
}

