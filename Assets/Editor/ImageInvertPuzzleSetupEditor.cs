using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using WaveStory.ImageInvert;
using System.Collections.Generic;

namespace WaveStory.Editor
{
    public class ImageInvertPuzzleSetupEditor : EditorWindow
    {
        private ImageInvertPuzzleData defaultPuzzleData;

        // PNG to Stage 변환 필드
        private Texture2D sourceTexture;
        private float threshold = 0.5f;
        private bool[,] previewBitmap;
        private string stageName = "New Stage";
        private int maxMovesValue = 10;
        private float timeLimitValue = 60f;
        private int initialInvertCountValue = 3;

        [MenuItem("WaveStory/Setup ImageInvert Puzzle")]
        public static void ShowWindow()
        {
            GetWindow<ImageInvertPuzzleSetupEditor>("ImageInvert Puzzle Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("이미지 인버트 퍼즐 설정", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            defaultPuzzleData = (ImageInvertPuzzleData)EditorGUILayout.ObjectField(
                "기본 퍼즐 데이터", defaultPuzzleData, typeof(ImageInvertPuzzleData), false);

            EditorGUILayout.Space();

            if (GUILayout.Button("1. Prefab 생성 (Cell + InvertButton)", GUILayout.Height(30)))
            {
                CreatePrefabs();
            }

            if (GUILayout.Button("2. 씬에 전체 퍼즐 UI 생성", GUILayout.Height(30)))
            {
                CreatePuzzleUIInScene();
            }

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("3. 씬의 UI를 프리팹으로 저장", GUILayout.Height(40)))
            {
                SaveSceneUIAsPrefab();
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space();
            GUILayout.Label("데이터 생성", EditorStyles.boldLabel);

            if (GUILayout.Button("4. 10개 스테이지 데이터 생성", GUILayout.Height(30)))
            {
                CreateStageData();
            }

            EditorGUILayout.Space();
            GUILayout.Label("PNG에서 스테이지 생성", EditorStyles.boldLabel);

            sourceTexture = (Texture2D)EditorGUILayout.ObjectField(
                "소스 PNG 텍스처", sourceTexture, typeof(Texture2D), false);

            if (sourceTexture != null)
            {
                EditorGUILayout.LabelField("텍스처 크기", $"{sourceTexture.width} x {sourceTexture.height}");

                threshold = EditorGUILayout.Slider("흑백 임계값", threshold, 0f, 1f);

                stageName = EditorGUILayout.TextField("스테이지 이름", stageName);
                maxMovesValue = EditorGUILayout.IntSlider("최대 이동 횟수", maxMovesValue, 5, 30);
                timeLimitValue = EditorGUILayout.FloatField("제한 시간 (초)", timeLimitValue);
                initialInvertCountValue = EditorGUILayout.IntSlider("초기 인버트 수", initialInvertCountValue, 1, 10);

                if (GUILayout.Button("미리보기", GUILayout.Height(25)))
                {
                    previewBitmap = ConvertTextureTobitmap(sourceTexture, threshold);
                }

                if (previewBitmap != null)
                {
                    DrawBitmapPreview(previewBitmap);

                    GUI.backgroundColor = Color.cyan;
                    if (GUILayout.Button("PNG를 스테이지 데이터로 저장", GUILayout.Height(30)))
                    {
                        SavePNGAsStageData(previewBitmap);
                    }
                    GUI.backgroundColor = Color.white;
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "사용법:\n" +
                "1. Prefab 생성 버튼 클릭\n" +
                "2. 씬에 Canvas가 있는지 확인\n" +
                "3. 전체 퍼즐 UI 생성 버튼 클릭\n" +
                "4. 씬의 UI를 프리팹으로 저장 버튼 클릭\n" +
                "5. 10개 스테이지 데이터 생성 버튼 클릭\n" +
                "6. Presenter의 Default Puzzle 참조 설정\n\n" +
                "PNG to Stage: PNG 텍스처를 선택하여 12x12 비트맵으로 변환 가능",
                MessageType.Info);
        }

        private void CreatePrefabs()
        {
            // 프리팹 저장 경로
            string prefabPath = "Assets/Prefabs/UI/ImageInvert";
            if (!AssetDatabase.IsValidFolder(prefabPath))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Prefabs/UI"))
                {
                    AssetDatabase.CreateFolder("Assets/Prefabs", "UI");
                }
                AssetDatabase.CreateFolder("Assets/Prefabs/UI", "ImageInvert");
            }

            // Cell Prefab 생성
            var cellObj = new GameObject("ImageInvertCell");
            var cellRect = cellObj.AddComponent<RectTransform>();
            cellRect.sizeDelta = new Vector2(30f, 30f);

            var cellImage = cellObj.AddComponent<Image>();
            cellImage.color = Color.black;

            string cellPath = $"{prefabPath}/ImageInvertCell.prefab";
            PrefabUtility.SaveAsPrefabAsset(cellObj, cellPath);
            DestroyImmediate(cellObj);

            // InvertButton Prefab 생성
            var buttonObj = new GameObject("InvertButton");
            var buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(30f, 30f);

            var buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.7f, 0.7f, 0.7f, 1f);

            var button = buttonObj.AddComponent<Button>();
            button.targetGraphic = buttonImage;

