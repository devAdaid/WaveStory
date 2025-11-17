using UnityEngine;

namespace WaveStory.Interference
{
    /// <summary>
    /// 간섭 패턴 퍼즐을 테스트하기 위한 런타임 스크립트
    /// 씬에 추가하면 자동으로 퍼즐을 로드하고 시작합니다.
    /// </summary>
    public class InterferencePuzzleTestRunner : MonoBehaviour
    {
        [Header("퍼즐 설정")]
        [SerializeField] private InterferencePuzzlePresenter presenter;
        [SerializeField] private InterferencePuzzleData puzzleData;

        [Header("테스트 옵션")]
        [SerializeField] private bool autoShowOnStart = true;
        [SerializeField] private KeyCode toggleKey = KeyCode.P;
        [SerializeField] private KeyCode resetKey = KeyCode.R;

        private bool isVisible = false;

        private void Start()
        {
            if (presenter == null)
            {
                presenter = FindFirstObjectByType<InterferencePuzzlePresenter>();
            }

            if (presenter == null)
            {
                Debug.LogError("InterferencePuzzlePresenter를 찾을 수 없습니다.");
                return;
            }

            // Resources에서 퍼즐 데이터 로드
            if (puzzleData == null)
            {
                puzzleData = Resources.Load<InterferencePuzzleData>("InterferencePuzzle/InterferencePuzzle");
            }

            if (puzzleData != null)
            {
                presenter.LoadPuzzle(puzzleData);
            }
            else
            {
                Debug.LogWarning("퍼즐 데이터를 찾을 수 없습니다. Resources/InterferencePuzzle/ 폴더를 확인하세요.");
            }

            if (autoShowOnStart)
            {
                ShowPuzzle();
            }
            else
            {
                HidePuzzle();
            }

            Debug.Log($"간섭 패턴 퍼즐 테스트 시작\n" +
                      $"  - {toggleKey} 키: 퍼즐 표시/숨기기 토글\n" +
                      $"  - {resetKey} 키: 퍼즐 리셋\n" +
                      $"  - 그리드 클릭: 파동 소스 배치/제거");
        }

        private void Update()
        {
            if (presenter == null) return;

            if (Input.GetKeyDown(toggleKey))
            {
                if (isVisible)
                    HidePuzzle();
                else
                    ShowPuzzle();
            }

            if (Input.GetKeyDown(resetKey) && isVisible)
            {
                if (puzzleData != null)
                {
                    presenter.LoadPuzzle(puzzleData);
                    Debug.Log("퍼즐이 리셋되었습니다.");
                }
            }
        }

        private void ShowPuzzle()
        {
            presenter.ShowPuzzle();
            isVisible = true;
        }

        private void HidePuzzle()
        {
            presenter.HidePuzzle();
            isVisible = false;
        }

        /// <summary>
        /// GM 시스템과 통합하여 사용할 때 호출
        /// </summary>
        public void OpenPuzzleFromGM()
        {
            if (GM.I != null)
            {
                // GM의 InterferencePuzzle 컨텍스트 사용
                var context = GM.I.InterferencePuzzle;
                if (puzzleData != null)
                {
                    context.LoadPuzzle(puzzleData);
                }
                ShowPuzzle();
            }
        }
    }
}
