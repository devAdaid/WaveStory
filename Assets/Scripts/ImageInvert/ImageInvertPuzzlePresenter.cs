using UnityEngine;

public class ImageInvertPuzzlePresenter : MonoBehaviour
{
    [SerializeField] private ImageInvertPuzzleUI puzzleUI;
    [SerializeField] private ImageInvertPuzzleData defaultPuzzle;

    private ImageInvertPuzzleContext context;

    public ImageInvertPuzzleContext Context => context;

    private void Awake()
    {
        context = new ImageInvertPuzzleContext();
    }

    private void Start()
    {
        if (puzzleUI != null)
        {
            puzzleUI.OnColumnButtonClicked += HandleColumnButtonClicked;
            puzzleUI.OnRowButtonClicked += HandleRowButtonClicked;
            puzzleUI.OnResetClicked += HandleResetClicked;
            puzzleUI.OnCloseClicked += HandleCloseClicked;
        }

        context.OnPuzzleLoaded.AddListener(HandlePuzzleLoaded);
        context.OnGridChanged.AddListener(HandleGridChanged);
        context.OnMovesChanged.AddListener(HandleMovesChanged);
        context.OnTimeChanged.AddListener(HandleTimeChanged);
        context.OnPuzzleSolved.AddListener(HandlePuzzleSolved);
        context.OnPuzzleFailed.AddListener(HandlePuzzleFailed);

        if (defaultPuzzle != null)
        {
            LoadPuzzle(defaultPuzzle);
        }
    }

    private void Update()
    {
        if (context != null && context.IsTimerActive &&
            !context.IsPuzzleSolved && !context.IsPuzzleFailed)
        {
            context.UpdateTime(Time.deltaTime);
        }
    }

    private void OnDestroy()
    {
        if (puzzleUI != null)
        {
            puzzleUI.OnColumnButtonClicked -= HandleColumnButtonClicked;
            puzzleUI.OnRowButtonClicked -= HandleRowButtonClicked;
            puzzleUI.OnResetClicked -= HandleResetClicked;
            puzzleUI.OnCloseClicked -= HandleCloseClicked;
        }

        context.OnPuzzleLoaded.RemoveListener(HandlePuzzleLoaded);
        context.OnGridChanged.RemoveListener(HandleGridChanged);
        context.OnMovesChanged.RemoveListener(HandleMovesChanged);
        context.OnTimeChanged.RemoveListener(HandleTimeChanged);
        context.OnPuzzleSolved.RemoveListener(HandlePuzzleSolved);
        context.OnPuzzleFailed.RemoveListener(HandlePuzzleFailed);
    }

    public void LoadPuzzle(ImageInvertPuzzleData puzzleData)
    {
        context.LoadPuzzle(puzzleData);
    }

    public void ShowPuzzle()
    {
        if (puzzleUI != null)
            puzzleUI.Show();
    }

    public void HidePuzzle()
    {
        if (puzzleUI != null)
            puzzleUI.Hide();
    }

    private void HandleColumnButtonClicked(int column)
    {
        context.ToggleColumn(column);
    }

    private void HandleRowButtonClicked(int row)
    {
        context.ToggleRow(row);
    }

    private void HandleResetClicked()
    {
        context.Reset();
    }

    private void HandleCloseClicked()
    {
        HidePuzzle();
    }

    private void HandlePuzzleLoaded()
    {
        if (puzzleUI == null || context.CurrentPuzzle == null) return;

        var puzzle = context.CurrentPuzzle;

        puzzleUI.Initialize();
        puzzleUI.UpdateGrid(context.CurrentGrid);
        puzzleUI.UpdatePuzzleInfo(
            puzzle.puzzleName,
            context.RemainingMoves,
            puzzle.maxMoves
        );
        puzzleUI.UpdateTime(context.RemainingTime);
        puzzleUI.ResetIndicators();
        puzzleUI.SetButtonsInteractable(true);
    }

    private void HandleGridChanged()
    {
        if (puzzleUI == null) return;
        puzzleUI.UpdateGrid(context.CurrentGrid);
    }

    private void HandleMovesChanged()
    {
        if (puzzleUI == null || context.CurrentPuzzle == null) return;
        puzzleUI.UpdatePuzzleInfo(
            context.CurrentPuzzle.puzzleName,
            context.RemainingMoves,
            context.CurrentPuzzle.maxMoves
        );
    }

    private void HandleTimeChanged()
    {
        if (puzzleUI == null) return;
        puzzleUI.UpdateTime(context.RemainingTime);
    }

    private void HandlePuzzleSolved()
    {
        if (puzzleUI != null)
        {
            puzzleUI.ShowSolvedState(true);
            puzzleUI.SetButtonsInteractable(false);
        }

        Debug.Log("ImageInvert 퍼즐 해결!");
    }

    private void HandlePuzzleFailed()
    {
        if (puzzleUI != null)
        {
            puzzleUI.ShowFailedState(true);
            puzzleUI.SetButtonsInteractable(false);
        }

        Debug.Log("ImageInvert 퍼즐 실패!");
    }
}
