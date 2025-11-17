using UnityEngine;

namespace WaveStory.ImageInvert
{
    [CreateAssetMenu(fileName = "ImageInvertPuzzle", menuName = "WaveStory/Image Invert Puzzle")]
    public class ImageInvertPuzzleData : ScriptableObject
    {
        [Header("퍼즐 설정")]
        public string puzzleName;

        [Header("12x12 비트맵 이미지")]
        [Tooltip("144개의 bool 값 (12x12). true = 켜짐, false = 꺼짐. 행 우선 순서 (row-major order)")]
        public bool[] bitmap = new bool[144];

        [Header("난이도 설정")]
        [Tooltip("시작 시 인버트될 행/열의 개수")]
        [Range(1, 12)]
        public int initialInvertCount = 3;

        [Tooltip("최대 시도 횟수 (행/열 토글 횟수)")]
        public int maxMoves = 10;

        [Tooltip("제한 시간 (초). 0이면 시간 제한 없음")]
        public float timeLimit = 60f;

        /// <summary>
        /// 1D 배열 인덱스를 2D 좌표로 변환
        /// </summary>
        public static int To1DIndex(int x, int y)
        {
            return y * 12 + x;
        }

        /// <summary>
        /// 특정 좌표의 비트맵 값 가져오기
        /// </summary>
        public bool GetPixel(int x, int y)
        {
            if (x < 0 || x >= 12 || y < 0 || y >= 12)
                return false;
            return bitmap[To1DIndex(x, y)];
        }

        /// <summary>
        /// 특정 좌표의 비트맵 값 설정하기
        /// </summary>
        public void SetPixel(int x, int y, bool value)
        {
            if (x < 0 || x >= 12 || y < 0 || y >= 12)
                return;
            bitmap[To1DIndex(x, y)] = value;
        }

        /// <summary>
        /// 전체 비트맵을 2D 배열로 복사
        /// </summary>
        public bool[,] GetBitmapAs2D()
        {
            bool[,] result = new bool[12, 12];
            for (int y = 0; y < 12; y++)
            {
                for (int x = 0; x < 12; x++)
                {
                    result[x, y] = GetPixel(x, y);
                }
            }
            return result;
        }
    }
}
