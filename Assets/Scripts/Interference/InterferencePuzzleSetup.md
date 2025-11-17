# 간섭 패턴 퍼즐 설정 가이드

## 빠른 설정 (자동화 도구 사용)

Unity 메뉴에서 **WaveStory → Setup Interference Puzzle**을 선택하면 자동으로 설정할 수 있습니다.

1. Cell Prefab 생성 버튼 클릭
2. 씬에 Canvas가 있는지 확인
3. 전체 퍼즐 UI 생성 버튼 클릭
4. GameUIHolder에 InterferencePuzzlePresenter 참조 연결

---

## 1. Cell Prefab 생성 (자동)

**WaveStory → Setup Interference Puzzle** 메뉴에서 "Cell Prefab 생성" 버튼 클릭

또는 수동으로:
1. Hierarchy에서 우클릭 → UI → Image 생성
2. 이름을 "PuzzleCell"로 변경
3. Button 컴포넌트 추가
4. RectTransform 설정:
   - Width: 40, Height: 40 (또는 원하는 크기)
5. Assets/Prefabs/UI/Interference 폴더에 드래그해서 프리팹화
6. Hierarchy에서 삭제

## 2. Puzzle UI 설정 (자동)

**WaveStory → Setup Interference Puzzle** 메뉴에서 "씬에 전체 퍼즐 UI 생성" 버튼 클릭

자동으로 생성되는 요소:
- InterferencePuzzleUI (메인 컨테이너)
- GridContainer (그리드 셀 컨테이너)
- 정보 텍스트 (퍼즐 이름, 정확도, 소스 개수)
- 버튼 (초기화, 닫기)
- 클리어 표시
- InterferencePuzzlePresenter

## 3. GameUIHolder 연결

Main 씬의 GameUIHolder 컴포넌트에서 InterferencePuzzlePresenter 필드에 생성된 Presenter를 연결하세요.

## 4. 퍼즐 데이터 생성

**이미 샘플 데이터가 준비되어 있습니다:**
`Resources/InterferencePuzzle/InterferencePuzzle.asset`

새 퍼즐 생성:
1. Project 창에서 우클릭 → Create → WaveStory → Interference Puzzle
2. Inspector에서 설정:
   - Puzzle Name: "테스트 퍼즐"
   - Grid Width/Height: 10x10
   - Max Sources: 3
   - Default Amplitude: 1
   - Default Frequency: 1

3. Target Points 추가:
   - **Constructive** 타입: 보강 간섭이 필요한 위치 (높은 진폭)
   - **Destructive** 타입: 상쇄 간섭이 필요한 위치 (낮은 진폭)

4. Required Accuracy: 0.7 (70% 이상 정확도로 클리어)

## 5. 게임플레이

- 그리드의 셀을 클릭하면 파동 소스 배치/제거
- 파동 소스는 **노란색**으로 표시
- 목표 지점:
  - **녹색**: 보강 간섭 필요 (높은 강도)
  - **청록색**: 상쇄 간섭 필요 (낮은 강도)
- 간섭 패턴이 실시간으로 표시 (파란색→빨간색 그라데이션)
- 정확도가 요구치를 넘으면 자동으로 클리어

## 6. 테스트 실행

### 방법 1: TestRunner 사용
1. 씬에 빈 GameObject 생성
2. `InterferencePuzzleTestRunner` 스크립트 추가
3. Presenter 참조 연결
4. Play 모드에서:
   - **P 키**: 퍼즐 표시/숨기기
   - **R 키**: 퍼즐 리셋

### 방법 2: 다이얼로그에서 호출
CSV 다이얼로그 파일에서:
```
InterferencePuzzle,보강 간섭 퍼즐
```

## 7. 게임에 통합 (이미 완료됨)

GM 시스템에 이미 통합되어 있습니다:

```csharp
// GM.cs
public InterferencePuzzleContext InterferencePuzzle { get; private set; }

// GameUIHolder.cs
public InterferencePuzzlePresenter InterferencePuzzlePresenter;
```

### 다이얼로그 커맨드 사용

```csv
// 퍼즐 열기
InterferencePuzzle,퍼즐이름

// 결과 확인
CheckInterferencePuzzle,성공라벨,실패라벨
```

### 코드에서 직접 호출

```csharp
// 예: SoulInteractable에서
var presenter = GM.I.UIHolder.InterferencePuzzlePresenter;
var puzzleData = Resources.Load<InterferencePuzzleData>("InterferencePuzzle/퍼즐이름");
presenter.LoadPuzzle(puzzleData);
presenter.ShowPuzzle();

// 클리어 이벤트 구독
GM.I.InterferencePuzzle.OnPuzzleSolved.AddListener(() => {
    Debug.Log("퍼즐 클리어!");
    // 보상 처리...
});
```

## 8. 추가 기능

### 파동 애니메이션
`InterferencePuzzleAnimator` 컴포넌트를 추가하면 실시간 파동 애니메이션을 볼 수 있습니다:
```csharp
animator.Initialize(gridCells, context, width, height);
animator.SetAnimationSpeed(1.0f);
animator.ToggleAnimation(true);
```

### 새로운 퍼즐 스테이지 추가
1. `Resources/InterferencePuzzle/` 폴더에 새 ScriptableObject 생성
2. 고유한 이름 지정
3. 타겟 포인트와 난이도 조절
