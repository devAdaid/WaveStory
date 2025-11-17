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
        [SerializeField] private Button helpButton;
        [SerializeField] private GameObject helpPanel;

        [Header("Colors")]
        [SerializeField] private Color emptyColor = Color.black;
        [SerializeField] private Color lowIntensityColor = Color.blue;
        [SerializeField] private Color highIntensityColor = Color.red;
        [SerializeField] private Color sourceColor = Color.yellow;
        [SerializeField] private Color constructiveTargetColor = Color.green;
        [SerializeField] private Color destructiveTargetColor = Color.cyan;
        [SerializeField] private Color cellBorderColor = new Color(0.3f, 0.3f, 0.3f, 1f);

        private List<Image> gridCells = new List<Image>();
        private List<Image> targetMarkers = new List<Image>();
        private List<Image> sourceMarkers = new List<Image>();
        private List<TextMeshProUGUI> targetStatusTexts = new List<TextMeshProUGUI>();
        private List<TargetPoint> currentTargets = new List<TargetPoint>();

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

            if (helpButton != null)
                helpButton.onClick.AddListener(ToggleHelp);

            if (helpPanel != null)
                helpPanel.SetActive(false);
        }

        private void ToggleHelp()
        {
            if (helpPanel != null)
                helpPanel.SetActive(!helpPanel.activeSelf);
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

            foreach (var text in targetStatusTexts)
            {
                if (text != null)
                    Destroy(text.gameObject);
            }
            targetStatusTexts.Clear();
            currentTargets.Clear();
        }

        private void CreateGrid(int width, int height)
        {
            if (gridContainer == null || cellPrefab == null) return;

            // GridLayoutGroup 설정
            var gridLayout = gridContainer.GetComponent<GridLayoutGroup>();
            if (gridLayout == null)
            {
                gridLayout = gridContainer.gameObject.AddComponent<GridLayoutGroup>();
            }

            // GridContainer의 현재 크기를 기준으로 셀 크기 계산
            float containerWidth = gridContainer.rect.width;
            float containerHeight = gridContainer.rect.height;

            if (containerWidth <= 0) containerWidth = gridContainer.sizeDelta.x;
            if (containerHeight <= 0) containerHeight = gridContainer.sizeDelta.y;


            // GridLayoutGroup 설정
            gridLayout.spacing = Vector2.zero;
            gridLayout.startCorner = GridLayoutGroup.Corner.LowerLeft;
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.childAlignment = TextAnchor.MiddleCenter;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = width;

            // 셀 생성 (위치/크기는 GridLayoutGroup이 자동 처리)
            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    var cellObj = Instantiate(cellPrefab, gridContainer);

                    var image = cellObj.GetComponent<Image>();
                    if (image != null)
                    {
                        image.color = emptyColor;
                        gridCells.Add(image);
                    }

                    // 셀 경계선 추가
                    var outline = cellObj.GetComponent<Outline>();
                    if (outline == null)
                    {
                        outline = cellObj.AddComponent<Outline>();
                    }
                    outline.effectColor = cellBorderColor;
                    outline.effectDistance = new Vector2(1, 1);

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

            // GridLayoutGroup이 LowerLeft에서 시작하므로 y를 역순으로 처리
            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = (height - 1 - y) * width + x;
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

            foreach (var text in targetStatusTexts)
            {
                if (text != null)
                    Destroy(text.gameObject);
            }
            targetStatusTexts.Clear();
            currentTargets.Clear();

            if (gridContainer == null) return;

            currentTargets.AddRange(targets);

            // 그리드 오프셋 계산 (CreateGrid와 동일하게)
            float containerWidth = gridContainer.rect.width;
            float containerHeight = gridContainer.rect.height;
            if (containerWidth <= 0) containerWidth = gridContainer.sizeDelta.x;
            if (containerHeight <= 0) containerHeight = gridContainer.sizeDelta.y;

            foreach (var target in targets)
            {
                var markerObj = new GameObject("TargetMarker");
                markerObj.transform.SetParent(gridContainer, false);

                var rectTransform = markerObj.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;

                var image = markerObj.AddComponent<Image>();
                image.color = target.targetType == TargetType.Constructive
                    ? constructiveTargetColor
                    : destructiveTargetColor;
                image.raycastTarget = false;

                // 테두리 효과를 위해 Outline 추가
                var outline = markerObj.AddComponent<Outline>();
                outline.effectColor = Color.white;
                outline.effectDistance = new Vector2(2, 2);

                targetMarkers.Add(image);

                // 타겟 타입 라벨 추가
                var labelObj = new GameObject("TargetLabel");
                labelObj.transform.SetParent(gridContainer, false);

                var labelRect = labelObj.AddComponent<RectTransform>();
                labelRect.anchorMin = Vector2.zero;
                labelRect.anchorMax = Vector2.zero;
                labelRect.sizeDelta = new Vector2(60f, 20f);

                var labelText = labelObj.AddComponent<TextMeshProUGUI>();
                labelText.fontSize = 10;
                labelText.alignment = TextAlignmentOptions.Center;
                labelText.color = Color.white;
                labelText.text = target.targetType == TargetType.Constructive ? "강하게!" : "약하게!";
                labelText.raycastTarget = false;

                var labelOutline = labelObj.AddComponent<Outline>();
                labelOutline.effectColor = Color.black;
                labelOutline.effectDistance = new Vector2(1, 1);

                targetStatusTexts.Add(labelText);
            }
        }

        public void UpdateTargetStatus(List<WaveSource> sources)
        {
            if (currentTargets.Count != targetStatusTexts.Count) return;

            for (int i = 0; i < currentTargets.Count; i++)
            {
                var target = currentTargets[i];
                float intensity = InterferenceLogic.CalculateIntensityAt(target.position, sources);
                float accuracy = InterferenceLogic.CalculateTargetAccuracy(target, intensity);

                string statusText;
                Color statusColor;

                if (accuracy >= 1f)
                {
                    statusText = "완벽!";
                    statusColor = Color.green;
                }
                else if (accuracy >= 0.7f)
                {
                    statusText = "좋음";
                    statusColor = Color.yellow;
                }
                else if (sources.Count == 0)
                {
                    statusText = target.targetType == TargetType.Constructive ? "강하게!" : "약하게!";
                    statusColor = Color.white;
                }
                else
                {
                    if (target.targetType == TargetType.Constructive)
                    {
                        statusText = $"더 강하게! ({intensity:F1})";
                    }
                    else
                    {
                        statusText = $"더 약하게! ({intensity:F1})";
                    }
                    statusColor = Color.red;
                }

                targetStatusTexts[i].text = statusText;
                targetStatusTexts[i].color = statusColor;

                // 타겟 마커 색상도 업데이트
                if (i < targetMarkers.Count)
                {
                    Color baseColor = target.targetType == TargetType.Constructive
                        ? constructiveTargetColor
                        : destructiveTargetColor;

                    if (accuracy >= 1f)
                    {
                        targetMarkers[i].color = Color.Lerp(baseColor, Color.white, 0.5f);
                    }
                    else
                    {
                        targetMarkers[i].color = baseColor;
                    }
                }
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

            // 그리드 오프셋 계산 (CreateGrid와 동일하게)
            float containerWidth = gridContainer.rect.width;
            float containerHeight = gridContainer.rect.height;
            if (containerWidth <= 0) containerWidth = gridContainer.sizeDelta.x;
            if (containerHeight <= 0) containerHeight = gridContainer.sizeDelta.y;

            foreach (var source in sources)
            {
                var markerObj = new GameObject("SourceMarker");
                markerObj.transform.SetParent(gridContainer, false);

                var rectTransform = markerObj.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;

                var image = markerObj.AddComponent<Image>();
                image.color = sourceColor;
                image.raycastTarget = false;

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
            // 도움말 패널도 닫기
            if (helpPanel != null)
                helpPanel.SetActive(false);
        }

        public void ShowHelpOnFirstPlay()
        {
            // 첫 플레이 시 도움말 자동 표시
            if (helpPanel != null && !PlayerPrefs.HasKey("InterferencePuzzle_HelpShown"))
            {
                helpPanel.SetActive(true);
                PlayerPrefs.SetInt("InterferencePuzzle_HelpShown", 1);
                PlayerPrefs.Save();
            }
        }
    }
}
