using System;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Post-processing/Custom/EdgeDitection")]
    public sealed class EdgeDetection : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Controls the intensity of the effect.")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
        public bool IsActive() => intensity.value > 0f;
        public bool IsTileCompatible() => false;
    }
}
