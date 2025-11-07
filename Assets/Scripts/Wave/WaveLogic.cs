using UnityEngine;

public static class WaveLogic
{
    public static float GetWaveY(WaveType waveType, float x, float amplitude, float frequency, float speed, float time)
    {
        switch (waveType)
        {
            case WaveType.Sin:
                return amplitude * Mathf.Sin(frequency * x + speed * time);
            case WaveType.Square:
                return amplitude * Mathf.Sign(Mathf.Sin(frequency * x + speed * time));
            case WaveType.PingPong:
                {
                    var t = (frequency * x + speed * time) / Mathf.PI;
                    return amplitude * (Mathf.PingPong(t, 2f) - 1f);
                }
        }

        return 0;
    }

    public static int MinAmplitudeStep => StaticDataHolder.I.WaveConstant.MinAmplitudeStep;
    public static int MaxAmplitudeStep => StaticDataHolder.I.WaveConstant.MaxAmplitudeStep;
    public static int MinFrequencyStep => StaticDataHolder.I.WaveConstant.MinFrequencyStep;
    public static int MaxFrequencyStep => StaticDataHolder.I.WaveConstant.MaxFrequencyStep;

    public static int GetClampedAmplitudeStep(int amplitudeStep)
    {
        return Mathf.Clamp(amplitudeStep, MinAmplitudeStep, MaxAmplitudeStep);
    }

    public static int GetClampedFrequenctStep(int frequencyStep)
    {
        return Mathf.Clamp(frequencyStep, MinFrequencyStep, MaxFrequencyStep);
    }
}
