using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[VFXBinder("Midi/CC")]
public class MidiCCPropertyBinder : VFXBinderBase
{
    [VFXPropertyBinding(nameof(System.Single))]
    public ExposedProperty valueProperty;

    public byte number;
    byte _value;

    public override bool IsValid(VisualEffect component)
    {
        return component.HasFloat(valueProperty);
    }

    public override void UpdateBinding(VisualEffect component)
    {
        component.SetFloat(valueProperty, _value / 127.0f);
    }

    public override string ToString()
    {
        return string.Format("Midi CC '{0} -> CC {1}'", valueProperty, number);
    }

    public void SetValue(byte value)
    {
        _value = value;
    }
}