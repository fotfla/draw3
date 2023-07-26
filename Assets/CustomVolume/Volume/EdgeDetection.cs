using System;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Post-processing/Custom/EdgeDitection")]
    public sealed class EdgeDetection : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Controls the intensity of the effect.")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
        public IntParameter depth = new IntParameter(1);
        public bool IsActive() => intensity.value > 0f && depth.value > 0;
        public bool IsTileCompatible() => false;
    }
}