            // 버튼 텍스트
            var textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);

            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = "1";
            tmp.fontSize = 12;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.black;

            string buttonPath = $"{prefabPath}/InvertButton.prefab";
            PrefabUtility.SaveAsPrefabAsset(buttonObj, buttonPath);
            DestroyImmediate(buttonObj);

            AssetDatabase.Refresh();
            Debug.Log($"Prefab 생성 완료: {prefabPath}");
            EditorUtility.DisplayDialog("완료", $"Prefab이 생성되었습니다.\n{cellPath}\n{buttonPath}", "확인");
        }

        private void CreatePuzzleUIInScene()
        {
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                EditorUtility.DisplayDialog("오류", "씬에 Canvas가 없습니다. Canvas를 먼저 생성하세요.", "확인");
                return;
            }

            // Prefab 찾기
            string cellPrefabPath = "Assets/Prefabs/UI/ImageInvert/ImageInvertCell.prefab";
            string buttonPrefabPath = "Assets/Prefabs/UI/ImageInvert/InvertButton.prefab";

            var cellPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(cellPrefabPath);
            var buttonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(buttonPrefabPath);

            if (cellPrefab == null || buttonPrefab == null)
            {
                EditorUtility.DisplayDialog("오류", "Prefab을 먼저 생성하세요.", "확인");
                return;
            }

            // 메인 UI 컨테이너 생성
            var puzzleUIObj = new GameObject("ImageInvertPuzzleUI");
            puzzleUIObj.transform.SetParent(canvas.transform, false);

            var puzzleUIRect = puzzleUIObj.AddComponent<RectTransform>();
            puzzleUIRect.anchorMin = Vector2.zero;
            puzzleUIRect.anchorMax = Vector2.one;
            puzzleUIRect.offsetMin = Vector2.zero;
            puzzleUIRect.offsetMax = Vector2.zero;

            var bgImage = puzzleUIObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.95f);

            var puzzleUIScript = puzzleUIObj.AddComponent<ImageInvertPuzzleUI>();

            // 상단 정보 패널
            var topPanel = CreateUIElement("TopPanel", puzzleUIObj.transform);
            var topPanelRect = topPanel.GetComponent<RectTransform>();
            topPanelRect.anchorMin = new Vector2(0.5f, 1f);
            topPanelRect.anchorMax = new Vector2(0.5f, 1f);
            topPanelRect.pivot = new Vector2(0.5f, 1f);
            topPanelRect.sizeDelta = new Vector2(600f, 80f);
            topPanelRect.anchoredPosition = new Vector2(0, -20);

            var puzzleNameText = CreateTextElement("PuzzleNameText", topPanel.transform, "스테이지 1", 24);
            var nameRect = puzzleNameText.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0.5f, 1f);
            nameRect.anchorMax = new Vector2(0.5f, 1f);
            nameRect.pivot = new Vector2(0.5f, 1f);
            nameRect.anchoredPosition = new Vector2(0, -10);

            var movesText = CreateTextElement("MovesText", topPanel.transform, "남은 횟수: 10/10", 18);
            var movesRect = movesText.GetComponent<RectTransform>();
            movesRect.anchorMin = new Vector2(0.5f, 1f);
            movesRect.anchorMax = new Vector2(0.5f, 1f);
            movesRect.pivot = new Vector2(0.5f, 1f);
            movesRect.anchoredPosition = new Vector2(-100, -45);

            var timeText = CreateTextElement("TimeText", topPanel.transform, "남은 시간: 01:00", 18);
            var timeRect = timeText.GetComponent<RectTransform>();
            timeRect.anchorMin = new Vector2(0.5f, 1f);
            timeRect.anchorMax = new Vector2(0.5f, 1f);
            timeRect.pivot = new Vector2(0.5f, 1f);
            timeRect.anchoredPosition = new Vector2(100, -45);

            // 중앙 컨테이너 (그리드 + 우측 버튼)
            var centerContainer = CreateUIElement("CenterContainer", puzzleUIObj.transform);
            var centerRect = centerContainer.GetComponent<RectTransform>();
            centerRect.anchorMin = new Vector2(0.5f, 0.5f);
            centerRect.anchorMax = new Vector2(0.5f, 0.5f);
            centerRect.pivot = new Vector2(0.5f, 0.5f);
            centerRect.sizeDelta = new Vector2(440f, 430f);  // Row 버튼 크기 조정에 맞춰 축소
            centerRect.anchoredPosition = new Vector2(0, 10);

            var centerHLG = centerContainer.AddComponent<HorizontalLayoutGroup>();
            centerHLG.spacing = 10f;
            centerHLG.childAlignment = TextAnchor.MiddleCenter;
            centerHLG.childControlWidth = false;
            centerHLG.childControlHeight = false;
            centerHLG.childForceExpandWidth = false;
            centerHLG.childForceExpandHeight = false;

            // 좌측 컨테이너 (그리드 + 하단 버튼)
            var leftContainer = CreateUIElement("LeftContainer", centerContainer.transform);
            var leftRect = leftContainer.GetComponent<RectTransform>();
            leftRect.sizeDelta = new Vector2(400f, 430f);

            var leftVLG = leftContainer.AddComponent<VerticalLayoutGroup>();
            leftVLG.spacing = 10f;
            leftVLG.childAlignment = TextAnchor.MiddleCenter;
            leftVLG.childControlWidth = false;
            leftVLG.childControlHeight = false;
            leftVLG.childForceExpandWidth = false;
            leftVLG.childForceExpandHeight = false;

            var leftLE = leftContainer.AddComponent<LayoutElement>();
            leftLE.preferredWidth = 400f;
            leftLE.preferredHeight = 430f;

            // Grid Container
            var gridContainer = CreateUIElement("GridContainer", leftContainer.transform);
            var gridRect = gridContainer.GetComponent<RectTransform>();
            gridRect.sizeDelta = new Vector2(396f, 396f);

            var gridLE = gridContainer.AddComponent<LayoutElement>();
            gridLE.preferredWidth = 396f;
            gridLE.preferredHeight = 396f;

            var gridBG = gridContainer.AddComponent<Image>();
            gridBG.color = new Color(0.1f, 0.1f, 0.1f, 1f);

            // Grid Layout Group 설정
            var gridLayout = gridContainer.AddComponent<GridLayoutGroup>();
            gridLayout.spacing = Vector2.one;
            gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.childAlignment = TextAnchor.MiddleCenter;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 12;
            gridLayout.cellSize = new Vector2(32f, 32f);

            // 12x12 그리드 셀 생성 (에디트 타임) - 상단부터 하단으로
            var gridCellsList = new List<Image>();
            for (int y = 0; y < 12; y++)
            {
                for (int x = 0; x < 12; x++)
                {
                    var cellObj = (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab, gridContainer.transform);
                    cellObj.name = $"Cell_{x}_{y}";
                    var cellImage = cellObj.GetComponent<Image>();
                    if (cellImage != null)
                    {
                        cellImage.color = Color.black;
                        gridCellsList.Add(cellImage);
                    }
                }
            }

            // 하단 버튼 컨테이너
            var columnButtonContainer = CreateUIElement("ColumnButtonContainer", leftContainer.transform);
            var columnRect = columnButtonContainer.GetComponent<RectTransform>();
            columnRect.sizeDelta = new Vector2(396f, 30f);

            var columnLE = columnButtonContainer.AddComponent<LayoutElement>();
            columnLE.preferredWidth = 396f;
            columnLE.preferredHeight = 30f;

            // Column 버튼 Layout Group 설정 - 그리드와 동일한 spacing 사용
            var columnLayout = columnButtonContainer.AddComponent<HorizontalLayoutGroup>();
            columnLayout.spacing = 1f;  // 그리드와 동일한 spacing
            columnLayout.childAlignment = TextAnchor.MiddleCenter;
            columnLayout.childForceExpandWidth = false;
            columnLayout.childForceExpandHeight = true;
            columnLayout.childScaleWidth = false;

            // 12개 Column 버튼 생성 (에디트 타임)
            var columnButtonsList = new List<Button>();
            for (int col = 0; col < 12; col++)
            {
                var buttonObj = (GameObject)PrefabUtility.InstantiatePrefab(buttonPrefab, columnButtonContainer.transform);
                buttonObj.name = $"ColumnButton_{col}";

                // 버튼 크기를 그리드 셀과 동일하게 설정
                var buttonRect = buttonObj.GetComponent<RectTransform>();
                if (buttonRect != null)
                {
                    buttonRect.sizeDelta = new Vector2(32f, 30f);  // 너비를 그리드 셀과 동일하게
                }

                var button = buttonObj.GetComponent<Button>();
                if (button != null)
                {
                    columnButtonsList.Add(button);
                }

                var label = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                {
                    label.text = (col + 1).ToString();
                    label.fontSize = 12;
                }
            }

            // 우측 버튼 컨테이너
            var rowButtonContainer = CreateUIElement("RowButtonContainer", centerContainer.transform);
            var rowRect = rowButtonContainer.GetComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(30f, 396f);  // 버튼 너비와 동일하게

            var rowLE = rowButtonContainer.AddComponent<LayoutElement>();
            rowLE.preferredWidth = 30f;  // 버튼 너비와 동일하게
            rowLE.preferredHeight = 396f;

            // Row 버튼 Layout Group 설정 - 그리드와 동일한 spacing 사용
            var rowLayout = rowButtonContainer.AddComponent<VerticalLayoutGroup>();
            rowLayout.spacing = 1f;  // 그리드와 동일한 spacing
            rowLayout.childAlignment = TextAnchor.MiddleCenter;
            rowLayout.childForceExpandWidth = true;
            rowLayout.childForceExpandHeight = false;
            rowLayout.childScaleHeight = false;

            // 12개 Row 버튼 생성 (에디트 타임) - 위에서 아래로, 번호는 역순
            var rowButtonsList = new List<Button>();
            for (int row = 0; row < 12; row++)  // 0부터 11까지 순서대로
            {
                var buttonObj = (GameObject)PrefabUtility.InstantiatePrefab(buttonPrefab, rowButtonContainer.transform);
                buttonObj.name = $"RowButton_{row}";

                // 버튼 크기를 그리드 셀과 동일하게 설정
                var buttonRect = buttonObj.GetComponent<RectTransform>();
                if (buttonRect != null)
                {
                    buttonRect.sizeDelta = new Vector2(30f, 32f);  // 높이를 그리드 셀과 동일하게
                }

                var button = buttonObj.GetComponent<Button>();
                if (button != null)
                {
                    rowButtonsList.Add(button);  // 순서대로 추가
                }

                var label = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                {
                    // Row 번호를 역순으로 표시 (맨 위가 12, 맨 아래가 1)
                    label.text = (12 - row).ToString();
                    label.fontSize = 12;
                }
            }

            // 하단 버튼 패널
            var buttonPanel = CreateUIElement("ButtonPanel", puzzleUIObj.transform);
            var btnPanelRect = buttonPanel.GetComponent<RectTransform>();
            btnPanelRect.anchorMin = new Vector2(0.5f, 0f);
            btnPanelRect.anchorMax = new Vector2(0.5f, 0f);
            btnPanelRect.pivot = new Vector2(0.5f, 0f);
            btnPanelRect.sizeDelta = new Vector2(400f, 60f);
            btnPanelRect.anchoredPosition = new Vector2(0, 30);

            var resetButton = CreateButton("ResetButton", buttonPanel.transform, "다시 시작");
            var resetRect = resetButton.GetComponent<RectTransform>();
            resetRect.anchorMin = new Vector2(0f, 0.5f);
            resetRect.anchorMax = new Vector2(0f, 0.5f);
            resetRect.pivot = new Vector2(0f, 0.5f);
            resetRect.sizeDelta = new Vector2(120f, 40f);
            resetRect.anchoredPosition = new Vector2(50, 0);

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
            solvedImage.color = new Color(0, 0.8f, 0, 0.9f);

            var solvedText = CreateTextElement("SolvedText", solvedIndicator.transform, "클리어!", 36);
            var solvedTextRect = solvedText.GetComponent<RectTransform>();
            solvedTextRect.anchorMin = Vector2.zero;
            solvedTextRect.anchorMax = Vector2.one;
            solvedTextRect.offsetMin = Vector2.zero;
            solvedTextRect.offsetMax = Vector2.zero;
            solvedText.alignment = TextAlignmentOptions.Center;
            solvedText.color = Color.white;

            solvedIndicator.SetActive(false);

            // 실패 표시
            var failedIndicator = CreateUIElement("FailedIndicator", puzzleUIObj.transform);
            var failedRect = failedIndicator.GetComponent<RectTransform>();
            failedRect.anchorMin = new Vector2(0.5f, 0.5f);
            failedRect.anchorMax = new Vector2(0.5f, 0.5f);
            failedRect.sizeDelta = new Vector2(300f, 100f);
            failedRect.anchoredPosition = Vector2.zero;

            var failedImage = failedIndicator.AddComponent<Image>();
            failedImage.color = new Color(0.8f, 0, 0, 0.9f);

            var failedText = CreateTextElement("FailedText", failedIndicator.transform, "실패!", 36);
            var failedTextRect = failedText.GetComponent<RectTransform>();
            failedTextRect.anchorMin = Vector2.zero;
            failedTextRect.anchorMax = Vector2.one;
            failedTextRect.offsetMin = Vector2.zero;
            failedTextRect.offsetMax = Vector2.zero;
            failedText.alignment = TextAlignmentOptions.Center;
            failedText.color = Color.white;

            failedIndicator.SetActive(false);

            // ImageInvertPuzzleUI에 참조 연결
            var serializedObj = new SerializedObject(puzzleUIScript);

            // Grid Cells 리스트 연결
            var gridCellsProperty = serializedObj.FindProperty("gridCells");
            if (gridCellsProperty != null)
            {
                gridCellsProperty.ClearArray();
                for (int i = 0; i < gridCellsList.Count; i++)
                {
                    gridCellsProperty.InsertArrayElementAtIndex(i);
                    gridCellsProperty.GetArrayElementAtIndex(i).objectReferenceValue = gridCellsList[i];
                }
            }
            else
            {
                Debug.LogError("gridCells property not found in ImageInvertPuzzleUI!");
            }

            // Column Buttons 리스트 연결
            var columnButtonsProperty = serializedObj.FindProperty("columnButtons");
            if (columnButtonsProperty != null)
            {
                columnButtonsProperty.ClearArray();
                for (int i = 0; i < columnButtonsList.Count; i++)
                {
                    columnButtonsProperty.InsertArrayElementAtIndex(i);
                    columnButtonsProperty.GetArrayElementAtIndex(i).objectReferenceValue = columnButtonsList[i];
                }
            }
            else
            {
                Debug.LogError("columnButtons property not found in ImageInvertPuzzleUI!");
            }

            // Row Buttons 리스트 연결
            var rowButtonsProperty = serializedObj.FindProperty("rowButtons");
            if (rowButtonsProperty != null)
            {
                rowButtonsProperty.ClearArray();
                for (int i = 0; i < rowButtonsList.Count; i++)
                {
                    rowButtonsProperty.InsertArrayElementAtIndex(i);
                    rowButtonsProperty.GetArrayElementAtIndex(i).objectReferenceValue = rowButtonsList[i];
                }
            }
            else
            {
                Debug.LogError("rowButtons property not found in ImageInvertPuzzleUI!");
            }

            serializedObj.FindProperty("puzzleNameText").objectReferenceValue = puzzleNameText;
            serializedObj.FindProperty("movesText").objectReferenceValue = movesText;
            serializedObj.FindProperty("timeText").objectReferenceValue = timeText;
            serializedObj.FindProperty("resetButton").objectReferenceValue = resetButton.GetComponent<Button>();
            serializedObj.FindProperty("closeButton").objectReferenceValue = closeButton.GetComponent<Button>();
            serializedObj.FindProperty("solvedIndicator").objectReferenceValue = solvedIndicator;
            serializedObj.FindProperty("failedIndicator").objectReferenceValue = failedIndicator;
            serializedObj.ApplyModifiedProperties();

            // Presenter 생성
            var presenterObj = new GameObject("ImageInvertPuzzlePresenter");
            presenterObj.transform.SetParent(canvas.transform, false);
            var presenter = presenterObj.AddComponent<ImageInvertPuzzlePresenter>();

            var presenterSerializedObj = new SerializedObject(presenter);
            presenterSerializedObj.FindProperty("puzzleUI").objectReferenceValue = puzzleUIScript;

            if (defaultPuzzleData != null)
            {
                presenterSerializedObj.FindProperty("defaultPuzzle").objectReferenceValue = defaultPuzzleData;
            }
            presenterSerializedObj.ApplyModifiedProperties();

            Undo.RegisterCreatedObjectUndo(puzzleUIObj, "Create ImageInvert Puzzle UI");
            Undo.RegisterCreatedObjectUndo(presenterObj, "Create ImageInvert Puzzle Presenter");

            Selection.activeGameObject = puzzleUIObj;

            Debug.Log("ImageInvert 퍼즐 UI 생성 완료 - 144개 셀과 24개 버튼이 모두 생성되었습니다.");
            EditorUtility.DisplayDialog("완료",
                "퍼즐 UI가 완전히 생성되었습니다.\n\n" +
                "- 144개 Grid Cells (12x12)\n" +
                "- 12개 Column Buttons\n" +
                "- 12개 Row Buttons\n" +
                "- ImageInvertPuzzleUI: 메인 UI 컨테이너\n" +
                "- ImageInvertPuzzlePresenter: 로직 컨트롤러\n\n" +
                "Inspector에서 Default Puzzle을 설정하세요.",
                "확인");
        }

        private void SaveSceneUIAsPrefab()
        {
            var puzzleUI = GameObject.Find("ImageInvertPuzzleUI");
            if (puzzleUI == null)
            {
                EditorUtility.DisplayDialog("오류", "씬에서 ImageInvertPuzzleUI를 찾을 수 없습니다.\n먼저 UI를 생성하세요.", "확인");
                return;
            }

            string prefabPath = "Assets/Prefabs/UI/ImageInvert/ImageInvertPuzzleUI_Complete.prefab";

            // 프리팹으로 저장
            PrefabUtility.SaveAsPrefabAssetAndConnect(puzzleUI, prefabPath, InteractionMode.UserAction);

            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            Debug.Log($"프리팹 저장 완료: {prefabPath}");
            EditorUtility.DisplayDialog("완료",
                $"UI가 프리팹으로 저장되었습니다.\n{prefabPath}\n\n" +
                "이제 이 프리팹을 씬에서 인스턴스화하여 사용할 수 있습니다.",
                "확인");
        }

        private void CreateStageData()
        {
            string dataPath = "Assets/Resources/ImageInvertPuzzle";
            if (!AssetDatabase.IsValidFolder(dataPath))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }
                AssetDatabase.CreateFolder("Assets/Resources", "ImageInvertPuzzle");
            }

            // 10개의 스테이지 데이터 생성
            CreateStage1(dataPath); // Apple
            CreateStage2(dataPath); // Heart
            CreateStage3(dataPath); // Star
            CreateStage4(dataPath); // House
            CreateStage5(dataPath); // Tree
            CreateStage6(dataPath); // Fish
            CreateStage7(dataPath); // Cat
            CreateStage8(dataPath); // Key
            CreateStage9(dataPath); // Crown
            CreateStage10(dataPath); // Rocket

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("완료", $"10개의 스테이지 데이터가 생성되었습니다.\n{dataPath}", "확인");
        }

        private void CreateStage1(string basePath)
        {
            // Apple (사과)
            var data = ScriptableObject.CreateInstance<ImageInvertPuzzleData>();
            data.puzzleName = "사과";
            data.initialInvertCount = 2;
            data.maxMoves = 8;
            data.timeLimit = 60f;

            // 12x12 사과 비트맵
            bool[] bitmap = new bool[144];
            int[] applePattern = {
                0,0,0,0,0,1,1,0,0,0,0,0,
                0,0,0,0,1,1,1,0,0,0,0,0,
                0,0,0,1,1,1,1,1,0,0,0,0,
                0,0,1,1,1,1,1,1,1,1,0,0,
                0,1,1,1,1,1,1,1,1,1,1,0,
                0,1,1,1,1,1,1,1,1,1,1,0,
                0,1,1,1,1,1,1,1,1,1,1,0,
                0,1,1,1,1,1,1,1,1,1,1,0,
                0,1,1,1,1,1,1,1,1,1,1,0,
                0,0,1,1,1,1,1,1,1,1,0,0,
                0,0,0,1,1,1,1,1,1,0,0,0,
                0,0,0,0,1,1,1,1,0,0,0,0
            };
            for (int i = 0; i < 144; i++) bitmap[i] = applePattern[i] == 1;
            data.bitmap = bitmap;

            AssetDatabase.CreateAsset(data, $"{basePath}/Stage01_Apple.asset");
        }

        private void CreateStage2(string basePath)
        {
            // Heart (하트)
            var data = ScriptableObject.CreateInstance<ImageInvertPuzzleData>();
            data.puzzleName = "하트";
            data.initialInvertCount = 3;
            data.maxMoves = 10;
            data.timeLimit = 90f;

            bool[] bitmap = new bool[144];
            int[] pattern = {
                0,0,1,1,0,0,0,0,1,1,0,0,
                0,1,1,1,1,0,0,1,1,1,1,0,
                1,1,1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,1,1,
                0,1,1,1,1,1,1,1,1,1,1,0,
                0,0,1,1,1,1,1,1,1,1,0,0,
                0,0,0,1,1,1,1,1,1,0,0,0,
                0,0,0,0,1,1,1,1,0,0,0,0,
                0,0,0,0,0,1,1,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0
            };
            for (int i = 0; i < 144; i++) bitmap[i] = pattern[i] == 1;
            data.bitmap = bitmap;

            AssetDatabase.CreateAsset(data, $"{basePath}/Stage02_Heart.asset");
        }

        private void CreateStage3(string basePath)
        {
            // Star (별)
            var data = ScriptableObject.CreateInstance<ImageInvertPuzzleData>();
            data.puzzleName = "별";
            data.initialInvertCount = 3;
            data.maxMoves = 12;
            data.timeLimit = 90f;

            bool[] bitmap = new bool[144];
            int[] pattern = {
                0,0,0,0,0,1,1,0,0,0,0,0,
                0,0,0,0,0,1,1,0,0,0,0,0,
                0,0,0,0,1,1,1,1,0,0,0,0,
                1,1,1,1,1,1,1,1,1,1,1,1,
                0,1,1,1,1,1,1,1,1,1,1,0,
                0,0,1,1,1,1,1,1,1,1,0,0,
                0,0,0,1,1,1,1,1,1,0,0,0,
                0,0,1,1,1,0,0,1,1,1,0,0,
                0,1,1,1,0,0,0,0,1,1,1,0,
                1,1,1,0,0,0,0,0,0,1,1,1,
                1,1,0,0,0,0,0,0,0,0,1,1,
                0,0,0,0,0,0,0,0,0,0,0,0
            };
            for (int i = 0; i < 144; i++) bitmap[i] = pattern[i] == 1;
            data.bitmap = bitmap;

            AssetDatabase.CreateAsset(data, $"{basePath}/Stage03_Star.asset");
        }

        private void CreateStage4(string basePath)
        {
            // House (집)
            var data = ScriptableObject.CreateInstance<ImageInvertPuzzleData>();
            data.puzzleName = "집";
            data.initialInvertCount = 4;
            data.maxMoves = 12;
            data.timeLimit = 120f;

            bool[] bitmap = new bool[144];
            int[] pattern = {
                0,0,0,0,0,1,1,0,0,0,0,0,
                0,0,0,0,1,1,1,1,0,0,0,0,
                0,0,0,1,1,1,1,1,1,0,0,0,
                0,0,1,1,1,1,1,1,1,1,0,0,
                0,1,1,1,1,1,1,1,1,1,1,0,
                1,1,1,1,1,1,1,1,1,1,1,1,
                0,1,1,1,1,1,1,1,1,1,1,0,
                0,1,1,1,0,0,0,0,1,1,1,0,
                0,1,1,1,0,0,0,0,1,1,1,0,
                0,1,1,1,0,0,0,0,1,1,1,0,
                0,1,1,1,1,1,1,1,1,1,1,0,
                0,1,1,1,1,1,1,1,1,1,1,0
            };
            for (int i = 0; i < 144; i++) bitmap[i] = pattern[i] == 1;
            data.bitmap = bitmap;

            AssetDatabase.CreateAsset(data, $"{basePath}/Stage04_House.asset");
        }

        private void CreateStage5(string basePath)
        {
            // Tree (나무)
            var data = ScriptableObject.CreateInstance<ImageInvertPuzzleData>();
            data.puzzleName = "나무";
            data.initialInvertCount = 4;
            data.maxMoves = 14;
            data.timeLimit = 120f;

            bool[] bitmap = new bool[144];
            int[] pattern = {
                0,0,0,0,0,1,1,0,0,0,0,0,
                0,0,0,0,1,1,1,1,0,0,0,0,
                0,0,0,1,1,1,1,1,1,0,0,0,
                0,0,1,1,1,1,1,1,1,1,0,0,
                0,0,0,1,1,1,1,1,1,0,0,0,
                0,0,1,1,1,1,1,1,1,1,0,0,
                0,1,1,1,1,1,1,1,1,1,1,0,
                1,1,1,1,1,1,1,1,1,1,1,1,
                0,0,0,0,0,1,1,0,0,0,0,0,
                0,0,0,0,0,1,1,0,0,0,0,0,
                0,0,0,0,0,1,1,0,0,0,0,0,
                0,0,0,0,1,1,1,1,0,0,0,0
            };
            for (int i = 0; i < 144; i++) bitmap[i] = pattern[i] == 1;
            data.bitmap = bitmap;

            AssetDatabase.CreateAsset(data, $"{basePath}/Stage05_Tree.asset");
        }

        private void CreateStage6(string basePath)
        {
            // Fish (물고기)
            var data = ScriptableObject.CreateInstance<ImageInvertPuzzleData>();
            data.puzzleName = "물고기";
            data.initialInvertCount = 5;
            data.maxMoves = 14;
            data.timeLimit = 120f;

            bool[] bitmap = new bool[144];
            int[] pattern = {
                0,0,0,0,1,1,1,1,0,0,0,0,
                0,0,0,1,1,1,1,1,1,0,0,0,
                1,0,1,1,1,1,1,1,1,1,0,0,
                1,1,1,1,1,0,1,1,1,1,1,0,
                1,1,1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,1,0,
                1,0,1,1,1,1,1,1,1,1,0,0,
                0,0,0,1,1,1,1,1,1,0,0,0,
                0,0,0,0,1,1,1,1,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0
            };
            for (int i = 0; i < 144; i++) bitmap[i] = pattern[i] == 1;
            data.bitmap = bitmap;

            AssetDatabase.CreateAsset(data, $"{basePath}/Stage06_Fish.asset");
        }

        private void CreateStage7(string basePath)
        {
            // Cat (고양이 얼굴)
            var data = ScriptableObject.CreateInstance<ImageInvertPuzzleData>();
            data.puzzleName = "고양이";
            data.initialInvertCount = 5;
            data.maxMoves = 16;
            data.timeLimit = 150f;

            bool[] bitmap = new bool[144];
            int[] pattern = {
                1,1,0,0,0,0,0,0,0,0,1,1,
                1,1,1,0,0,0,0,0,0,1,1,1,
                0,1,1,1,1,1,1,1,1,1,1,0,
                0,1,1,1,1,1,1,1,1,1,1,0,
                0,1,1,0,1,1,1,1,0,1,1,0,
                0,1,1,1,1,1,1,1,1,1,1,0,
                0,1,1,1,1,0,0,1,1,1,1,0,
                0,1,1,1,0,1,1,0,1,1,1,0,
                0,0,1,1,1,1,1,1,1,1,0,0,
                0,0,0,1,1,1,1,1,1,0,0,0,
                0,0,0,0,1,1,1,1,0,0,0,0,
                0,0,0,0,0,0,0,0,0,0,0,0
            };
            for (int i = 0; i < 144; i++) bitmap[i] = pattern[i] == 1;
            data.bitmap = bitmap;

            AssetDatabase.CreateAsset(data, $"{basePath}/Stage07_Cat.asset");
        }

        private void CreateStage8(string basePath)
        {
            // Key (열쇠)
            var data = ScriptableObject.CreateInstance<ImageInvertPuzzleData>();
            data.puzzleName = "열쇠";
            data.initialInvertCount = 6;
            data.maxMoves = 16;
            data.timeLimit = 150f;

            bool[] bitmap = new bool[144];
            int[] pattern = {
                0,0,1,1,1,1,1,1,0,0,0,0,
                0,1,1,0,0,0,0,1,1,0,0,0,
                0,1,0,0,0,0,0,0,1,0,0,0,
                0,1,1,0,0,0,0,1,1,0,0,0,
                0,0,1,1,1,1,1,1,0,0,0,0,
                0,0,0,0,1,1,0,0,0,0,0,0,
                0,0,0,0,1,1,0,0,0,0,0,0,
                0,0,0,0,1,1,1,1,0,0,0,0,
                0,0,0,0,1,1,0,0,0,0,0,0,
                0,0,0,0,1,1,1,1,0,0,0,0,
                0,0,0,0,1,1,0,0,0,0,0,0,
                0,0,0,0,1,1,0,0,0,0,0,0
            };
            for (int i = 0; i < 144; i++) bitmap[i] = pattern[i] == 1;
            data.bitmap = bitmap;

            AssetDatabase.CreateAsset(data, $"{basePath}/Stage08_Key.asset");
        }

        private void CreateStage9(string basePath)
        {
            // Crown (왕관)
            var data = ScriptableObject.CreateInstance<ImageInvertPuzzleData>();
            data.puzzleName = "왕관";
            data.initialInvertCount = 6;
            data.maxMoves = 18;
            data.timeLimit = 180f;

            bool[] bitmap = new bool[144];
            int[] pattern = {
                0,1,0,0,0,1,1,0,0,0,1,0,
                0,1,1,0,0,1,1,0,0,1,1,0,
                0,1,1,0,0,1,1,0,0,1,1,0,
                0,1,1,1,0,1,1,0,1,1,1,0,
                0,1,1,1,1,1,1,1,1,1,1,0,
                0,1,1,1,1,1,1,1,1,1,1,0,
                0,1,1,1,1,1,1,1,1,1,1,0,
                0,1,1,1,1,1,1,1,1,1,1,0,
                0,1,1,1,1,1,1,1,1,1,1,0,
                1,1,1,1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,1,1,1,
                0,0,0,0,0,0,0,0,0,0,0,0
            };
            for (int i = 0; i < 144; i++) bitmap[i] = pattern[i] == 1;
            data.bitmap = bitmap;

            AssetDatabase.CreateAsset(data, $"{basePath}/Stage09_Crown.asset");
        }

        private void CreateStage10(string basePath)
        {
            // Rocket (로켓)
            var data = ScriptableObject.CreateInstance<ImageInvertPuzzleData>();
            data.puzzleName = "로켓";
            data.initialInvertCount = 7;
            data.maxMoves = 20;
            data.timeLimit = 180f;

            bool[] bitmap = new bool[144];
            int[] pattern = {
                0,0,0,0,0,1,1,0,0,0,0,0,
                0,0,0,0,1,1,1,1,0,0,0,0,
                0,0,0,1,1,1,1,1,1,0,0,0,
                0,0,0,1,1,1,1,1,1,0,0,0,
                0,0,0,1,1,1,1,1,1,0,0,0,
                0,0,1,1,1,1,1,1,1,1,0,0,
                0,0,1,1,1,1,1,1,1,1,0,0,
                0,1,1,1,1,1,1,1,1,1,1,0,
                0,1,1,1,1,1,1,1,1,1,1,0,
                1,1,0,1,1,1,1,1,1,0,1,1,
                1,0,0,0,1,1,1,1,0,0,0,1,
                0,0,0,0,1,0,0,1,0,0,0,0
            };
            for (int i = 0; i < 144; i++) bitmap[i] = pattern[i] == 1;
            data.bitmap = bitmap;

            AssetDatabase.CreateAsset(data, $"{basePath}/Stage10_Rocket.asset");
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

        // PNG to ImageInvert 변환 메서드들
        private bool[,] ConvertTextureTobitmap(Texture2D texture, float threshold)
        {
            // 텍스처를 읽을 수 있도록 설정
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null && !importer.isReadable)
            {
                importer.isReadable = true;
                importer.SaveAndReimport();
            }

            // 12x12 비트맵 생성
            bool[,] bitmap = new bool[12, 12];

            // 텍스처를 12x12로 리샘플링 (Y축 뒤집기)
            for (int y = 0; y < 12; y++)
            {
                for (int x = 0; x < 12; x++)
                {
                    // 원본 텍스처의 대응 좌표 계산 (Y축 뒤집기)
                    float u = (float)x / 11f;
                    float v = (float)(11 - y) / 11f;  // Y축을 뒤집어서 계산
                    int sourceX = Mathf.RoundToInt(u * (texture.width - 1));
                    int sourceY = Mathf.RoundToInt(v * (texture.height - 1));

                    Color pixel = texture.GetPixel(sourceX, sourceY);
                    float grayscale = pixel.grayscale; // RGB를 그레이스케일로 변환

                    // 임계값 적용 (밝으면 true, 어두우면 false)
                    bitmap[x, y] = grayscale > threshold;
                }
            }

            return bitmap;
        }

        private void DrawBitmapPreview(bool[,] bitmap)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("12x12 비트맵 미리보기:", EditorStyles.boldLabel);

            // 미리보기 영역
            Rect previewRect = GUILayoutUtility.GetRect(240, 240);

            // 배경 그리기
            EditorGUI.DrawRect(previewRect, new Color(0.2f, 0.2f, 0.2f));

            float cellSize = 20f;
            float startX = previewRect.x;
            float startY = previewRect.y;

            for (int y = 0; y < 12; y++)
            {
                for (int x = 0; x < 12; x++)
                {
                    Rect cellRect = new Rect(
                        startX + x * cellSize,
                        startY + y * cellSize,
                        cellSize - 1,
                        cellSize - 1
                    );

                    Color cellColor = bitmap[x, y] ? Color.white : Color.black;
                    EditorGUI.DrawRect(cellRect, cellColor);
                }
            }

            EditorGUILayout.Space();
        }

        private void SavePNGAsStageData(bool[,] bitmap)
        {
            string dataPath = "Assets/Resources/ImageInvertPuzzle";
            if (!AssetDatabase.IsValidFolder(dataPath))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }
                AssetDatabase.CreateFolder("Assets/Resources", "ImageInvertPuzzle");
            }

            var data = ScriptableObject.CreateInstance<ImageInvertPuzzleData>();
            data.puzzleName = stageName;
            data.initialInvertCount = initialInvertCountValue;
            data.maxMoves = maxMovesValue;
            data.timeLimit = timeLimitValue;

            // 비트맵을 1D 배열로 변환
            bool[] flatBitmap = new bool[144];
            for (int y = 0; y < 12; y++)
            {
                for (int x = 0; x < 12; x++)
                {
                    flatBitmap[y * 12 + x] = bitmap[x, y];
                }
            }
            data.bitmap = flatBitmap;

            // 파일명 생성 (공백과 특수문자 제거)
            string fileName = stageName.Replace(" ", "_");
            fileName = System.Text.RegularExpressions.Regex.Replace(fileName, @"[^a-zA-Z0-9_]", "");

            string assetPath = $"{dataPath}/Stage_{fileName}.asset";

            // 중복 파일 체크
            int counter = 1;
            string finalPath = assetPath;
            while (AssetDatabase.LoadAssetAtPath<ImageInvertPuzzleData>(finalPath) != null)
            {
                finalPath = $"{dataPath}/Stage_{fileName}_{counter}.asset";
                counter++;
            }

            AssetDatabase.CreateAsset(data, finalPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("완료",
                $"스테이지 데이터가 저장되었습니다.\n{finalPath}",
                "확인");

            // 생성된 에셋 선택
            Selection.activeObject = data;
        }
    }
}