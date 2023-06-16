using System;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Post-processing/Custom/UVShiftPostProcessVolume")]
    public sealed class UVShiftPostProcessVolume : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Controls the intensity of the effect.")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter shiftX = new ClampedFloatParameter(0,0,1);
        public ClampedFloatParameter shiftY = new ClampedFloatParameter(0,0,1);
        public bool IsActive() => intensity.value > 0f && ( shiftX.value > 0 || shiftY.value > 0 );
        public bool IsTileCompatible() => false;
    }
}
