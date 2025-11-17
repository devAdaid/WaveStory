# 간섭 패턴 퍼즐 설정 가이드

## 1. Cell Prefab 생성

1. Hierarchy에서 우클릭 → UI → Image 생성
2. 이름을 "PuzzleCell"로 변경
3. Button 컴포넌트 추가
4. RectTransform 설정:
   - Width: 40, Height: 40 (또는 원하는 크기)
5. Assets/Prefabs 폴더에 드래그해서 프리팹화
6. Hierarchy에서 삭제

## 2. Puzzle UI 설정

1. Canvas 아래에 빈 GameObject 생성, 이름: "InterferencePuzzleUI"
2. InterferencePuzzleUI 스크립트 연결
3. 자식으로 다음 UI 요소 추가:

### Grid Container
- 빈 GameObject, RectTransform만 있으면 됨
- 이름: "GridContainer"
- Anchor: 중앙

### 정보 텍스트들
- PuzzleNameText (TextMeshPro)
- AccuracyText (TextMeshPro)
- SourceCountText (TextMeshPro)

### 버튼들
- ClearButton (Button)
- CloseButton (Button)

### 클리어 표시
- SolvedIndicator (Image 또는 Panel)
- 기본적으로 비활성화

4. InterferencePuzzleUI Inspector에서 모든 참조 연결

## 3. Presenter 설정

1. 빈 GameObject 생성, 이름: "InterferencePuzzlePresenter"
2. InterferencePuzzlePresenter 스크립트 연결
3. PuzzleUI 참조 연결
4. DefaultPuzzle에 테스트용 퍼즐 데이터 연결

## 4. 퍼즐 데이터 생성

1. Project 창에서 우클릭 → Create → WaveStory → Interference Puzzle
2. Inspector에서 설정:
   - Puzzle Name: "테스트 퍼즐"
   - Grid Width/Height: 8x8
   - Max Sources: 3
   - Default Amplitude: 1
   - Default Frequency: 1

3. Target Points 추가:
   - Constructive 타입: 보강 간섭이 필요한 위치 (높은 진폭)
   - Destructive 타입: 상쇄 간섭이 필요한 위치 (낮은 진폭)

4. Required Accuracy: 0.8 (80% 이상 정확도로 클리어)

## 5. 게임플레이

- 그리드의 셀을 클릭하면 파동 소스 배치/제거
- 파동 소스는 노란색으로 표시
- 목표 지점:
  - 녹색: 보강 간섭 필요 (높은 강도)
  - 청록색: 상쇄 간섭 필요 (낮은 강도)
- 간섭 패턴이 실시간으로 표시 (파란색→빨간색 그라데이션)
- 정확도가 요구치를 넘으면 자동으로 클리어

## 6. 게임에 통합

기존 GM 시스템과 연동하려면:

```csharp
// GM.cs에 추가
public InterferencePuzzleContext InterferencePuzzle { get; private set; }

// Initialize()에서
InterferencePuzzle = new InterferencePuzzleContext();
```

Soul이나 다른 상호작용 오브젝트에서 퍼즐을 열도록 연결:

```csharp
// 예: SoulInteractable에서
interferencePuzzlePresenter.LoadPuzzle(puzzleData);
interferencePuzzlePresenter.ShowPuzzle();
```
