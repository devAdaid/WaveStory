using UnityEngine;
using WaveStory.Interference;

/// <summary>
/// 다이얼로그에서 간섭 패턴 퍼즐을 열기 위한 커맨드
/// 사용법: InterferencePuzzle,퍼즐이름
/// 예: InterferencePuzzle,보강 간섭 퍼즐
/// </summary>
[DialogueCommand("InterferencePuzzle")]
public class InterferencePuzzleCommand : DialogueCommandBase
{
    private string puzzleName;
    private InterferencePuzzleData puzzleData;

    public override bool IsWaitingInput => true; // 퍼즐이 클리어될 때까지 대기

    public override void Initialize(string[] parameters)
    {
        if (parameters.Length > 0)
        {
            puzzleName = parameters[0];
            // Resources에서 퍼즐 데이터 로드
            puzzleData = Resources.Load<InterferencePuzzleData>($"InterferencePuzzle/{puzzleName}");

            if (puzzleData == null)
            {
                // 기본 퍼즐 로드 시도
                puzzleData = Resources.Load<InterferencePuzzleData>("InterferencePuzzle/InterferencePuzzle");
            }
        }
    }

    public override void Execute(IDialogueRuntime r)
    {
        var presenter = GM.I?.UIHolder?.InterferencePuzzlePresenter;
        if (presenter == null)
        {
            Debug.LogError("InterferencePuzzlePresenter를 찾을 수 없습니다.");
            r.ProcessNextCommand(); // 에러 시 다음 커맨드로 진행
            return;
        }

        if (puzzleData == null)
        {
            Debug.LogError($"퍼즐 데이터를 찾을 수 없습니다: {puzzleName}");
            r.ProcessNextCommand();
            return;
        }

        // 퍼즐 로드 및 표시
        presenter.LoadPuzzle(puzzleData);
        presenter.ShowPuzzle();

        // 퍼즐 클리어 시 다음 커맨드로 진행
        var context = GM.I.InterferencePuzzle;
        context.OnPuzzleSolved.RemoveAllListeners();
        context.OnPuzzleSolved.AddListener(() =>
        {
            // 퍼즐 클리어 후 잠시 대기했다가 닫기
            presenter.HidePuzzle();
            r.ProcessNextCommand();
        });
    }
}

/// <summary>
/// 간섭 패턴 퍼즐 결과를 확인하는 커맨드
/// 사용법: CheckInterferencePuzzle,성공라벨,실패라벨
/// </summary>
[DialogueCommand("CheckInterferencePuzzle")]
public class CheckInterferencePuzzleCommand : DialogueCommandBase
{
    private string successLabel;
    private string failLabel;

    public override bool IsWaitingInput => false;

    public override void Initialize(string[] parameters)
    {
        if (parameters.Length >= 2)
        {
            successLabel = parameters[0];
            failLabel = parameters[1];
        }
    }

    public override void Execute(IDialogueRuntime r)
    {
        var context = GM.I?.InterferencePuzzle;
        if (context != null && context.IsPuzzleSolved)
        {
            r.JumpToLabel(successLabel);
        }
        else
        {
            r.JumpToLabel(failLabel);
        }
    }
}
