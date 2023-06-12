public interface IMidiInput
{
    void OnMidiNoteOn(byte channel, byte note, byte velocity);
    void OnMidiNoteOff(byte channel, byte note);
    void OnMidiControlChange(byte channel, byte number, byte value);
}
