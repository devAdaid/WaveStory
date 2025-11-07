using UnityEngine;

[CreateAssetMenu(fileName = "WaveConstant", menuName = "Scriptable Objects/WaveConstant")]
public class WaveConstant : ScriptableObject
{
    public float MaxAmplitude;
    public int MaxAmplitudeStep;
    public int MinAmplitudeStep = 1;

    public float MaxFrequency;
    public int MaxFrequencyStep;
    public int MinFrequencyStep = 1;
}
