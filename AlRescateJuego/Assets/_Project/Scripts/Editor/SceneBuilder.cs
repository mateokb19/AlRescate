using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.IO;
using System.Collections.Generic;

public static class SceneBuilder
{
    [MenuItem("AlRescate/Construir Todas las Escenas")]
    public static void BuildAllScenes()
    {
        BuildBootstrap();
        BuildMainMenu();
        BuildGachaHub();
        SetupBuildSettings();
        Debug.Log("[SceneBuilder] ¡Todas las escenas creadas y configuradas en Build Settings!");
        EditorUtility.DisplayDialog("AlRescate", "¡Escenas creadas exitosamente!\n\n" +
            "• 99_Bootstrap\n• 00_MainMenu\n• 01_GachaHub\n\n" +
            "Revisa la consola y abre cada escena para verificar.", "OK");
    }

    // ─────────────────────────────────────────────────────────────
    // BOOTSTRAP
    // ─────────────────────────────────────────────────────────────
    static void BuildBootstrap()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var boot = new GameObject("GameBootstrapper");
        boot.AddComponent<GameBootstrapper>();

        SaveScene(scene, "Assets/_Project/Scenes/99_Bootstrap.unity");
    }

    // ─────────────────────────────────────────────────────────────
    // MAIN MENU
    // ─────────────────────────────────────────────────────────────
    static void BuildMainMenu()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Camera
        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5.4f;
        cam.backgroundColor = new Color(0.12f, 0.12f, 0.18f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        camGO.tag = "MainCamera";

        // Canvas
        var canvasGO = CreateCanvas("Canvas_MainMenu", 0);
        var ctrl = new GameObject("MainMenuController");
        var mmCtrl = ctrl.AddComponent<MainMenuController>();

        // Botones
        var vGroup = CreateUIObject("VerticalGroup_Buttons", canvasGO.transform);
        var vlg = vGroup.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 20;
        vlg.childAlignment = TextAnchor.MiddleCenter;
        vlg.childForceExpandWidth = false;
        vlg.childForceExpandHeight = false;
        var vRect = vGroup.GetComponent<RectTransform>();
        vRect.anchorMin = new Vector2(0.5f, 0.5f);
        vRect.anchorMax = new Vector2(0.5f, 0.5f);
        vRect.sizeDelta = new Vector2(400, 500);
        vRect.anchoredPosition = new Vector2(0, -80);

        mmCtrl.btnPlay       = CreateButton("Btn_Play",       "JUGAR",       vGroup.transform).GetComponent<Button>();
        mmCtrl.btnCollection = CreateButton("Btn_Collection", "COLECCION",   vGroup.transform).GetComponent<Button>();
        mmCtrl.btnOptions    = CreateButton("Btn_Options",    "OPCIONES",    vGroup.transform).GetComponent<Button>();
        mmCtrl.btnCredits    = CreateButton("Btn_Credits",    "CREDITOS",    vGroup.transform).GetComponent<Button>();
        mmCtrl.btnQuit       = CreateButton("Btn_Quit",       "SALIR",       vGroup.transform).GetComponent<Button>();

        // Logo placeholder
        var logo = CreateUIObject("Logo_AlRescate", canvasGO.transform);
        var logoRect = logo.GetComponent<RectTransform>();
        logoRect.anchorMin = new Vector2(0.5f, 1f);
        logoRect.anchorMax = new Vector2(0.5f, 1f);
        logoRect.pivot = new Vector2(0.5f, 1f);
        logoRect.anchoredPosition = new Vector2(0, -60);
        logoRect.sizeDelta = new Vector2(600, 200);
        var logoText = logo.AddComponent<TextMeshProUGUI>();
        logoText.text = "AL RESCATE";
        logoText.fontSize = 72;
        logoText.alignment = TextAlignmentOptions.Center;
        logoText.color = new Color(0.96f, 0.62f, 0.04f);

        // Footer Cruz Roja
        var footer = CreateUIObject("Footer_CruzRoja", canvasGO.transform);
        var footerRect = footer.GetComponent<RectTransform>();
        footerRect.anchorMin = new Vector2(0, 0);
        footerRect.anchorMax = new Vector2(1, 0);
        footerRect.pivot = new Vector2(0.5f, 0f);
        footerRect.anchoredPosition = new Vector2(0, 20);
        footerRect.sizeDelta = new Vector2(0, 60);
        var footerText = footer.AddComponent<TextMeshProUGUI>();
        footerText.text = "Apoyando a la Cruz Roja";
        footerText.fontSize = 24;
        footerText.alignment = TextAlignmentOptions.Center;
        footerText.color = new Color(0.85f, 0.15f, 0.15f);

        // EventSystem
        CreateEventSystem();

        SaveScene(scene, "Assets/_Project/Scenes/00_MainMenu.unity");
    }

    // ─────────────────────────────────────────────────────────────
    // GACHA HUB
    // ─────────────────────────────────────────────────────────────
    static void BuildGachaHub()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Camera
        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5.4f;
        cam.backgroundColor = new Color(0.08f, 0.08f, 0.14f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        camGO.tag = "MainCamera";

        // Managers
        var audioMgrGO = new GameObject("AudioManager");
        audioMgrGO.AddComponent<AudioManager>();

        var gachaMgrGO = new GameObject("GachaManager");
        gachaMgrGO.AddComponent<GachaManager>();

        var dbGO = new GameObject("GachaDatabase");
        dbGO.AddComponent<GachaDatabase>();

        var invGO = new GameObject("PlayerInventory");
        invGO.AddComponent<PlayerInventory>();

        // Pet spawn point
        var petSpawn = new GameObject("PetSpawnPoint");
        petSpawn.transform.position = new Vector3(-3f, 0f, 0f);
        petSpawn.AddComponent<PetController>();

        // Particle container
        new GameObject("ParticleFX_Container");

        // ── HUD Canvas (Sort Order 0)
        var canvasHUD = CreateCanvas("Canvas_HUD", 0);

        // TopBar
        var topBar = CreateUIObject("TopBar", canvasHUD.transform);
        var topRect = topBar.GetComponent<RectTransform>();
        topRect.anchorMin = new Vector2(0, 1);
        topRect.anchorMax = new Vector2(1, 1);
        topRect.pivot = new Vector2(0.5f, 1f);
        topRect.anchoredPosition = Vector2.zero;
        topRect.sizeDelta = new Vector2(0, 120);
        var topHlg = topBar.AddComponent<HorizontalLayoutGroup>();
        topHlg.padding = new RectOffset(20, 20, 20, 20);
        topHlg.spacing = 20;
        topHlg.childAlignment = TextAnchor.MiddleLeft;
        topHlg.childForceExpandWidth = false;
        topHlg.childForceExpandHeight = false;

        // GemCounter
        var gemCounterGO = CreateUIObject("GemCounter", topBar.transform);
        var gemDisplay = gemCounterGO.AddComponent<CurrencyDisplay>();
        var gemRect = gemCounterGO.GetComponent<RectTransform>();
        gemRect.sizeDelta = new Vector2(200, 60);
        var gemTextGO = CreateUIObject("GemText", gemCounterGO.transform);
        var gemText = gemTextGO.AddComponent<TextMeshProUGUI>();
        gemText.text = "0 Gemas";
        gemText.fontSize = 32;
        gemText.color = new Color(1f, 0.85f, 0.1f);
        var gemTextRect = gemTextGO.GetComponent<RectTransform>();
        gemTextRect.anchorMin = Vector2.zero;
        gemTextRect.anchorMax = Vector2.one;
        gemTextRect.sizeDelta = Vector2.zero;
        gemDisplay.gemText = gemText;

        var btnShop = CreateButton("ShopButton", "+ Gemas", topBar.transform, new Vector2(160, 60));

        // BottomBar
        var bottomBar = CreateUIObject("BottomBar", canvasHUD.transform);
        var botRect = bottomBar.GetComponent<RectTransform>();
        botRect.anchorMin = new Vector2(0, 0);
        botRect.anchorMax = new Vector2(1, 0);
        botRect.pivot = new Vector2(0.5f, 0f);
        botRect.anchoredPosition = Vector2.zero;
        botRect.sizeDelta = new Vector2(0, 160);
        var botHlg = bottomBar.AddComponent<HorizontalLayoutGroup>();
        botHlg.padding = new RectOffset(30, 30, 30, 30);
        botHlg.spacing = 30;
        botHlg.childAlignment = TextAnchor.MiddleCenter;
        botHlg.childForceExpandWidth = false;
        botHlg.childForceExpandHeight = false;

        var btnX1  = CreateButton("Btn_PullX1",    "TIRAR x1\n160 Gemas",  bottomBar.transform, new Vector2(280, 100));
        var btnX10 = CreateButton("Btn_PullX10",   "TIRAR x10\n1440 Gemas",bottomBar.transform, new Vector2(280, 100));
        var btnCol = CreateButton("Btn_Collection","COLECCION",             bottomBar.transform, new Vector2(200, 100));

        // Options button (top-right)
        var btnOpt = CreateButton("Btn_Options", "OPT", canvasHUD.transform, new Vector2(80, 80));
        var optRect = btnOpt.GetComponent<RectTransform>();
        optRect.anchorMin = new Vector2(1, 1);
        optRect.anchorMax = new Vector2(1, 1);
        optRect.pivot = new Vector2(1, 1);
        optRect.anchoredPosition = new Vector2(-20, -20);

        // PityBar
        var pityGO = CreateUIObject("PityBar", canvasHUD.transform);
        var pityRect = pityGO.GetComponent<RectTransform>();
        pityRect.anchorMin = new Vector2(0.5f, 0f);
        pityRect.anchorMax = new Vector2(0.5f, 0f);
        pityRect.pivot = new Vector2(0.5f, 0f);
        pityRect.anchoredPosition = new Vector2(0, 165);
        pityRect.sizeDelta = new Vector2(600, 20);
        pityGO.AddComponent<Slider>();

        // ── Result Canvas (Sort Order 10, inactive)
        var canvasResult = CreateCanvas("Canvas_Result", 10);
        canvasResult.SetActive(false);
        var dimResult = CreateDimmer(canvasResult.transform);
        var resultRoot = CreateUIObject("ResultRoot", canvasResult.transform);
        var rrRect = resultRoot.GetComponent<RectTransform>();
        rrRect.anchorMin = new Vector2(0.05f, 0.1f);
        rrRect.anchorMax = new Vector2(0.95f, 0.95f);
        rrRect.sizeDelta = Vector2.zero;
        var cardContainer = CreateUIObject("ResultCardContainer", resultRoot.transform);
        var ccRect = cardContainer.GetComponent<RectTransform>();
        ccRect.anchorMin = new Vector2(0, 0.2f);
        ccRect.anchorMax = new Vector2(1, 1);
        ccRect.sizeDelta = Vector2.zero;
        var ccHlg = cardContainer.AddComponent<HorizontalLayoutGroup>();
        ccHlg.childAlignment = TextAnchor.MiddleCenter;
        ccHlg.spacing = 10;
        ccHlg.childForceExpandWidth = false;
        ccHlg.childForceExpandHeight = false;

        var beamGO = CreateUIObject("BeamOfLight", resultRoot.transform);
        var beamRect = beamGO.GetComponent<RectTransform>();
        beamRect.anchorMin = new Vector2(0.5f, 0);
        beamRect.anchorMax = new Vector2(0.5f, 0);
        beamRect.pivot = new Vector2(0.5f, 0);
        beamRect.anchoredPosition = Vector2.zero;
        beamRect.sizeDelta = new Vector2(200, 1200);
        var beamImg = beamGO.AddComponent<Image>();
        beamImg.color = new Color(0.3f, 0.45f, 1f, 0.6f);
        beamGO.SetActive(false);

        var btnPullAgain = CreateButton("Btn_PullAgain", "TIRAR DE NUEVO", resultRoot.transform, new Vector2(300, 80));
        var bpaRect = btnPullAgain.GetComponent<RectTransform>();
        bpaRect.anchorMin = new Vector2(0.3f, 0);
        bpaRect.anchorMax = new Vector2(0.3f, 0);
        bpaRect.anchoredPosition = new Vector2(0, 40);

        var btnCloseResult = CreateButton("Btn_Close", "CERRAR", resultRoot.transform, new Vector2(200, 80));
        var bcrRect = btnCloseResult.GetComponent<RectTransform>();
        bcrRect.anchorMin = new Vector2(0.7f, 0);
        bcrRect.anchorMax = new Vector2(0.7f, 0);
        bcrRect.anchoredPosition = new Vector2(0, 40);

        var btnSkip = CreateButton("Btn_Skip", "SALTAR", canvasResult.transform, new Vector2(160, 60));
        var bsRect = btnSkip.GetComponent<RectTransform>();
        bsRect.anchorMin = new Vector2(1, 1);
        bsRect.anchorMax = new Vector2(1, 1);
        bsRect.pivot = new Vector2(1, 1);
        bsRect.anchoredPosition = new Vector2(-20, -20);
        btnSkip.gameObject.SetActive(false);

        // Reveal controller
        var revealCtrl = canvasResult.AddComponent<RevealAnimationController>();
        revealCtrl.canvasResult = canvasResult;
        revealCtrl.resultRoot = resultRoot.GetComponent<RectTransform>();
        revealCtrl.beamOfLight = beamImg;
        revealCtrl.cardContainer = cardContainer.transform;
        revealCtrl.btnPullAgain = btnPullAgain.GetComponent<Button>();
        revealCtrl.btnClose = btnCloseResult.GetComponent<Button>();
        revealCtrl.btnSkip = btnSkip.GetComponent<Button>();

        // ── Collection Canvas (Sort Order 20, inactive)
        var canvasCollection = CreateCanvas("Canvas_Collection", 20);
        canvasCollection.SetActive(false);
        CreateDimmer(canvasCollection.transform);
        var colRoot = CreateUIObject("CollectionRoot", canvasCollection.transform);
        var colRootRect = colRoot.GetComponent<RectTransform>();
        colRootRect.anchorMin = new Vector2(0.05f, 0.05f);
        colRootRect.anchorMax = new Vector2(0.95f, 0.95f);
        colRootRect.sizeDelta = Vector2.zero;
        var scrollView = CreateUIObject("ScrollView_Grid", colRoot.transform);
        scrollView.AddComponent<ScrollRect>();
        var btnCloseCol = CreateButton("Btn_Close_Collection", "X", canvasCollection.transform, new Vector2(80, 80));
        var bccRect = btnCloseCol.GetComponent<RectTransform>();
        bccRect.anchorMin = new Vector2(1, 1);
        bccRect.anchorMax = new Vector2(1, 1);
        bccRect.pivot = new Vector2(1, 1);
        bccRect.anchoredPosition = new Vector2(-20, -20);
        var btnCloseColComp = btnCloseCol.GetComponent<Button>();
        btnCloseColComp.onClick.AddListener(() => canvasCollection.SetActive(false));

        // ── Shop Canvas (Sort Order 30, inactive)
        var canvasShop = CreateCanvas("Canvas_Shop", 30);
        canvasShop.SetActive(false);
        CreateDimmer(canvasShop.transform);
        var shopRoot = CreateUIObject("ShopRoot", canvasShop.transform);
        var shopRootRect = shopRoot.GetComponent<RectTransform>();
        shopRootRect.anchorMin = new Vector2(0.05f, 0.05f);
        shopRootRect.anchorMax = new Vector2(0.95f, 0.95f);
        shopRootRect.sizeDelta = Vector2.zero;
        var shopGrid = CreateUIObject("ScrollView_Offers", shopRoot.transform);
        shopGrid.AddComponent<ScrollRect>();
        var btnCloseShop = CreateButton("Btn_Close_Shop", "X", canvasShop.transform, new Vector2(80, 80));
        var bcsRect = btnCloseShop.GetComponent<RectTransform>();
        bcsRect.anchorMin = new Vector2(1, 1);
        bcsRect.anchorMax = new Vector2(1, 1);
        bcsRect.pivot = new Vector2(1, 1);
        bcsRect.anchoredPosition = new Vector2(-20, -20);
        var btnCloseShopComp = btnCloseShop.GetComponent<Button>();
        btnCloseShopComp.onClick.AddListener(() => canvasShop.SetActive(false));

        // Welcome Panel (inactive)
        var welcomePanel = CreateUIObject("WelcomePanel", canvasHUD.transform);
        var wpRect = welcomePanel.GetComponent<RectTransform>();
        wpRect.anchorMin = new Vector2(0.1f, 0.2f);
        wpRect.anchorMax = new Vector2(0.9f, 0.8f);
        wpRect.sizeDelta = Vector2.zero;
        var wpBg = welcomePanel.AddComponent<Image>();
        wpBg.color = new Color(0.1f, 0.1f, 0.2f, 0.95f);
        var wpText = CreateUIObject("WelcomeText", welcomePanel.transform);
        var wpTmp = wpText.AddComponent<TextMeshProUGUI>();
        wpTmp.text = "¡Bienvenido a Al Rescate!\n\nAquí tienes 1.600 Gemas de regalo.";
        wpTmp.fontSize = 36;
        wpTmp.alignment = TextAlignmentOptions.Center;
        var wpTextRect = wpText.GetComponent<RectTransform>();
        wpTextRect.anchorMin = new Vector2(0.1f, 0.3f);
        wpTextRect.anchorMax = new Vector2(0.9f, 0.9f);
        wpTextRect.sizeDelta = Vector2.zero;
        var btnWelcomeClose = CreateButton("Btn_WelcomeClose", "¡VAMOS!", welcomePanel.transform, new Vector2(280, 80));
        var bwRect = btnWelcomeClose.GetComponent<RectTransform>();
        bwRect.anchorMin = new Vector2(0.5f, 0.1f);
        bwRect.anchorMax = new Vector2(0.5f, 0.1f);
        bwRect.anchoredPosition = Vector2.zero;
        btnWelcomeClose.GetComponent<Button>().onClick.AddListener(() => welcomePanel.SetActive(false));
        welcomePanel.SetActive(false);

        // ── Hub Controller
        var hubCtrl = new GameObject("HubController");
        var gachaUI = hubCtrl.AddComponent<GachaUIController>();
        gachaUI.btnPullX1 = btnX1.GetComponent<Button>();
        gachaUI.btnPullX10 = btnX10.GetComponent<Button>();
        gachaUI.btnShop = btnShop.GetComponent<Button>();
        gachaUI.btnCollection = btnCol.GetComponent<Button>();
        gachaUI.btnOptions = btnOpt.GetComponent<Button>();
        gachaUI.canvasResult = canvasResult;
        gachaUI.canvasShop = canvasShop;
        gachaUI.canvasCollection = canvasCollection;
        gachaUI.revealController = revealCtrl;
        gachaUI.welcomePanel = welcomePanel;

        // EventSystem
        CreateEventSystem();

        SaveScene(scene, "Assets/_Project/Scenes/01_GachaHub.unity");
    }

    // ─────────────────────────────────────────────────────────────
    // REPARAR COLECCION
    // ─────────────────────────────────────────────────────────────
    [MenuItem("AlRescate/Reparar Panel Coleccion")]
    public static void FixCollectionPanel()
    {
        // Buscar Canvas_Collection incluyendo objetos inactivos
        GameObject canvasCol = null;
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.name == "Canvas_Collection" && go.scene.isLoaded)
            { canvasCol = go; break; }
        }
        if (canvasCol == null) { Debug.LogError("No se encontró Canvas_Collection en la escena."); return; }

        var colRoot = canvasCol.transform.Find("CollectionRoot");
        if (colRoot == null) { Debug.LogError("No se encontró CollectionRoot."); return; }

        // Eliminar ScrollView_Grid viejo si existe
        var oldScroll = colRoot.Find("ScrollView_Grid");
        if (oldScroll != null) Object.DestroyImmediate(oldScroll.gameObject);

        // Crear ScrollView correctamente
        var scrollGO = new GameObject("ScrollView_Grid");
        scrollGO.transform.SetParent(colRoot, false);
        var scrollRect_RT = scrollGO.AddComponent<RectTransform>();
        scrollRect_RT.anchorMin = Vector2.zero;
        scrollRect_RT.anchorMax = Vector2.one;
        scrollRect_RT.offsetMin = new Vector2(0, 60);
        scrollRect_RT.offsetMax = new Vector2(0, -10);
        var scrollRect = scrollGO.AddComponent<ScrollRect>();
        scrollGO.AddComponent<Image>().color = new Color(0, 0, 0, 0.01f);

        // Viewport
        var viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollGO.transform, false);
        var vpRect = viewport.AddComponent<RectTransform>();
        vpRect.anchorMin = Vector2.zero;
        vpRect.anchorMax = Vector2.one;
        vpRect.sizeDelta = Vector2.zero;
        vpRect.pivot = new Vector2(0, 1);
        viewport.AddComponent<RectMask2D>();

        // Content
        var content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        var contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.sizeDelta = new Vector2(0, 0);
        var grid = content.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(170, 220);
        grid.spacing = new Vector2(15, 15);
        grid.padding = new RectOffset(15, 15, 15, 15);
        grid.childAlignment = TextAnchor.UpperCenter;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 5;
        var csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Conectar ScrollRect
        scrollRect.content = contentRect;
        scrollRect.viewport = vpRect;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.scrollSensitivity = 30;

        // Agregar o actualizar CollectionUI
        var collUI = colRoot.gameObject.GetComponent<CollectionUI>();
        if (collUI == null) collUI = colRoot.gameObject.AddComponent<CollectionUI>();
        collUI.gridPets = contentRect;
        collUI.gridItems = contentRect;

        // Botón cerrar
        var existingClose = canvasCol.transform.Find("CollectionRoot/Btn_Close_Collection");
        if (existingClose == null)
        {
            var btnClose = CreateButton("Btn_Close_Collection", "X", colRoot, new Vector2(80, 80));
            var bRect = btnClose.GetComponent<RectTransform>();
            bRect.anchorMin = new Vector2(1, 1);
            bRect.anchorMax = new Vector2(1, 1);
            bRect.pivot = new Vector2(1, 1);
            bRect.anchoredPosition = new Vector2(-10, -10);
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("[SceneBuilder] Panel Coleccion reparado. Asigna Database y CellPrefab en CollectionUI.");
        EditorUtility.DisplayDialog("AlRescate",
            "Panel Coleccion reparado.\n\nAhora en el componente CollectionUI de CollectionRoot asigna:\n" +
            "• Database → GachaDatabase\n• Cell Prefab → CollectionCell", "OK");
    }

    // ─────────────────────────────────────────────────────────────
    // BUILD SETTINGS
    // ─────────────────────────────────────────────────────────────
    static void SetupBuildSettings()
    {
        var scenes = new[]
        {
            new EditorBuildSettingsScene("Assets/_Project/Scenes/99_Bootstrap.unity", true),
            new EditorBuildSettingsScene("Assets/_Project/Scenes/00_MainMenu.unity",  true),
            new EditorBuildSettingsScene("Assets/_Project/Scenes/01_GachaHub.unity",  true),
        };
        EditorBuildSettings.scenes = scenes;
    }

    // ─────────────────────────────────────────────────────────────
    // HELPERS
    // ─────────────────────────────────────────────────────────────
    static GameObject CreateCanvas(string name, int sortOrder)
    {
        var go = new GameObject(name);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortOrder;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        return go;
    }

    static GameObject CreateUIObject(string name, Transform parent)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        return go;
    }

    static GameObject CreateButton(string name, string label, Transform parent, Vector2 size = default)
    {
        if (size == default) size = new Vector2(300, 88);
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        var img = go.AddComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.25f);
        var btn = go.AddComponent<Button>();
        var cb = btn.colors;
        cb.highlightedColor = new Color(0.25f, 0.25f, 0.4f);
        cb.pressedColor = new Color(0.1f, 0.1f, 0.18f);
        btn.colors = cb;

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        var textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 28;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        return go;
    }

    static GameObject CreateDimmer(Transform parent)
    {
        var go = new GameObject("DimmerBackground");
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        var img = go.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.75f);
        return go;
    }

    static void CreateEventSystem()
    {
        var esGO = new GameObject("EventSystem");
        esGO.AddComponent<EventSystem>();
        esGO.AddComponent<StandaloneInputModule>();
    }

    static void SaveScene(Scene scene, string path)
    {
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        EditorSceneManager.SaveScene(scene, path);
        AssetDatabase.Refresh();
    }
}
