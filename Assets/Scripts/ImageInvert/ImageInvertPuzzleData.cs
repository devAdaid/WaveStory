using UnityEngine;

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

    public static int To1DIndex(int x, int y)
    {
        return y * 12 + x;
    }

    public bool GetPixel(int x, int y)
    {
        if (x < 0 || x >= 12 || y < 0 || y >= 12)
            return false;
        return bitmap[To1DIndex(x, y)];
    }

    public void SetPixel(int x, int y, bool value)
    {
        if (x < 0 || x >= 12 || y < 0 || y >= 12)
            return;
        bitmap[To1DIndex(x, y)] = value;
    }

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
