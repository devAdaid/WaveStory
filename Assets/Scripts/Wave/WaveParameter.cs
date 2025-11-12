using System;

[System.Serializable]
public struct WaveParameter : IEquatable<WaveParameter>
{
    public WaveType WaveType;
    public int AmplitudeStep;
    public int FrequencyStep;

    public static WaveParameter Invalid => new WaveParameter(WaveType.Sin, -1, -1);
    public static WaveParameter Min => new WaveParameter(WaveType.Sin, WaveLogic.MinAmplitudeStep, WaveLogic.MinFrequencyStep);

    public WaveParameter(WaveType waveType, int amplitudeStep, int frequencyStep)
    {
        WaveType = waveType;
        AmplitudeStep = amplitudeStep;
        FrequencyStep = frequencyStep;
    }

    // IEquatable<WaveParameter> 구현
    public bool Equals(WaveParameter other)
    {
        return WaveType == other.WaveType
            && AmplitudeStep == other.AmplitudeStep
            && FrequencyStep == other.FrequencyStep;
    }

    // Object.Equals 오버라이드
    public override bool Equals(object obj)
    {
        return obj is WaveParameter other && Equals(other);
    }

    // Object.GetHashCode 오버라이드
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + WaveType.GetHashCode();
            hash = hash * 31 + AmplitudeStep.GetHashCode();
            hash = hash * 31 + FrequencyStep.GetHashCode();
            return hash;
        }
    }

    // == 연산자 오버라이드
    public static bool operator ==(WaveParameter left, WaveParameter right)
    {
        return left.Equals(right);
    }

    // != 연산자 오버라이드
    public static bool operator !=(WaveParameter left, WaveParameter right)
    {
        return !left.Equals(right);
    }
}