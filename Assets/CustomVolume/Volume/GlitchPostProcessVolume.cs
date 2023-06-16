using System;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Post-processing/Custom/Glitch")]
    public sealed class GlitchPostProcessVolume : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Controls the intensity of the effect.")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

        public IntParameter width = new IntParameter(16);
        public IntParameter height = new IntParameter(16);

        public ClampedFloatParameter threshold = new ClampedFloatParameter(0.5f, 0f, 1f);
        public IntParameter seed = new IntParameter(0);

        public bool IsActive() => intensity.value > 0f;

        public bool IsTileCompatible() => false;
    }
}
