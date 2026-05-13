using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    public class GemPackage
    {
        public string id;
        public int gems;
        public float priceCOP;
        public string label;
        public string bonusText;
    }

    [Header("Paquetes")]
    public List<GemPackage> packages;

    [Header("UI")]
    public Transform packageGrid;
    public GameObject packageCardPrefab;
    public Button btnClose;
    [Header("Feedback (opcional)")]
    public TextMeshProUGUI feedbackText;

    void Start()
    {
        if (btnClose != null) btnClose.onClick.AddListener(Close);
        if (feedbackText != null) feedbackText.gameObject.SetActive(false);
        BuildList();
    }

    void BuildList()
    {
        for (int i = packageGrid.childCount - 1; i >= 0; i--)
            Destroy(packageGrid.GetChild(i).gameObject);
        foreach (var pkg in packages)
        {
            var go = Instantiate(packageCardPrefab, packageGrid);
            go.GetComponent<GemPackageCard>().Setup(pkg, OnBuy);
        }
    }

    public void OnBuy(GemPackage pkg)
    {
        AudioManager.Instance.Play("sfx_shop_click");
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        PlayerInventory.Instance.AddGems(pkg.gems);
        ShowFeedback($"+{pkg.gems:N0} Gemas");
        Debug.Log($"[SHOP] MODO PRUEBA: +{pkg.gems} Gemas otorgadas.");
#else
        Debug.Log($"[SHOP] Compra de {pkg.gems} Gemas por ${pkg.priceCOP} COP — pasarela de pago pendiente.");
#endif
    }

    void ShowFeedback(string msg)
    {
        if (feedbackText == null) return;
        feedbackText.text = msg;
        feedbackText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideFeedback));
        Invoke(nameof(HideFeedback), 2f);
    }

    void HideFeedback()
    {
        if (feedbackText != null) feedbackText.gameObject.SetActive(false);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        AudioManager.Instance.Play("sfx_ui_close");
    }
}

