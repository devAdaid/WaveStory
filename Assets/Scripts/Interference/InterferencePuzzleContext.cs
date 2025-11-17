using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace WaveStory.Interference
{
    public class InterferencePuzzleContext
    {
        public InterferencePuzzleData CurrentPuzzle { get; private set; }
        public List<WaveSource> PlacedSources { get; private set; } = new List<WaveSource>();
        public bool IsPuzzleSolved { get; private set; }

        public UnityEvent OnPuzzleLoaded = new UnityEvent();
        public UnityEvent OnSourcesChanged = new UnityEvent();
        public UnityEvent OnPuzzleSolved = new UnityEvent();

        public void LoadPuzzle(InterferencePuzzleData puzzleData)
        {
            CurrentPuzzle = puzzleData;
            PlacedSources.Clear();
            IsPuzzleSolved = false;
            OnPuzzleLoaded.Invoke();
        }

        public bool TryAddSource(Vector2Int position)
        {
            if (CurrentPuzzle == null) return false;
            if (PlacedSources.Count >= CurrentPuzzle.maxSources) return false;
            if (IsPositionOccupied(position)) return false;

            var newSource = new WaveSource(
                position,
                CurrentPuzzle.defaultAmplitude,
                CurrentPuzzle.defaultFrequency
            );

            PlacedSources.Add(newSource);
            OnSourcesChanged.Invoke();
            CheckSolution();
            return true;
        }

        public bool TryRemoveSource(Vector2Int position)
        {
            var index = PlacedSources.FindIndex(s => s.position == position);
            if (index < 0) return false;

            PlacedSources.RemoveAt(index);
            OnSourcesChanged.Invoke();
            CheckSolution();
            return true;
        }

        public void UpdateSource(int index, WaveSource updatedSource)
        {
            if (index < 0 || index >= PlacedSources.Count) return;

            PlacedSources[index] = updatedSource;
            OnSourcesChanged.Invoke();
            CheckSolution();
        }

        public void ClearAllSources()
        {
            PlacedSources.Clear();
            IsPuzzleSolved = false;
            OnSourcesChanged.Invoke();
        }

        private bool IsPositionOccupied(Vector2Int position)
        {
            return PlacedSources.Exists(s => s.position == position);
        }

        private void CheckSolution()
        {
            if (CurrentPuzzle == null || CurrentPuzzle.targetPoints.Count == 0)
            {
                IsPuzzleSolved = false;
                return;
            }

            float totalAccuracy = 0f;
            foreach (var target in CurrentPuzzle.targetPoints)
            {
                float intensity = InterferenceLogic.CalculateIntensityAt(
                    target.position,
                    PlacedSources
                );

                float accuracy = InterferenceLogic.CalculateTargetAccuracy(target, intensity);
                totalAccuracy += accuracy;
            }

            float averageAccuracy = totalAccuracy / CurrentPuzzle.targetPoints.Count;
            bool wasSolved = IsPuzzleSolved;
            IsPuzzleSolved = averageAccuracy >= CurrentPuzzle.requiredAccuracy;

            if (IsPuzzleSolved && !wasSolved)
            {
                OnPuzzleSolved.Invoke();
            }
        }

        public float GetCurrentAccuracy()
        {
            if (CurrentPuzzle == null || CurrentPuzzle.targetPoints.Count == 0)
                return 0f;

            float totalAccuracy = 0f;
            foreach (var target in CurrentPuzzle.targetPoints)
            {
                float intensity = InterferenceLogic.CalculateIntensityAt(
                    target.position,
                    PlacedSources
                );
                totalAccuracy += InterferenceLogic.CalculateTargetAccuracy(target, intensity);
            }

            return totalAccuracy / CurrentPuzzle.targetPoints.Count;
        }
    }
}
