using System;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Post-processing/Custom/WorldVoroNoisePostProcessVolume")]
    public sealed class WorldVoroNoisePostProcessVolume : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Controls the intensity of the effect.")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
        public FloatParameter noiseScale = new FloatParameter(0.1f);
        public FloatParameter noiseIntensity = new FloatParameter(0.1f);
        public IntParameter seed = new IntParameter(0);

        public bool IsActive() => intensity.value > 0f;

        public bool IsTileCompatible() => false;
    }
}
