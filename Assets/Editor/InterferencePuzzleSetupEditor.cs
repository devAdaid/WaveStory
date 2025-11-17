using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using WaveStory.Interference;
using System.Linq;

namespace WaveStory.Editor
{
    public class InterferencePuzzleSetupEditor : EditorWindow
    {
        private InterferencePuzzleData defaultPuzzleData;

        [MenuItem("WaveStory/Setup Interference Puzzle")]
        public static void ShowWindow()
        {
            GetWindow<InterferencePuzzleSetupEditor>("Interference Puzzle Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("간섭 패턴 퍼즐 설정", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            defaultPuzzleData = (InterferencePuzzleData)EditorGUILayout.ObjectField(
                "기본 퍼즐 데이터", defaultPuzzleData, typeof(InterferencePuzzleData), false);

            EditorGUILayout.Space();

            if (GUILayout.Button("1. Cell Prefab 생성", GUILayout.Height(30)))
            {
                CreateCellPrefab();
            }

            if (GUILayout.Button("2. 씬에 전체 퍼즐 UI 생성", GUILayout.Height(30)))
            {
                CreatePuzzleUIInScene();
            }

            EditorGUILayout.Space();
            GUILayout.Label("레이아웃 설정", EditorStyles.boldLabel);

            if (GUILayout.Button("3. 기존 UI에 LayoutGroup 적용", GUILayout.Height(30)))
            {
                ApplyLayoutGroupToExistingUI();
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "사용법:\n" +
                "1. Cell Prefab 생성 버튼 클릭\n" +
                "2. 씬에 Canvas가 있는지 확인\n" +
                "3. 전체 퍼즐 UI 생성 버튼 클릭\n" +
                "4. Inspector에서 참조 연결 확인\n\n" +
                "레이아웃 수정:\n" +
                "• 기존 UI에 LayoutGroup 적용 버튼 클릭\n" +
                "• TopPanel, GridContainer, ButtonPanel이\n" +
                "  자동으로 정렬됩니다",
                MessageType.Info);
        }

        private void CreateCellPrefab()
        {
            // 임시 GameObject 생성
            var cellObj = new GameObject("PuzzleCell");

            // RectTransform 설정
            var rectTransform = cellObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(40f, 40f);

            // Image 컴포넌트 추가
            var image = cellObj.AddComponent<Image>();
            image.color = Color.black;

            // Button 컴포넌트 추가
            var button = cellObj.AddComponent<Button>();
            button.targetGraphic = image;

            // 프리팹 저장 경로
            string prefabPath = "Assets/Prefabs/UI/Interference";
            if (!AssetDatabase.IsValidFolder(prefabPath))
            {
                AssetDatabase.CreateFolder("Assets/Prefabs/UI", "Interference");
            }

            string fullPath = $"{prefabPath}/PuzzleCell.prefab";

            // 프리팹 생성
            PrefabUtility.SaveAsPrefabAsset(cellObj, fullPath);
            DestroyImmediate(cellObj);

            AssetDatabase.Refresh();
            Debug.Log($"Cell Prefab 생성 완료: {fullPath}");
            EditorUtility.DisplayDialog("완료", "Cell Prefab이 생성되었습니다.\n" + fullPath, "확인");
        }

        private void CreatePuzzleUIInScene()
        {
            // Canvas 찾기
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                EditorUtility.DisplayDialog("오류", "씬에 Canvas가 없습니다. Canvas를 먼저 생성하세요.", "확인");
                return;
            }

            // Cell Prefab 찾기
            string cellPrefabPath = "Assets/Prefabs/UI/Interference/PuzzleCell.prefab";
            var cellPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(cellPrefabPath);
            if (cellPrefab == null)
            {
                EditorUtility.DisplayDialog("오류", "Cell Prefab을 먼저 생성하세요.", "확인");
                return;
            }

            // 메인 UI 컨테이너 생성
            var puzzleUIObj = new GameObject("InterferencePuzzleUI");
            puzzleUIObj.transform.SetParent(canvas.transform, false);

