using UnityEngine;

namespace WaveStory.Interference
{
    [System.Serializable]
    public class WaveSource
    {
        public Vector2Int position;
        public float amplitude = 1f;
        public float frequency = 1f;
        public float phase = 0f;

        public WaveSource(Vector2Int pos, float amp = 1f, float freq = 1f, float ph = 0f)
        {
            position = pos;
            amplitude = amp;
            frequency = freq;
            phase = ph;
        }

        public WaveSource Clone()
        {
            return new WaveSource(position, amplitude, frequency, phase);
        }
    }
}
