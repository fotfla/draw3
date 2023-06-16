using UnityEngine.Rendering;

namespace UnityEngine.Rendering.Universal
{
    public enum ShapeType
    {
        All = 0,
        Slice = 1,
        Circle = 2,
        Triangle = 3,
        Square = 4,
        Line = 5,
    }

    [System.Serializable]
    public sealed class ShapeTypeParameter : VolumeParameter<ShapeType>
    {
        public ShapeTypeParameter(ShapeType value, bool overrideState = false) : base(value, overrideState) { }
    }
}

