using UnityEngine;

namespace WaveStory.Interference
{
    public class InterferencePuzzlePresenter : MonoBehaviour
    {
        [SerializeField] private InterferencePuzzleUI puzzleUI;
        [SerializeField] private InterferencePuzzleData defaultPuzzle;

        private InterferencePuzzleContext context;

        private void Awake()
        {
            context = new InterferencePuzzleContext();
        }

        private void Start()
        {
            if (puzzleUI != null)
            {
                puzzleUI.OnCellClicked += HandleCellClicked;
                puzzleUI.OnClearClicked += HandleClearClicked;
                puzzleUI.OnCloseClicked += HandleCloseClicked;
            }

            context.OnPuzzleLoaded.AddListener(HandlePuzzleLoaded);
            context.OnSourcesChanged.AddListener(HandleSourcesChanged);
            context.OnPuzzleSolved.AddListener(HandlePuzzleSolved);

            if (defaultPuzzle != null)
            {
                LoadPuzzle(defaultPuzzle);
            }
        }

        private void OnDestroy()
        {
            if (puzzleUI != null)
            {
                puzzleUI.OnCellClicked -= HandleCellClicked;
                puzzleUI.OnClearClicked -= HandleClearClicked;
                puzzleUI.OnCloseClicked -= HandleCloseClicked;
            }

            context.OnPuzzleLoaded.RemoveListener(HandlePuzzleLoaded);
            context.OnSourcesChanged.RemoveListener(HandleSourcesChanged);
            context.OnPuzzleSolved.RemoveListener(HandlePuzzleSolved);
        }

        public void LoadPuzzle(InterferencePuzzleData puzzleData)
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

        private void HandleCellClicked(Vector2Int position)
        {
            // 이미 소스가 있으면 제거, 없으면 추가
            bool removed = context.TryRemoveSource(position);
            if (!removed)
            {
                context.TryAddSource(position);
            }
        }

        private void HandleClearClicked()
        {
            context.ClearAllSources();
        }

        private void HandleCloseClicked()
        {
            HidePuzzle();
        }

        private void HandlePuzzleLoaded()
        {
            if (puzzleUI == null || context.CurrentPuzzle == null) return;

            var puzzle = context.CurrentPuzzle;

            puzzleUI.Initialize(puzzle.gridWidth, puzzle.gridHeight);
            puzzleUI.ShowTargetPoints(puzzle.targetPoints);
            puzzleUI.UpdatePuzzleInfo(
                puzzle.puzzleName,
                0f,
                0,
                puzzle.maxSources
            );
            puzzleUI.ShowSolvedState(false);

            UpdateInterferenceDisplay();

            // 첫 플레이 시 도움말 자동 표시
            puzzleUI.ShowHelpOnFirstPlay();
        }

        private void HandleSourcesChanged()
        {
            UpdateInterferenceDisplay();
            UpdateUI();
        }

        private void HandlePuzzleSolved()
        {
            if (puzzleUI != null)
            {
                puzzleUI.ShowSolvedState(true);
            }

            Debug.Log("퍼즐 해결!");
            // 여기에 클리어 보상 로직 추가 가능
        }

        private void UpdateInterferenceDisplay()
        {
            if (puzzleUI == null || context.CurrentPuzzle == null) return;

            var puzzle = context.CurrentPuzzle;
            var pattern = InterferenceLogic.CalculateInterferencePattern(
                puzzle.gridWidth,
                puzzle.gridHeight,
                context.PlacedSources
            );

            puzzleUI.UpdateInterferencePattern(pattern);
            puzzleUI.ShowSourcePositions(context.PlacedSources);
            puzzleUI.UpdateTargetStatus(context.PlacedSources);
        }

        private void UpdateUI()
        {
            if (puzzleUI == null || context.CurrentPuzzle == null) return;

            var puzzle = context.CurrentPuzzle;
            puzzleUI.UpdatePuzzleInfo(
                puzzle.puzzleName,
                context.GetCurrentAccuracy(),
                context.PlacedSources.Count,
                puzzle.maxSources
            );
        }
    }
}
