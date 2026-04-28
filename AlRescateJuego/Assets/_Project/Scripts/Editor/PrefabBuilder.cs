using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public static class PrefabBuilder
{
    [MenuItem("AlRescate/Crear Prefab ResultCard")]
    public static void CreateResultCard()
    {
        // Raiz
        var root = new GameObject("ResultCard");
        var rootRect = root.AddComponent<RectTransform>();
        rootRect.sizeDelta = new Vector2(300, 420);
        var rootImg = root.AddComponent<Image>();
        rootImg.color = new Color(0.1f, 0.1f, 0.18f, 0.97f);
        var view = root.AddComponent<ResultCardView>();

        // Frame (borde de color de rareza)
        var frame = new GameObject("Frame");
        frame.transform.SetParent(root.transform, false);
        var frameRect = frame.AddComponent<RectTransform>();
        frameRect.anchorMin = Vector2.zero;
        frameRect.anchorMax = Vector2.one;
        frameRect.offsetMin = new Vector2(-6, -6);
        frameRect.offsetMax = new Vector2(6, 6);
        var frameImg = frame.AddComponent<Image>();
        frameImg.color = new Color(0.6f, 0.6f, 0.6f, 1f);
        frame.transform.SetAsFirstSibling();

        // Artwork
        var art = new GameObject("Artwork");
        art.transform.SetParent(root.transform, false);
        var artRect = art.AddComponent<RectTransform>();
        artRect.anchorMin = new Vector2(0, 0.25f);
        artRect.anchorMax = new Vector2(1, 0.85f);
        artRect.offsetMin = new Vector2(10, 0);
        artRect.offsetMax = new Vector2(-10, 0);
        var artImg = art.AddComponent<Image>();
        artImg.color = new Color(0.2f, 0.2f, 0.3f, 1f);
        artImg.preserveAspect = true;

        // RarityText (arriba)
        var rarityGO = new GameObject("RarityText");
        rarityGO.transform.SetParent(root.transform, false);
        var rarityRect = rarityGO.AddComponent<RectTransform>();
        rarityRect.anchorMin = new Vector2(0, 0.84f);
        rarityRect.anchorMax = new Vector2(1, 1f);
        rarityRect.offsetMin = new Vector2(8, 0);
        rarityRect.offsetMax = new Vector2(-8, -8);
        var rarityTmp = rarityGO.AddComponent<TextMeshProUGUI>();
        rarityTmp.text = "RAREZA";
        rarityTmp.fontSize = 22;
        rarityTmp.fontStyle = FontStyles.Bold;
        rarityTmp.alignment = TextAlignmentOptions.Center;
        rarityTmp.color = Color.yellow;

        // NameText (abajo)
        var nameGO = new GameObject("NameText");
        nameGO.transform.SetParent(root.transform, false);
        var nameRect = nameGO.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0);
        nameRect.anchorMax = new Vector2(1, 0.24f);
        nameRect.offsetMin = new Vector2(8, 8);
        nameRect.offsetMax = new Vector2(-8, 0);
        var nameTmp = nameGO.AddComponent<TextMeshProUGUI>();
        nameTmp.text = "Nombre";
        nameTmp.fontSize = 26;
        nameTmp.fontStyle = FontStyles.Bold;
        nameTmp.alignment = TextAlignmentOptions.Center;
        nameTmp.color = Color.white;
        nameTmp.enableAutoSizing = true;
        nameTmp.fontSizeMin = 16;
        nameTmp.fontSizeMax = 30;

        // Asignar referencias al script
        view.frameImage = frameImg;
        view.artworkImage = artImg;
        view.nameText = nameTmp;
        view.rarityText = rarityTmp;

        // Guardar como prefab
        string path = "Assets/_Project/Prefabs/UI/ResultCard.prefab";
        var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        AssetDatabase.Refresh();

        if (prefab != null)
        {
            Debug.Log("[PrefabBuilder] ResultCard creado en: " + path);
            EditorUtility.DisplayDialog("AlRescate",
                "Prefab ResultCard creado en:\nAssets/_Project/Prefabs/UI/ResultCard.prefab\n\n" +
                "Asignalo al campo 'Result Card Prefab' del RevealAnimationController en Canvas_Result.",
                "OK");
        }
    }

    [MenuItem("AlRescate/Crear Prefab CollectionCell")]
    public static void CreateCollectionCell()
    {
        var root = new GameObject("CollectionCell");
        var rootRect = root.AddComponent<RectTransform>();
        rootRect.sizeDelta = new Vector2(180, 220);
        var rootImg = root.AddComponent<Image>();
        rootImg.color = new Color(0.12f, 0.12f, 0.2f, 1f);
        var cell = root.AddComponent<CollectionCell>();
        var btn = root.AddComponent<Button>();
        var cb = btn.colors;
        cb.highlightedColor = new Color(0.2f, 0.2f, 0.35f);
        btn.colors = cb;

        // Frame (borde)
        var frame = new GameObject("Frame");
        frame.transform.SetParent(root.transform, false);
        var frameRect = frame.AddComponent<RectTransform>();
        frameRect.anchorMin = Vector2.zero;
        frameRect.anchorMax = Vector2.one;
        frameRect.offsetMin = new Vector2(-4, -4);
        frameRect.offsetMax = new Vector2(4, 4);
        frame.transform.SetAsFirstSibling();
        var frameImg = frame.AddComponent<Image>();
        frameImg.color = new Color(0.5f, 0.5f, 0.5f, 1f);

        // Artwork
        var art = new GameObject("Artwork");
        art.transform.SetParent(root.transform, false);
        var artRect = art.AddComponent<RectTransform>();
        artRect.anchorMin = new Vector2(0, 0.22f);
        artRect.anchorMax = new Vector2(1, 1f);
        artRect.offsetMin = new Vector2(8, 0);
        artRect.offsetMax = new Vector2(-8, -8);
        var artImg = art.AddComponent<Image>();
        artImg.color = new Color(0.2f, 0.2f, 0.3f, 1f);
        artImg.preserveAspect = true;

        // Silueta overlay (para items no obtenidos)
        var silhouette = new GameObject("SilhouetteOverlay");
        silhouette.transform.SetParent(root.transform, false);
        var silRect = silhouette.AddComponent<RectTransform>();
        silRect.anchorMin = new Vector2(0, 0.22f);
        silRect.anchorMax = new Vector2(1, 1f);
        silRect.offsetMin = new Vector2(8, 0);
        silRect.offsetMax = new Vector2(-8, -8);
        var silImg = silhouette.AddComponent<Image>();
        silImg.color = new Color(0f, 0f, 0f, 0.7f);
        silhouette.SetActive(false);

        // Label
        var labelGO = new GameObject("Label");
        labelGO.transform.SetParent(root.transform, false);
        var labelRect = labelGO.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(1, 0.22f);
        labelRect.offsetMin = new Vector2(4, 4);
        labelRect.offsetMax = new Vector2(-4, 0);
        var labelTmp = labelGO.AddComponent<TextMeshProUGUI>();
        labelTmp.text = "Nombre";
        labelTmp.fontSize = 18;
        labelTmp.alignment = TextAlignmentOptions.Center;
        labelTmp.color = Color.white;
        labelTmp.enableAutoSizing = true;
        labelTmp.fontSizeMin = 12;
        labelTmp.fontSizeMax = 20;

        // Asignar refs
        cell.artwork = artImg;
        cell.frame = frameImg;
        cell.label = labelTmp;
        cell.button = btn;
        cell.silhouetteOverlay = silhouette;

        string path = "Assets/_Project/Prefabs/UI/CollectionCell.prefab";
        var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        AssetDatabase.Refresh();

        if (prefab != null)
        {
            Debug.Log("[PrefabBuilder] CollectionCell creado en: " + path);
            EditorUtility.DisplayDialog("AlRescate",
                "Prefab CollectionCell creado en:\nAssets/_Project/Prefabs/UI/CollectionCell.prefab\n\n" +
                "Asignalo al campo 'Cell Prefab' del CollectionUI en Canvas_Collection.",
                "OK");
        }
    }
}
