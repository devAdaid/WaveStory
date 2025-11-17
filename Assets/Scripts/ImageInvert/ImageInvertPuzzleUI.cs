using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ImageInvertPuzzleUI : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private List<Image> gridCells = new List<Image>();

    [Header("Invert Buttons")]
    [SerializeField] private List<Button> columnButtons = new List<Button>();
    [SerializeField] private List<Button> rowButtons = new List<Button>();

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI puzzleNameText;
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject solvedIndicator;
    [SerializeField] private GameObject failedIndicator;

    [Header("Colors")]
    [SerializeField] private Color onColor = Color.white;
    [SerializeField] private Color offColor = Color.black;

    public System.Action<int> OnColumnButtonClicked;
    public System.Action<int> OnRowButtonClicked;
    public System.Action OnResetClicked;
    public System.Action OnCloseClicked;

    private void Awake()
    {
        if (resetButton != null)
            resetButton.onClick.AddListener(() => OnResetClicked?.Invoke());

        if (closeButton != null)
            closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
    }

    public void Initialize()
    {
        SetupButtonEvents();
        ResetGridColors();
    }

    private void SetupButtonEvents()
    {
        for (int col = 0; col < columnButtons.Count; col++)
        {
            var button = columnButtons[col];
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                int capturedCol = col;
                button.onClick.AddListener(() => OnColumnButtonClicked?.Invoke(capturedCol));
            }
        }

        for (int row = 0; row < rowButtons.Count; row++)
        {
            var button = rowButtons[row];
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                int capturedRow = row;
                button.onClick.AddListener(() => OnRowButtonClicked?.Invoke(capturedRow));
            }
        }
    }

    private void ResetGridColors()
    {
        foreach (var cell in gridCells)
        {
            if (cell != null)
            {
                cell.color = offColor;
            }
        }
    }

    public void UpdateGrid(bool[,] grid)
    {
        if (grid == null) return;

        for (int y = 0; y < 12; y++)
        {
            for (int x = 0; x < 12; x++)
            {
                int index = y * 12 + x;
                if (index < gridCells.Count)
                {
                    gridCells[index].color = grid[x, y] ? onColor : offColor;
                }
            }
        }
    }

    public void UpdatePuzzleInfo(string puzzleName, int remainingMoves, int maxMoves)
    {
        if (puzzleNameText != null)
            puzzleNameText.text = puzzleName;

        if (movesText != null)
            movesText.text = $"남은 횟수: {remainingMoves}/{maxMoves}";
    }

    public void UpdateTime(float remainingTime)
    {
        if (timeText != null)
        {
            if (remainingTime <= 0)
            {
                timeText.text = "시간 초과";
                timeText.color = Color.red;
            }
            else
            {
                int minutes = Mathf.FloorToInt(remainingTime / 60f);
                int seconds = Mathf.FloorToInt(remainingTime % 60f);
                timeText.text = $"남은 시간: {minutes:00}:{seconds:00}";

                timeText.color = remainingTime < 10f ? Color.red : Color.white;
            }
        }
    }

    public void ShowSolvedState(bool solved)
    {
        if (solvedIndicator != null)
            solvedIndicator.SetActive(solved);

        if (failedIndicator != null)
            failedIndicator.SetActive(false);
    }

    public void ShowFailedState(bool failed)
    {
        if (failedIndicator != null)
            failedIndicator.SetActive(failed);

        if (solvedIndicator != null)
            solvedIndicator.SetActive(false);
    }

    public void SetButtonsInteractable(bool interactable)
    {
        foreach (var button in columnButtons)
        {
            if (button != null)
                button.interactable = interactable;
        }

        foreach (var button in rowButtons)
        {
            if (button != null)
                button.interactable = interactable;
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ResetIndicators()
    {
        if (solvedIndicator != null)
            solvedIndicator.SetActive(false);

        if (failedIndicator != null)
            failedIndicator.SetActive(false);

        if (timeText != null)
            timeText.color = Color.white;
    }
}