            var puzzleUIRect = puzzleUIObj.AddComponent<RectTransform>();
            puzzleUIRect.anchorMin = Vector2.zero;
            puzzleUIRect.anchorMax = Vector2.one;
            puzzleUIRect.offsetMin = Vector2.zero;
            puzzleUIRect.offsetMax = Vector2.zero;

            // 배경 패널
            var bgImage = puzzleUIObj.AddComponent<Image>();
            bgImage.color = new Color(0, 0, 0, 0.8f);

            // InterferencePuzzleUI 스크립트 추가
            var puzzleUIScript = puzzleUIObj.AddComponent<InterferencePuzzleUI>();

            // Grid Container 생성
            var gridContainer = CreateUIElement("GridContainer", puzzleUIObj.transform);
            var gridRect = gridContainer.GetComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0.5f, 0.5f);
            gridRect.anchorMax = new Vector2(0.5f, 0.5f);
            gridRect.pivot = new Vector2(0.5f, 0.5f);
            gridRect.sizeDelta = new Vector2(400f, 400f);
            gridRect.anchoredPosition = new Vector2(0, 50);

            // 정보 텍스트 - 상단 패널
            var topPanel = CreateUIElement("TopPanel", puzzleUIObj.transform);
            var topPanelRect = topPanel.GetComponent<RectTransform>();
            topPanelRect.anchorMin = new Vector2(0.5f, 1f);
            topPanelRect.anchorMax = new Vector2(0.5f, 1f);
            topPanelRect.pivot = new Vector2(0.5f, 1f);
            topPanelRect.sizeDelta = new Vector2(500f, 100f);
            topPanelRect.anchoredPosition = new Vector2(0, -20);

