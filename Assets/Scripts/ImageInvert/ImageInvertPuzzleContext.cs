using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace WaveStory.ImageInvert
{
    public class ImageInvertPuzzleContext
    {
        public ImageInvertPuzzleData CurrentPuzzle { get; private set; }
        public bool[,] CurrentGrid { get; private set; }
        public bool[,] AnswerGrid { get; private set; }
        public HashSet<int> InvertedRows { get; private set; } = new HashSet<int>();
        public HashSet<int> InvertedColumns { get; private set; } = new HashSet<int>();
        public int RemainingMoves { get; private set; }
        public float RemainingTime { get; private set; }
        public bool IsPuzzleSolved { get; private set; }
        public bool IsPuzzleFailed { get; private set; }
        public bool IsTimerActive { get; private set; }

        public UnityEvent OnPuzzleLoaded = new UnityEvent();
        public UnityEvent OnGridChanged = new UnityEvent();
        public UnityEvent OnMovesChanged = new UnityEvent();
        public UnityEvent OnTimeChanged = new UnityEvent();
        public UnityEvent OnPuzzleSolved = new UnityEvent();
        public UnityEvent OnPuzzleFailed = new UnityEvent();

        public void LoadPuzzle(ImageInvertPuzzleData puzzleData)
        {
            CurrentPuzzle = puzzleData;
            AnswerGrid = puzzleData.GetBitmapAs2D();
            CurrentGrid = puzzleData.GetBitmapAs2D();
            InvertedRows.Clear();
            InvertedColumns.Clear();
            RemainingMoves = puzzleData.maxMoves;
            RemainingTime = puzzleData.timeLimit;
            IsPuzzleSolved = false;
            IsPuzzleFailed = false;
            IsTimerActive = puzzleData.timeLimit > 0;

            ApplyRandomInversions(puzzleData.initialInvertCount);

            OnPuzzleLoaded.Invoke();
            OnGridChanged.Invoke();
            OnMovesChanged.Invoke();
            OnTimeChanged.Invoke();
        }

        private void ApplyRandomInversions(int count)
        {
            // 행과 열을 랜덤하게 선택하여 인버트
            List<int> availableRows = new List<int>();
            List<int> availableColumns = new List<int>();

            for (int i = 0; i < 12; i++)
            {
                availableRows.Add(i);
                availableColumns.Add(i);
            }

            // 셔플
            ShuffleList(availableRows);
            ShuffleList(availableColumns);

            // count개의 행/열을 랜덤하게 선택 (행과 열 중 랜덤하게)
            int inversionsApplied = 0;
            int rowIndex = 0;
            int colIndex = 0;

            while (inversionsApplied < count && (rowIndex < 12 || colIndex < 12))
            {
                bool invertRow = Random.value > 0.5f;

                if (invertRow && rowIndex < 12)
                {
                    int row = availableRows[rowIndex++];
                    InvertRow(row);
                    InvertedRows.Add(row);
                    inversionsApplied++;
                }
                else if (!invertRow && colIndex < 12)
                {
                    int col = availableColumns[colIndex++];
                    InvertColumn(col);
                    InvertedColumns.Add(col);
                    inversionsApplied++;
                }
                else if (rowIndex < 12)
                {
                    int row = availableRows[rowIndex++];
                    InvertRow(row);
                    InvertedRows.Add(row);
                    inversionsApplied++;
                }
                else if (colIndex < 12)
                {
                    int col = availableColumns[colIndex++];
                    InvertColumn(col);
                    InvertedColumns.Add(col);
                    inversionsApplied++;
                }
            }
        }

        private void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        private void InvertRow(int row)
        {
            for (int x = 0; x < 12; x++)
            {
                CurrentGrid[x, row] = !CurrentGrid[x, row];
            }
        }

        private void InvertColumn(int col)
        {
            for (int y = 0; y < 12; y++)
            {
                CurrentGrid[col, y] = !CurrentGrid[col, y];
            }
        }

        public bool ToggleRow(int row)
        {
            if (IsPuzzleSolved || IsPuzzleFailed) return false;
            if (RemainingMoves <= 0) return false;
            if (row < 0 || row >= 12) return false;

            InvertRow(row);

            // 인버트 상태 토글
            if (InvertedRows.Contains(row))
                InvertedRows.Remove(row);
            else
                InvertedRows.Add(row);

            RemainingMoves--;
            OnMovesChanged.Invoke();
            OnGridChanged.Invoke();
            CheckSolution();

            return true;
        }

        public bool ToggleColumn(int col)
        {
            if (IsPuzzleSolved || IsPuzzleFailed) return false;
            if (RemainingMoves <= 0) return false;
            if (col < 0 || col >= 12) return false;

            InvertColumn(col);

            // 인버트 상태 토글
            if (InvertedColumns.Contains(col))
                InvertedColumns.Remove(col);
            else
                InvertedColumns.Add(col);

            RemainingMoves--;
            OnMovesChanged.Invoke();
            OnGridChanged.Invoke();
            CheckSolution();

            return true;
        }

        public void UpdateTime(float deltaTime)
        {
            if (!IsTimerActive || IsPuzzleSolved || IsPuzzleFailed) return;

            RemainingTime -= deltaTime;
            OnTimeChanged.Invoke();

            if (RemainingTime <= 0)
            {
                RemainingTime = 0;
                IsPuzzleFailed = true;
                OnPuzzleFailed.Invoke();
            }
        }

        private void CheckSolution()
        {
            if (IsPuzzleFailed) return;

            // 남은 횟수 체크
            if (RemainingMoves <= 0 && !IsPuzzleSolved)
            {
                bool isSolved = CompareGrids();
                if (!isSolved)
                {
                    IsPuzzleFailed = true;
                    OnPuzzleFailed.Invoke();
                    return;
                }
            }

            // 정답 체크
            if (CompareGrids())
            {
                IsPuzzleSolved = true;
                OnPuzzleSolved.Invoke();
            }
        }

        private bool CompareGrids()
        {
            for (int x = 0; x < 12; x++)
            {
                for (int y = 0; y < 12; y++)
                {
                    if (CurrentGrid[x, y] != AnswerGrid[x, y])
                        return false;
                }
            }
            return true;
        }

        public void Reset()
        {
            if (CurrentPuzzle != null)
            {
                LoadPuzzle(CurrentPuzzle);
            }
        }
    }
}
