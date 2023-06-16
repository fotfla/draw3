using UnityEngine.Rendering.Universal;

namespace UnityEditor.Rendering.Universal
{
    [VolumeComponentEditor(typeof(InverseSlicePostProcessVolume))]
    sealed class InverseSliceEditor : VolumeComponentEditor
    {
        SerializedDataParameter shapeType;
        SerializedDataParameter intensity;
        SerializedDataParameter angle;
        SerializedDataParameter offset;
        SerializedDataParameter width;
        SerializedDataParameter radius;

        public override void OnEnable()
        {
            var o = new PropertyFetcher<InverseSlicePostProcessVolume>(serializedObject);

            shapeType = Unpack(o.Find(x => x.shapeType));

            intensity = Unpack(o.Find(x => x.intensity));

            angle = Unpack(o.Find(x => x.angle));
            offset = Unpack(o.Find(x => x.offset));
            angle = Unpack(o.Find(x => x.angle));
            offset = Unpack(o.Find(x => x.offset));
            width = Unpack(o.Find(x => x.width));
            radius = Unpack(o.Find(x => x.radius));
        }

        public override void OnInspectorGUI()
        {
            PropertyField(intensity);
            PropertyField(shapeType);

            if (shapeType.value.intValue == (int)ShapeType.Slice)
            {
                PropertyField(angle);
                PropertyField(offset);
            }
            else if (shapeType.value.intValue == (int)ShapeType.Triangle || shapeType.value.intValue == (int)ShapeType.Square  || shapeType.value.intValue == (int)ShapeType.Line)
            {
                PropertyField(width);
                PropertyField(radius);
                PropertyField(angle);
            }
            else if (shapeType.value.intValue == (int)ShapeType.Circle)
            {
                PropertyField(width);
                PropertyField(radius);
            }
        }
    }
}

