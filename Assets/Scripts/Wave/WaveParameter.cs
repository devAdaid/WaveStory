public struct WaveParameter
{
    public WaveType WaveType { get; set; }
    public int AmplitudeStep { get; set; }
    public int FrequencyStep { get; set; }

    public WaveParameter(WaveType waveType, int amplitudeStep, int frequencyStep)
    {
        WaveType = waveType;
        AmplitudeStep = amplitudeStep;
        FrequencyStep = frequencyStep;
    }
}