            var puzzleNameText = CreateTextElement("PuzzleNameText", topPanel.transform, "퍼즐 이름", 24);
            var nameRect = puzzleNameText.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0.5f, 1f);
            nameRect.anchorMax = new Vector2(0.5f, 1f);
            nameRect.pivot = new Vector2(0.5f, 1f);
            nameRect.anchoredPosition = new Vector2(0, -10);

            var accuracyText = CreateTextElement("AccuracyText", topPanel.transform, "정확도: 0%", 18);
            var accRect = accuracyText.GetComponent<RectTransform>();
            accRect.anchorMin = new Vector2(0.5f, 1f);
            accRect.anchorMax = new Vector2(0.5f, 1f);
            accRect.pivot = new Vector2(0.5f, 1f);
            accRect.anchoredPosition = new Vector2(0, -45);

            var sourceCountText = CreateTextElement("SourceCountText", topPanel.transform, "파동 소스: 0/3", 18);
            var srcRect = sourceCountText.GetComponent<RectTransform>();
            srcRect.anchorMin = new Vector2(0.5f, 1f);
            srcRect.anchorMax = new Vector2(0.5f, 1f);
            srcRect.pivot = new Vector2(0.5f, 1f);
            srcRect.anchoredPosition = new Vector2(0, -70);

            // 버튼 패널
            var buttonPanel = CreateUIElement("ButtonPanel", puzzleUIObj.transform);
            var btnPanelRect = buttonPanel.GetComponent<RectTransform>();
            btnPanelRect.anchorMin = new Vector2(0.5f, 0f);
            btnPanelRect.anchorMax = new Vector2(0.5f, 0f);
            btnPanelRect.pivot = new Vector2(0.5f, 0f);
            btnPanelRect.sizeDelta = new Vector2(400f, 60f);
            btnPanelRect.anchoredPosition = new Vector2(0, 30);

            var clearButton = CreateButton("ClearButton", buttonPanel.transform, "초기화");
            var clearRect = clearButton.GetComponent<RectTransform>();
            clearRect.anchorMin = new Vector2(0f, 0.5f);
            clearRect.anchorMax = new Vector2(0f, 0.5f);
            clearRect.pivot = new Vector2(0f, 0.5f);
            clearRect.sizeDelta = new Vector2(120f, 40f);
            clearRect.anchoredPosition = new Vector2(50, 0);

            var closeButton = CreateButton("CloseButton", buttonPanel.transform, "닫기");
            var closeRect = closeButton.GetComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(1f, 0.5f);
            closeRect.anchorMax = new Vector2(1f, 0.5f);
            closeRect.pivot = new Vector2(1f, 0.5f);
            closeRect.sizeDelta = new Vector2(120f, 40f);
            closeRect.anchoredPosition = new Vector2(-50, 0);

            // 클리어 표시
            var solvedIndicator = CreateUIElement("SolvedIndicator", puzzleUIObj.transform);
            var solvedRect = solvedIndicator.GetComponent<RectTransform>();
            solvedRect.anchorMin = new Vector2(0.5f, 0.5f);
            solvedRect.anchorMax = new Vector2(0.5f, 0.5f);
            solvedRect.sizeDelta = new Vector2(300f, 100f);
            solvedRect.anchoredPosition = Vector2.zero;

            var solvedImage = solvedIndicator.AddComponent<Image>();
            solvedImage.color = new Color(0, 1, 0, 0.7f);

            var solvedText = CreateTextElement("SolvedText", solvedIndicator.transform, "클리어!", 36);
            var solvedTextRect = solvedText.GetComponent<RectTransform>();
            solvedTextRect.anchorMin = Vector2.zero;
            solvedTextRect.anchorMax = Vector2.one;
            solvedTextRect.offsetMin = Vector2.zero;
            solvedTextRect.offsetMax = Vector2.zero;
            solvedText.alignment = TextAlignmentOptions.Center;
            solvedText.color = Color.white;

            solvedIndicator.SetActive(false);

            // 도움말 버튼 생성
            var helpButton = CreateButton("HelpButton", buttonPanel.transform, "?");
            var helpRect = helpButton.GetComponent<RectTransform>();
            helpRect.anchorMin = new Vector2(0.5f, 0.5f);
            helpRect.anchorMax = new Vector2(0.5f, 0.5f);
            helpRect.pivot = new Vector2(0.5f, 0.5f);
            helpRect.sizeDelta = new Vector2(40f, 40f);
            helpRect.anchoredPosition = new Vector2(0, 0);

            // 도움말 패널 생성
            var helpPanel = CreateUIElement("HelpPanel", puzzleUIObj.transform);
            var helpPanelRect = helpPanel.GetComponent<RectTransform>();
            helpPanelRect.anchorMin = new Vector2(0.5f, 0.5f);
            helpPanelRect.anchorMax = new Vector2(0.5f, 0.5f);
            helpPanelRect.sizeDelta = new Vector2(500f, 400f);
            helpPanelRect.anchoredPosition = Vector2.zero;

            var helpPanelImage = helpPanel.AddComponent<Image>();
            helpPanelImage.color = new Color(0, 0, 0, 0.95f);

            var helpPanelOutline = helpPanel.AddComponent<Outline>();
            helpPanelOutline.effectColor = Color.white;
            helpPanelOutline.effectDistance = new Vector2(2, 2);

            // 도움말 제목
            var helpTitle = CreateTextElement("HelpTitle", helpPanel.transform, "파동 간섭 퍼즐 - 플레이 방법", 22);
            var helpTitleRect = helpTitle.GetComponent<RectTransform>();
            helpTitleRect.anchorMin = new Vector2(0.5f, 1f);
            helpTitleRect.anchorMax = new Vector2(0.5f, 1f);
            helpTitleRect.pivot = new Vector2(0.5f, 1f);
            helpTitleRect.anchoredPosition = new Vector2(0, -20);

            // 도움말 내용
            var helpContent = CreateTextElement("HelpContent", helpPanel.transform, "", 14);
            var helpContentRect = helpContent.GetComponent<RectTransform>();
            helpContentRect.anchorMin = new Vector2(0f, 0f);
            helpContentRect.anchorMax = new Vector2(1f, 1f);
            helpContentRect.offsetMin = new Vector2(20, 60);
            helpContentRect.offsetMax = new Vector2(-20, -50);
            helpContent.alignment = TextAlignmentOptions.TopLeft;
            helpContent.text =
                "<b>목표:</b> 파동 소스를 배치하여 목표 지점에 원하는 강도의 파동을 만드세요.\n\n" +
                "<b>조작법:</b>\n" +
                "• 그리드를 클릭하면 파동 소스를 배치/제거합니다\n" +
                "• 노란색 원 = 파동 소스 (파동이 퍼져나가는 곳)\n\n" +
                "<b>목표 지점:</b>\n" +
                "• <color=#00FF00>녹색</color> = 보강 간섭 (파동이 강해야 함)\n" +
                "  → 파동 소스를 가까이 배치하거나 여러 파동이 겹치게 하세요\n\n" +
                "• <color=#00FFFF>청록색</color> = 상쇄 간섭 (파동이 약해야 함)\n" +
                "  → 파동 소스를 멀리 배치하거나 파동이 상쇄되게 하세요\n\n" +
                "<b>힌트:</b>\n" +
                "• 두 파동 소스 사이의 중간 지점은 파동이 강해집니다\n" +
                "• 파동 소스에서 멀어질수록 파동이 약해집니다\n" +
                "• 각 목표 위의 라벨을 확인하세요!";

            // 도움말 닫기 안내
            var closeHelpText = CreateTextElement("CloseHelpText", helpPanel.transform, "다시 ? 버튼을 누르면 닫힙니다", 12);
            var closeHelpRect = closeHelpText.GetComponent<RectTransform>();
            closeHelpRect.anchorMin = new Vector2(0.5f, 0f);
            closeHelpRect.anchorMax = new Vector2(0.5f, 0f);
            closeHelpRect.pivot = new Vector2(0.5f, 0f);
            closeHelpRect.anchoredPosition = new Vector2(0, 20);
            closeHelpText.color = Color.gray;

            helpPanel.SetActive(false);

            // InterferencePuzzleUI에 참조 연결
            var serializedObj = new SerializedObject(puzzleUIScript);
            serializedObj.FindProperty("gridContainer").objectReferenceValue = gridRect;
            serializedObj.FindProperty("cellPrefab").objectReferenceValue = cellPrefab;
            serializedObj.FindProperty("puzzleNameText").objectReferenceValue = puzzleNameText;
            serializedObj.FindProperty("accuracyText").objectReferenceValue = accuracyText;
            serializedObj.FindProperty("sourceCountText").objectReferenceValue = sourceCountText;
            serializedObj.FindProperty("clearButton").objectReferenceValue = clearButton.GetComponent<Button>();
            serializedObj.FindProperty("closeButton").objectReferenceValue = closeButton.GetComponent<Button>();
            serializedObj.FindProperty("solvedIndicator").objectReferenceValue = solvedIndicator;
            serializedObj.FindProperty("helpButton").objectReferenceValue = helpButton.GetComponent<Button>();
            serializedObj.FindProperty("helpPanel").objectReferenceValue = helpPanel;
            serializedObj.ApplyModifiedProperties();

            // Presenter 생성
            var presenterObj = new GameObject("InterferencePuzzlePresenter");
            presenterObj.transform.SetParent(canvas.transform, false);
            var presenter = presenterObj.AddComponent<InterferencePuzzlePresenter>();

            var presenterSerializedObj = new SerializedObject(presenter);
            presenterSerializedObj.FindProperty("puzzleUI").objectReferenceValue = puzzleUIScript;

            if (defaultPuzzleData != null)
            {
                presenterSerializedObj.FindProperty("defaultPuzzle").objectReferenceValue = defaultPuzzleData;
            }
            presenterSerializedObj.ApplyModifiedProperties();

            // Undo 등록
            Undo.RegisterCreatedObjectUndo(puzzleUIObj, "Create Interference Puzzle UI");
            Undo.RegisterCreatedObjectUndo(presenterObj, "Create Interference Puzzle Presenter");

            Selection.activeGameObject = puzzleUIObj;

            Debug.Log("퍼즐 UI 생성 완료");
            EditorUtility.DisplayDialog("완료",
                "퍼즐 UI가 생성되었습니다.\n\n" +
                "- InterferencePuzzleUI: 메인 UI 컨테이너\n" +
                "- InterferencePuzzlePresenter: 로직 컨트롤러\n\n" +
                "기본적으로 비활성화 상태입니다. 테스트하려면 활성화하세요.",
                "확인");
        }

        private GameObject CreateUIElement(string name, Transform parent)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            obj.AddComponent<RectTransform>();
            return obj;
        }

        private TextMeshProUGUI CreateTextElement(string name, Transform parent, string text, int fontSize)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);

            var rectTransform = obj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(400f, 30f);

            var tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            return tmp;
        }

        private GameObject CreateButton(string name, Transform parent, string text)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);

            var rectTransform = obj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(120f, 40f);

            var image = obj.AddComponent<Image>();
            image.color = new Color(0.3f, 0.3f, 0.3f);

            var button = obj.AddComponent<Button>();
            button.targetGraphic = image;

            var textObj = new GameObject("Text");
            textObj.transform.SetParent(obj.transform, false);

            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 18;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            return obj;
        }

        private void ApplyLayoutGroupToExistingUI()
        {
            var puzzleUI = FindFirstObjectByType<InterferencePuzzleUI>(FindObjectsInactive.Include);
            if (puzzleUI == null)
            {
                EditorUtility.DisplayDialog("오류", "씬에서 InterferencePuzzleUI를 찾을 수 없습니다.", "확인");
                return;
            }

            Undo.RecordObject(puzzleUI.gameObject, "Apply LayoutGroup");

            var puzzleUIRect = puzzleUI.GetComponent<RectTransform>();

            // 자식 찾기
            Transform topPanel = puzzleUIRect.Find("TopPanel");
            Transform gridContainer = puzzleUIRect.Find("GridContainer");
            Transform buttonPanel = puzzleUIRect.Find("ButtonPanel");
            Transform solvedIndicator = puzzleUIRect.Find("SolvedIndicator");

            if (topPanel == null || gridContainer == null || buttonPanel == null)
            {
                EditorUtility.DisplayDialog("오류",
                    "TopPanel, GridContainer, ButtonPanel 중 하나를 찾을 수 없습니다.\n" +
                    "UI 구조를 확인하세요.", "확인");
                return;
            }

            // 1. 자식 순서 재정렬 (TopPanel -> GridContainer -> ButtonPanel -> SolvedIndicator)
            topPanel.SetSiblingIndex(0);
            gridContainer.SetSiblingIndex(1);
            buttonPanel.SetSiblingIndex(2);
            if (solvedIndicator != null)
                solvedIndicator.SetSiblingIndex(3);

            // 2. InterferencePuzzleUI에 VerticalLayoutGroup 추가
            var vlg = puzzleUI.GetComponent<VerticalLayoutGroup>();
            if (vlg == null)
            {
                vlg = Undo.AddComponent<VerticalLayoutGroup>(puzzleUI.gameObject);
            }
            vlg.childAlignment = TextAnchor.MiddleCenter;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.spacing = 10f;
            vlg.padding = new RectOffset(50, 50, 30, 30);

            // 3. TopPanel 설정
            var topPanelRect = topPanel.GetComponent<RectTransform>();
            topPanelRect.anchorMin = new Vector2(0, 1);
            topPanelRect.anchorMax = new Vector2(1, 1);
            topPanelRect.pivot = new Vector2(0.5f, 1);

            var topLayoutElement = topPanel.GetComponent<LayoutElement>();
            if (topLayoutElement == null)
            {
                topLayoutElement = Undo.AddComponent<LayoutElement>(topPanel.gameObject);
            }
            topLayoutElement.preferredHeight = 80f;
            topLayoutElement.flexibleHeight = 0;

            var topHLG = topPanel.GetComponent<HorizontalLayoutGroup>();
            if (topHLG == null)
            {
                topHLG = Undo.AddComponent<HorizontalLayoutGroup>(topPanel.gameObject);
            }
            topHLG.childAlignment = TextAnchor.MiddleCenter;
            topHLG.childControlWidth = true;
            topHLG.childControlHeight = true;
            topHLG.childForceExpandWidth = true;
            topHLG.childForceExpandHeight = true;
            topHLG.spacing = 20f;

            // TopPanel 자식들에 LayoutElement 추가
            foreach (Transform child in topPanel)
            {
                var childLE = child.GetComponent<LayoutElement>();
                if (childLE == null)
                {
                    childLE = Undo.AddComponent<LayoutElement>(child.gameObject);
                }
                childLE.flexibleWidth = 1;
            }

            // 4. GridContainer 설정
            var gridContainerRect = gridContainer.GetComponent<RectTransform>();
            gridContainerRect.anchorMin = new Vector2(0, 0.5f);
            gridContainerRect.anchorMax = new Vector2(1, 0.5f);
            gridContainerRect.pivot = new Vector2(0.5f, 0.5f);

            var gridLayoutElement = gridContainer.GetComponent<LayoutElement>();
            if (gridLayoutElement == null)
            {
                gridLayoutElement = Undo.AddComponent<LayoutElement>(gridContainer.gameObject);
            }
            gridLayoutElement.flexibleHeight = 1;
            gridLayoutElement.flexibleWidth = 0;
            gridLayoutElement.preferredWidth = 400f;
            gridLayoutElement.preferredHeight = 400f;

            // GridLayoutGroup 추가 (셀 배치를 자동으로 처리)
            var gridLayoutGroup = gridContainer.GetComponent<GridLayoutGroup>();
            if (gridLayoutGroup == null)
            {
                gridLayoutGroup = Undo.AddComponent<GridLayoutGroup>(gridContainer.gameObject);
            }
            gridLayoutGroup.cellSize = new Vector2(40f, 40f);
            gridLayoutGroup.spacing = Vector2.zero;
            gridLayoutGroup.startCorner = GridLayoutGroup.Corner.LowerLeft;
            gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = 10;

            // 5. ButtonPanel 설정
            var buttonPanelRect = buttonPanel.GetComponent<RectTransform>();
            buttonPanelRect.anchorMin = new Vector2(0, 0);
            buttonPanelRect.anchorMax = new Vector2(1, 0);
            buttonPanelRect.pivot = new Vector2(0.5f, 0);

            var buttonLayoutElement = buttonPanel.GetComponent<LayoutElement>();
            if (buttonLayoutElement == null)
            {
                buttonLayoutElement = Undo.AddComponent<LayoutElement>(buttonPanel.gameObject);
            }
            buttonLayoutElement.preferredHeight = 60f;
            buttonLayoutElement.flexibleHeight = 0;

            var buttonHLG = buttonPanel.GetComponent<HorizontalLayoutGroup>();
            if (buttonHLG == null)
            {
                buttonHLG = Undo.AddComponent<HorizontalLayoutGroup>(buttonPanel.gameObject);
            }
            buttonHLG.childAlignment = TextAnchor.MiddleCenter;
            buttonHLG.childControlWidth = false;
            buttonHLG.childControlHeight = false;
            buttonHLG.childForceExpandWidth = false;
            buttonHLG.childForceExpandHeight = false;
            buttonHLG.spacing = 30f;

            // 6. SolvedIndicator를 LayoutGroup에서 제외 (겹쳐 표시되어야 함)
            if (solvedIndicator != null)
            {
                var solvedLE = solvedIndicator.GetComponent<LayoutElement>();
                if (solvedLE == null)
                {
                    solvedLE = Undo.AddComponent<LayoutElement>(solvedIndicator.gameObject);
                }
                solvedLE.ignoreLayout = true;

                // 중앙 고정
                var solvedRect = solvedIndicator.GetComponent<RectTransform>();
                solvedRect.anchorMin = new Vector2(0.5f, 0.5f);
                solvedRect.anchorMax = new Vector2(0.5f, 0.5f);
                solvedRect.anchoredPosition = Vector2.zero;
            }

            EditorUtility.SetDirty(puzzleUI.gameObject);

            Debug.Log("LayoutGroup 적용 완료");
            EditorUtility.DisplayDialog("완료",
                "LayoutGroup이 적용되었습니다.\n\n" +
                "• InterferencePuzzleUI: VerticalLayoutGroup\n" +
                "• TopPanel: HorizontalLayoutGroup + LayoutElement\n" +
                "• GridContainer: LayoutElement (Flexible)\n" +
                "• ButtonPanel: HorizontalLayoutGroup + LayoutElement\n\n" +
                "이제 UI 요소들이 그리드와 겹치지 않습니다.",
                "확인");
        }
    }
}
