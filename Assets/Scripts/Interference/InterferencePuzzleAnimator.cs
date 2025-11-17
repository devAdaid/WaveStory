using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace WaveStory.Interference
{
    /// <summary>
    /// 간섭 패턴의 실시간 애니메이션을 담당
    /// </summary>
    public class InterferencePuzzleAnimator : MonoBehaviour
    {
        [SerializeField] private InterferencePuzzlePresenter presenter;
        [SerializeField] private float animationSpeed = 1f;
        [SerializeField] private bool animateWaves = true;

        [Header("Colors")]
        [SerializeField] private Color lowColor = Color.blue;
        [SerializeField] private Color midColor = Color.black;
        [SerializeField] private Color highColor = Color.red;

        private List<Image> gridCells;
        private InterferencePuzzleContext context;
        private int gridWidth;
        private int gridHeight;
        private float time;

        public void Initialize(List<Image> cells, InterferencePuzzleContext ctx, int width, int height)
        {
            gridCells = cells;
            context = ctx;
            gridWidth = width;
            gridHeight = height;
            time = 0f;
        }

        private void Update()
        {
            if (!animateWaves || gridCells == null || context == null) return;
            if (context.PlacedSources.Count == 0) return;

            time += Time.deltaTime * animationSpeed;
            UpdateAnimatedPattern();
        }

        private void UpdateAnimatedPattern()
        {
            float maxValue = 0f;
            float[,] values = new float[gridWidth, gridHeight];

            // 먼저 모든 값 계산
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    float value = InterferenceLogic.CalculateWaveValueAt(
                        new Vector2Int(x, y),
                        context.PlacedSources,
                        time
                    );
                    values[x, y] = value;
                    maxValue = Mathf.Max(maxValue, Mathf.Abs(value));
                }
            }

            if (maxValue < 0.01f) maxValue = 1f;

            // 색상 업데이트
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    int index = y * gridWidth + x;
                    if (index < gridCells.Count)
                    {
                        float normalizedValue = values[x, y] / maxValue;

                        // -1 ~ 1 범위를 색상으로 매핑
                        Color cellColor;
                        if (normalizedValue > 0)
                        {
                            cellColor = Color.Lerp(midColor, highColor, normalizedValue);
                        }
                        else
                        {
                            cellColor = Color.Lerp(midColor, lowColor, -normalizedValue);
                        }

                        gridCells[index].color = cellColor;
                    }
                }
            }
        }

        public void SetAnimationSpeed(float speed)
        {
            animationSpeed = speed;
        }

        public void ToggleAnimation(bool enabled)
        {
            animateWaves = enabled;
        }

        public void ResetTime()
        {
            time = 0f;
        }
    }
}
