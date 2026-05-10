using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[InitializeOnLoad]
public static class SetupPhotonUiScenes
{
    private const string LobbyScenePath = "Assets/2.Scenes/Scenes/Garden/Lobby.unity";
    private const string RoomScenePath = "Assets/2.Scenes/Scenes/Garden/Room.unity";
    private const string EditorPrefsKey = "PhotonNetProject.UiHierarchySetup.v1";

    static SetupPhotonUiScenes()
    {
        EditorApplication.delayCall += RunOnceAfterCompile;
    }

    [MenuItem("Tools/Photon/Setup Lobby And Room UI")]
    public static void Run()
    {
        SetupLobbyScene();
        SetupRoomScene();
        AssetDatabase.SaveAssets();
        EditorPrefs.SetBool(EditorPrefsKey, true);
    }

    private static void RunOnceAfterCompile()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorPrefs.GetBool(EditorPrefsKey, false))
        {
            return;
        }

        Run();
    }

    private static void SetupLobbyScene()
    {
        EditorSceneManager.OpenScene(LobbyScenePath);

        LobbyManager lobbyManager = Object.FindFirstObjectByType<LobbyManager>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (lobbyManager == null || canvas == null)
        {
            throw new MissingReferenceException("Lobby scene needs LobbyManager and Canvas.");
        }

        ClearCanvas(canvas);
        AddCanvasBackground(canvas, new Color(0.04f, 0.07f, 0.1f, 1f));

        RectTransform root = CreateRect("Lobby Root", canvas.transform);
        Stretch(root, 64f, 64f, 64f, 64f);
        HorizontalLayoutGroup rootLayout = root.gameObject.AddComponent<HorizontalLayoutGroup>();
        rootLayout.spacing = 28f;
        rootLayout.childControlWidth = true;
        rootLayout.childControlHeight = true;
        rootLayout.childForceExpandWidth = true;
        rootLayout.childForceExpandHeight = true;

        RectTransform actionPanel = CreatePanel("Action Panel", root, new Color(0.08f, 0.12f, 0.16f, 0.95f));
        AddLayoutElement(actionPanel, 420f, 0f, 0.75f, 1f);
        VerticalLayoutGroup actionGroup = actionPanel.gameObject.AddComponent<VerticalLayoutGroup>();
        actionGroup.padding = new RectOffset(40, 40, 40, 40);
        actionGroup.spacing = 24f;
        actionGroup.childControlWidth = true;
        actionGroup.childForceExpandWidth = true;
        actionGroup.childForceExpandHeight = false;

        CreateText("Lobby Title Text", actionPanel, "Lobby", 54f, FontStyles.Bold, TextAlignmentOptions.Left, new Color(0.95f, 0.98f, 1f, 1f));
        CreateText("Lobby Description Text", actionPanel, "Find a room or create a new one automatically.", 23f, FontStyles.Normal, TextAlignmentOptions.Left, new Color(0.64f, 0.74f, 0.8f, 1f));
        Button createRoomButton = CreateButton("Enter Room Button", actionPanel, "Enter Room", 34f, new Color(0.12f, 0.48f, 0.56f, 1f), 92f);
        AddButtonListener(createRoomButton, lobbyManager.OnCreateRoomButtonClicked);
        TMP_Text statusText = CreateText("Lobby Status Text", actionPanel, "Connecting...", 22f, FontStyles.Bold, TextAlignmentOptions.Left, new Color(0.96f, 0.78f, 0.42f, 1f));

        RectTransform listPanel = CreatePanel("Room List Panel", root, new Color(0.06f, 0.09f, 0.12f, 0.96f));
        AddLayoutElement(listPanel, 620f, 0f, 1.25f, 1f);
        VerticalLayoutGroup listGroup = listPanel.gameObject.AddComponent<VerticalLayoutGroup>();
        listGroup.padding = new RectOffset(32, 32, 32, 32);
        listGroup.spacing = 18f;
        listGroup.childControlWidth = true;
        listGroup.childForceExpandWidth = true;
        listGroup.childForceExpandHeight = false;

        CreateText("Open Rooms Title Text", listPanel, "Open Rooms", 38f, FontStyles.Bold, TextAlignmentOptions.Left, Color.white);

        RectTransform scrollRoot = CreatePanel("Room List View", listPanel, new Color(0.03f, 0.05f, 0.07f, 0.72f));
        AddLayoutElement(scrollRoot, 0f, 560f, 1f, 1f);
        ScrollRect scrollRect = scrollRoot.gameObject.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;

        RectTransform viewport = CreateRect("Viewport", scrollRoot);
        Stretch(viewport, 14f, 14f, 14f, 14f);
        Image viewportImage = viewport.gameObject.AddComponent<Image>();
        viewportImage.color = new Color(1f, 1f, 1f, 0.02f);
        viewport.gameObject.AddComponent<Mask>().showMaskGraphic = false;

        RectTransform content = CreateRect("Content", viewport);
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);
        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = Vector2.zero;
        VerticalLayoutGroup contentGroup = content.gameObject.AddComponent<VerticalLayoutGroup>();
        contentGroup.spacing = 12f;
        contentGroup.childControlWidth = true;
        contentGroup.childControlHeight = true;
        contentGroup.childForceExpandWidth = true;
        contentGroup.childForceExpandHeight = false;
        content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        RectTransform rowTemplate = CreateRoomRowTemplate(content);
        TMP_Text emptyText = CreateText("Empty Room List Text", viewport, "No rooms yet.", 26f, FontStyles.Normal, TextAlignmentOptions.Center, new Color(0.52f, 0.62f, 0.68f, 1f));
        Stretch(emptyText.rectTransform, 0f, 0f, 0f, 0f);

        scrollRect.viewport = viewport;
        scrollRect.content = content;

        SerializedObject serializedManager = new SerializedObject(lobbyManager);
        serializedManager.FindProperty("createRoomButton").objectReferenceValue = createRoomButton;
        serializedManager.FindProperty("statusText").objectReferenceValue = statusText;
        serializedManager.FindProperty("roomListContent").objectReferenceValue = content;
        serializedManager.FindProperty("roomRowTemplate").objectReferenceValue = rowTemplate;
        serializedManager.FindProperty("emptyRoomListText").objectReferenceValue = emptyText;
        serializedManager.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
        EditorSceneManager.SaveScene(canvas.gameObject.scene);
    }

    private static void SetupRoomScene()
    {
        EditorSceneManager.OpenScene(RoomScenePath);

        RoomManager roomManager = Object.FindFirstObjectByType<RoomManager>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (roomManager == null || canvas == null)
        {
            throw new MissingReferenceException("Room scene needs RoomManager and Canvas.");
        }

        ClearCanvas(canvas);
        AddCanvasBackground(canvas, new Color(0.045f, 0.065f, 0.075f, 1f));

        RectTransform root = CreateRect("Room Root", canvas.transform);
        Stretch(root, 90f, 80f, 90f, 80f);
        HorizontalLayoutGroup rootGroup = root.gameObject.AddComponent<HorizontalLayoutGroup>();
        rootGroup.spacing = 30f;
        rootGroup.childControlWidth = true;
        rootGroup.childControlHeight = true;
        rootGroup.childForceExpandWidth = true;
        rootGroup.childForceExpandHeight = true;

        RectTransform rosterPanel = CreatePanel("Roster Panel", root, new Color(0.07f, 0.1f, 0.12f, 0.96f));
        AddLayoutElement(rosterPanel, 720f, 0f, 1.25f, 1f);
        VerticalLayoutGroup rosterGroup = rosterPanel.gameObject.AddComponent<VerticalLayoutGroup>();
        rosterGroup.padding = new RectOffset(38, 38, 34, 34);
        rosterGroup.spacing = 18f;
        rosterGroup.childControlWidth = true;
        rosterGroup.childForceExpandWidth = true;
        rosterGroup.childForceExpandHeight = false;

        TMP_Text roomNameText = CreateText("Room Name Text", rosterPanel, "Room", 44f, FontStyles.Bold, TextAlignmentOptions.Left, Color.white);
        TMP_Text playerCountText = CreateText("Player Count Text", rosterPanel, "0 players", 24f, FontStyles.Bold, TextAlignmentOptions.Left, new Color(0.94f, 0.72f, 0.34f, 1f));

        RectTransform playerListPanel = CreatePanel("Player List Panel", rosterPanel, new Color(0.035f, 0.055f, 0.065f, 0.78f));
        AddLayoutElement(playerListPanel, 0f, 520f, 1f, 1f);
        TMP_Text playerListText = CreateText("Player List Text", playerListPanel, string.Empty, 30f, FontStyles.Normal, TextAlignmentOptions.TopLeft, new Color(0.88f, 0.95f, 0.98f, 1f));
        Stretch(playerListText.rectTransform, 28f, 26f, 28f, 26f);

        RectTransform actionPanel = CreatePanel("Action Panel", root, new Color(0.1f, 0.13f, 0.14f, 0.94f));
        AddLayoutElement(actionPanel, 430f, 0f, 0.75f, 1f);
        VerticalLayoutGroup actionGroup = actionPanel.gameObject.AddComponent<VerticalLayoutGroup>();
        actionGroup.padding = new RectOffset(36, 36, 40, 40);
        actionGroup.spacing = 24f;
        actionGroup.childControlWidth = true;
        actionGroup.childForceExpandWidth = true;
        actionGroup.childForceExpandHeight = false;

        CreateText("Ready Room Title Text", actionPanel, "Ready Room", 42f, FontStyles.Bold, TextAlignmentOptions.Left, Color.white);
        CreateText("Ready Room Description Text", actionPanel, "Everyone in this room will move to the garden when the scene is loaded.", 22f, FontStyles.Normal, TextAlignmentOptions.Left, new Color(0.66f, 0.76f, 0.78f, 1f));
        Button enterGameButton = CreateButton("Enter Garden Button", actionPanel, "Enter Garden", 32f, new Color(0.18f, 0.5f, 0.42f, 1f), 92f);
        AddButtonListener(enterGameButton, roomManager.OnEnterGameButtonClicked);

        SerializedObject serializedManager = new SerializedObject(roomManager);
        serializedManager.FindProperty("roomNameText").objectReferenceValue = roomNameText;
        serializedManager.FindProperty("playerCountText").objectReferenceValue = playerCountText;
        serializedManager.FindProperty("playerListText").objectReferenceValue = playerListText;
        serializedManager.FindProperty("enterGameButton").objectReferenceValue = enterGameButton;
        serializedManager.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
        EditorSceneManager.SaveScene(canvas.gameObject.scene);
    }

    private static RectTransform CreateRoomRowTemplate(Transform parent)
    {
        RectTransform row = CreatePanel("Room Row Template", parent, new Color(0.1f, 0.14f, 0.18f, 1f));
        row.gameObject.SetActive(false);
        AddLayoutElement(row, 0f, 86f, 1f, 0f);

        HorizontalLayoutGroup rowGroup = row.gameObject.AddComponent<HorizontalLayoutGroup>();
        rowGroup.padding = new RectOffset(22, 18, 12, 12);
        rowGroup.spacing = 16f;
        rowGroup.childAlignment = TextAnchor.MiddleCenter;
        rowGroup.childControlWidth = true;
        rowGroup.childControlHeight = true;

        RectTransform infoRoot = CreateRect("Room Info", row);
        AddLayoutElement(infoRoot, 0f, 0f, 1f, 1f);
        VerticalLayoutGroup infoGroup = infoRoot.gameObject.AddComponent<VerticalLayoutGroup>();
        infoGroup.spacing = 2f;
        infoGroup.childControlWidth = true;
        infoGroup.childControlHeight = true;
        infoGroup.childForceExpandWidth = true;
        infoGroup.childForceExpandHeight = false;

        CreateText("Room Name Text", infoRoot, "Room Name", 24f, FontStyles.Bold, TextAlignmentOptions.Left, Color.white);
        CreateText("Player Count Text", infoRoot, "0/2 players", 18f, FontStyles.Normal, TextAlignmentOptions.Left, new Color(0.64f, 0.74f, 0.8f, 1f));

        Button joinButton = CreateButton("Join Room Button", row, "Join", 20f, new Color(0.18f, 0.54f, 0.48f, 1f), 56f);
        AddLayoutElement(joinButton.GetComponent<RectTransform>(), 140f, 56f, 0f, 0f);
        return row;
    }

    private static RectTransform CreateRect(string name, Transform parent)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform));
        gameObject.layer = 5;
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        return rect;
    }

    private static RectTransform CreatePanel(string name, Transform parent, Color color)
    {
        RectTransform rect = CreateRect(name, parent);
        Image image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = true;
        return rect;
    }

    private static TMP_Text CreateText(string name, Transform parent, string text, float size, FontStyles style, TextAlignmentOptions alignment, Color color)
    {
        RectTransform rect = CreateRect(name, parent);
        TextMeshProUGUI label = rect.gameObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = size;
        label.fontStyle = style;
        label.alignment = alignment;
        label.color = color;
        label.raycastTarget = false;
        label.textWrappingMode = TextWrappingModes.Normal;
        return label;
    }

    private static Button CreateButton(string name, Transform parent, string label, float textSize, Color color, float height)
    {
        RectTransform rect = CreatePanel(name, parent, color);
        AddLayoutElement(rect, 0f, height, 1f, 0f);
        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = rect.gameObject.GetComponent<Image>();

        TMP_Text text = CreateText("Label", rect, label, textSize, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
        Stretch(text.rectTransform, 16f, 12f, 16f, 12f);
        return button;
    }

    private static void AddButtonListener(Button button, UnityAction action)
    {
        button.onClick = new Button.ButtonClickedEvent();
        UnityEventTools.AddPersistentListener(button.onClick, action);
    }

    private static void AddCanvasBackground(Canvas canvas, Color color)
    {
        Image background = canvas.GetComponent<Image>();
        if (background == null)
        {
            background = canvas.gameObject.AddComponent<Image>();
        }

        background.color = color;
        background.raycastTarget = false;
    }

    private static void ClearCanvas(Canvas canvas)
    {
        for (int i = canvas.transform.childCount - 1; i >= 0; i--)
        {
            Object.DestroyImmediate(canvas.transform.GetChild(i).gameObject);
        }
    }

    private static void AddLayoutElement(RectTransform rect, float minWidth, float minHeight, float flexibleWidth, float flexibleHeight)
    {
        LayoutElement layout = rect.gameObject.GetComponent<LayoutElement>();
        if (layout == null)
        {
            layout = rect.gameObject.AddComponent<LayoutElement>();
        }

        layout.minWidth = minWidth;
        layout.minHeight = minHeight;
        layout.flexibleWidth = flexibleWidth;
        layout.flexibleHeight = flexibleHeight;
    }

    private static void Stretch(RectTransform rect, float left, float top, float right, float bottom)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, -top);
    }
}
