using System;

namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Post-processing/Custom/InverseSlicePostProcessVolume")]
    public sealed class InverseSlicePostProcessVolume : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Controls the intensity of the effect.")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

        public ClampedFloatParameter angle = new ClampedFloatParameter(0,0,1);
        public Vector2Parameter offset = new Vector2Parameter(Vector2.zero);

        public FloatParameter width = new FloatParameter(0.1f);
        public FloatParameter radius = new FloatParameter(0);

        public ShapeTypeParameter shapeType = new ShapeTypeParameter(ShapeType.All);

        public bool IsActive() => intensity.value > 0f;

        public bool IsTileCompatible() => false;
    }
}



