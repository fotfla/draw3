using System;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Post-processing/Custom/TextureOverray")]
    public sealed class TextureOverrayPostProcessVolume : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Controls the intensity of the effect.")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
        public TextureParameter texture = new TextureParameter(null);
        public bool IsActive() => intensity.value > 0f && texture != null;
        public bool IsTileCompatible() => false;
    }
}
