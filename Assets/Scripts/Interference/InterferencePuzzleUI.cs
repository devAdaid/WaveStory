using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace WaveStory.Interference
{
    public class InterferencePuzzleUI : MonoBehaviour
    {
        [Header("Grid")]
        [SerializeField] private RectTransform gridContainer;
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private float cellSize = 40f;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI puzzleNameText;
        [SerializeField] private TextMeshProUGUI accuracyText;
        [SerializeField] private TextMeshProUGUI sourceCountText;
        [SerializeField] private Button clearButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject solvedIndicator;

        [Header("Colors")]
        [SerializeField] private Color emptyColor = Color.black;
        [SerializeField] private Color lowIntensityColor = Color.blue;
        [SerializeField] private Color highIntensityColor = Color.red;
        [SerializeField] private Color sourceColor = Color.yellow;
        [SerializeField] private Color constructiveTargetColor = Color.green;
        [SerializeField] private Color destructiveTargetColor = Color.cyan;

        private List<Image> gridCells = new List<Image>();
        private List<Image> targetMarkers = new List<Image>();
        private List<Image> sourceMarkers = new List<Image>();

        private int currentWidth;
        private int currentHeight;

        public System.Action<Vector2Int> OnCellClicked;
        public System.Action OnClearClicked;
        public System.Action OnCloseClicked;

        private void Awake()
        {
            if (clearButton != null)
                clearButton.onClick.AddListener(() => OnClearClicked?.Invoke());

            if (closeButton != null)
                closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
        }

        public void Initialize(int width, int height)
        {
            currentWidth = width;
            currentHeight = height;

            ClearGrid();
            CreateGrid(width, height);
        }

        private void ClearGrid()
        {
            foreach (var cell in gridCells)
            {
                if (cell != null)
                    Destroy(cell.gameObject);
            }
            gridCells.Clear();

            foreach (var marker in targetMarkers)
            {
                if (marker != null)
                    Destroy(marker.gameObject);
            }
            targetMarkers.Clear();

            foreach (var marker in sourceMarkers)
            {
                if (marker != null)
                    Destroy(marker.gameObject);
            }
            sourceMarkers.Clear();
        }

        private void CreateGrid(int width, int height)
        {
            if (gridContainer == null || cellPrefab == null) return;

            float totalWidth = width * cellSize;
            float totalHeight = height * cellSize;

            gridContainer.sizeDelta = new Vector2(totalWidth, totalHeight);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var cellObj = Instantiate(cellPrefab, gridContainer);
                    var rectTransform = cellObj.GetComponent<RectTransform>();

                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.zero;
                    rectTransform.sizeDelta = new Vector2(cellSize, cellSize);
                    rectTransform.anchoredPosition = new Vector2(
                        x * cellSize + cellSize / 2,
                        y * cellSize + cellSize / 2
                    );

                    var image = cellObj.GetComponent<Image>();
                    if (image != null)
                    {
                        image.color = emptyColor;
                        gridCells.Add(image);
                    }

                    var button = cellObj.GetComponent<Button>();
                    if (button != null)
                    {
                        int capturedX = x;
                        int capturedY = y;
                        button.onClick.AddListener(() =>
                        {
                            OnCellClicked?.Invoke(new Vector2Int(capturedX, capturedY));
                        });
                    }
                }
            }
        }

        public void UpdateInterferencePattern(float[,] pattern)
        {
            if (pattern == null) return;

            int width = pattern.GetLength(0);
            int height = pattern.GetLength(1);

            float maxIntensity = 0f;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    maxIntensity = Mathf.Max(maxIntensity, pattern[x, y]);
                }
            }

            if (maxIntensity < 0.01f) maxIntensity = 1f;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    if (index < gridCells.Count)
                    {
                        float normalizedIntensity = pattern[x, y] / maxIntensity;
                        gridCells[index].color = Color.Lerp(lowIntensityColor, highIntensityColor, normalizedIntensity);
                    }
                }
            }
        }

        public void ShowTargetPoints(List<TargetPoint> targets)
        {
            foreach (var marker in targetMarkers)
            {
                if (marker != null)
                    Destroy(marker.gameObject);
            }
            targetMarkers.Clear();

            if (gridContainer == null) return;

            foreach (var target in targets)
            {
                var markerObj = new GameObject("TargetMarker");
                markerObj.transform.SetParent(gridContainer, false);

                var rectTransform = markerObj.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;
                rectTransform.sizeDelta = new Vector2(cellSize * 0.6f, cellSize * 0.6f);
                rectTransform.anchoredPosition = new Vector2(
                    target.position.x * cellSize + cellSize / 2,
                    target.position.y * cellSize + cellSize / 2
                );

                var image = markerObj.AddComponent<Image>();
                image.color = target.targetType == TargetType.Constructive
                    ? constructiveTargetColor
                    : destructiveTargetColor;

                // 테두리 효과를 위해 Outline 추가
                var outline = markerObj.AddComponent<Outline>();
                outline.effectColor = Color.white;
                outline.effectDistance = new Vector2(2, 2);

                targetMarkers.Add(image);
            }
        }

        public void ShowSourcePositions(List<WaveSource> sources)
        {
            foreach (var marker in sourceMarkers)
            {
                if (marker != null)
                    Destroy(marker.gameObject);
            }
            sourceMarkers.Clear();

            if (gridContainer == null) return;

            foreach (var source in sources)
            {
                var markerObj = new GameObject("SourceMarker");
                markerObj.transform.SetParent(gridContainer, false);

                var rectTransform = markerObj.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;
                rectTransform.sizeDelta = new Vector2(cellSize * 0.8f, cellSize * 0.8f);
                rectTransform.anchoredPosition = new Vector2(
                    source.position.x * cellSize + cellSize / 2,
                    source.position.y * cellSize + cellSize / 2
                );

                var image = markerObj.AddComponent<Image>();
                image.color = sourceColor;

                var outline = markerObj.AddComponent<Outline>();
                outline.effectColor = Color.black;
                outline.effectDistance = new Vector2(3, 3);

                sourceMarkers.Add(image);
            }
        }

        public void UpdatePuzzleInfo(string puzzleName, float accuracy, int currentSources, int maxSources)
        {
            if (puzzleNameText != null)
                puzzleNameText.text = puzzleName;

            if (accuracyText != null)
                accuracyText.text = $"정확도: {accuracy:P0}";

            if (sourceCountText != null)
                sourceCountText.text = $"파동 소스: {currentSources}/{maxSources}";
        }

        public void ShowSolvedState(bool solved)
        {
            if (solvedIndicator != null)
                solvedIndicator.SetActive(solved);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
