using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[VFXBinder("Midi/CC")]
public class MidiCCPropertyBinder : VFXBinderBase
{
    [VFXPropertyBinding(nameof(System.Single))]
    public ExposedProperty valueProperty;

    public byte CCNumber;

    public float minValue = 0;
    public float maxValue = 1;
    byte _value;

    public override bool IsValid(VisualEffect component)
    {
        return component.HasFloat(valueProperty);
    }

    public override void UpdateBinding(VisualEffect component)
    {
        component.SetFloat(valueProperty, (maxValue - minValue) * _value / 127.0f + minValue);
    }

    public override string ToString()
    {
        return string.Format("Midi CC '{0} -> CC {1}'", valueProperty, CCNumber);
    }

    public void SetValue(byte value)
    {
        _value = value;
    }
}