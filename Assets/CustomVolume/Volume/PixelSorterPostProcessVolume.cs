using System;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Post-processing/Custom/PixelSorter")]
    public class PixelSorterPostProcessVolume : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Controls the intensity of the effect.")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

        public ClampedFloatParameter threshold = new ClampedFloatParameter(0, 0, 1);
        public BoolParameter vertical = new BoolParameter(false);
        public BoolParameter invert = new BoolParameter(false);
        public bool IsActive() => intensity.value > 0f;
        public bool IsTileCompatible() => false;
    }
}
