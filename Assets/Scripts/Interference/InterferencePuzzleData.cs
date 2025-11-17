using UnityEngine;
using System.Collections.Generic;

namespace WaveStory.Interference
{
    [CreateAssetMenu(fileName = "InterferencePuzzle", menuName = "WaveStory/Interference Puzzle")]
    public class InterferencePuzzleData : ScriptableObject
    {
        [Header("퍼즐 설정")]
        public string puzzleName;
        public int gridWidth = 10;
        public int gridHeight = 10;

        [Header("배치 가능한 파동 소스")]
        public int maxSources = 3;
        public float defaultAmplitude = 1f;
        public float defaultFrequency = 1f;

        [Header("목표 지점")]
        public List<TargetPoint> targetPoints = new List<TargetPoint>();

        [Header("클리어 조건")]
        [Range(0f, 1f)]
        public float requiredAccuracy = 0.8f;
    }

    [System.Serializable]
    public class TargetPoint
    {
        public Vector2Int position;
        public TargetType targetType;
        [Range(0f, 2f)]
        public float targetIntensity = 1f;
    }

    public enum TargetType
    {
        Constructive,  // 보강 간섭 (높은 진폭)
        Destructive    // 상쇄 간섭 (낮은 진폭)
    }
}
