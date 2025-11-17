using UnityEngine;
using System.Collections.Generic;

namespace WaveStory.Interference
{
    public static class InterferenceLogic
    {
        /// <summary>
        /// 특정 위치에서의 파동 간섭 강도를 계산
        /// </summary>
        public static float CalculateIntensityAt(Vector2Int position, List<WaveSource> sources)
        {
            if (sources == null || sources.Count == 0)
                return 0f;

            float totalAmplitude = 0f;

            foreach (var source in sources)
            {
                float distance = Vector2Int.Distance(position, source.position);

                // 거리에 따른 감쇠 (1/r 감쇠)
                float attenuation = distance > 0.1f ? 1f / (1f + distance * 0.5f) : 1f;

                // 파동 방정식: A * sin(k*r + phase)
                // k = 2π * frequency
                float waveNumber = 2f * Mathf.PI * source.frequency;
                float waveValue = source.amplitude * attenuation * Mathf.Sin(waveNumber * distance + source.phase);

                totalAmplitude += waveValue;
            }

            // 강도는 진폭의 제곱에 비례 (물리적으로 정확)
            // 하지만 게임플레이를 위해 절대값 사용
            return Mathf.Abs(totalAmplitude);
        }

        /// <summary>
        /// 시간에 따른 파동 값 계산 (애니메이션용)
        /// </summary>
        public static float CalculateWaveValueAt(Vector2Int position, List<WaveSource> sources, float time)
        {
            if (sources == null || sources.Count == 0)
                return 0f;

            float totalAmplitude = 0f;

            foreach (var source in sources)
            {
                float distance = Vector2Int.Distance(position, source.position);
                float attenuation = distance > 0.1f ? 1f / (1f + distance * 0.5f) : 1f;

                float waveNumber = 2f * Mathf.PI * source.frequency;
                float angularFrequency = 2f * Mathf.PI * source.frequency;

                // 시간에 따른 파동: A * sin(k*r - ω*t + phase)
                float waveValue = source.amplitude * attenuation *
                    Mathf.Sin(waveNumber * distance - angularFrequency * time + source.phase);

                totalAmplitude += waveValue;
            }

            return totalAmplitude;
        }

        /// <summary>
        /// 전체 그리드의 간섭 패턴 계산
        /// </summary>
        public static float[,] CalculateInterferencePattern(int width, int height, List<WaveSource> sources)
        {
            float[,] pattern = new float[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pattern[x, y] = CalculateIntensityAt(new Vector2Int(x, y), sources);
                }
            }

            return pattern;
        }

        /// <summary>
        /// 목표 지점의 정확도 계산
        /// </summary>
        public static float CalculateTargetAccuracy(TargetPoint target, float actualIntensity)
        {
            float expectedIntensity = target.targetIntensity;

            switch (target.targetType)
            {
                case TargetType.Constructive:
                    // 보강 간섭: 높은 강도 필요
                    if (actualIntensity >= expectedIntensity)
                        return 1f;
                    return actualIntensity / expectedIntensity;

                case TargetType.Destructive:
                    // 상쇄 간섭: 낮은 강도 필요 (0에 가까울수록 좋음)
                    float threshold = 0.3f; // 이 값 이하면 상쇄 성공
                    if (actualIntensity <= threshold)
                        return 1f;
                    return Mathf.Max(0f, 1f - (actualIntensity - threshold) / (expectedIntensity - threshold));

                default:
                    return 0f;
            }
        }

        /// <summary>
        /// 두 파동 소스 사이의 간섭 타입 판별
        /// </summary>
        public static bool IsConstructiveInterference(WaveSource source1, WaveSource source2, Vector2Int position)
        {
            float dist1 = Vector2Int.Distance(position, source1.position);
            float dist2 = Vector2Int.Distance(position, source2.position);

            float pathDifference = Mathf.Abs(dist1 - dist2);
            float wavelength = 1f / source1.frequency; // 단순화: 두 소스의 주파수가 같다고 가정

            // 경로차가 파장의 정수배면 보강 간섭
            float ratio = pathDifference / wavelength;
            float fractionalPart = ratio - Mathf.Floor(ratio);

            return fractionalPart < 0.25f || fractionalPart > 0.75f;
        }
    }
}
