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
        public float priceMXN;
        public string label;
        public string bonusText;
    }

    [Header("Paquetes")]
    public List<GemPackage> packages;

    [Header("UI")]
    public Transform packageGrid;
    public GameObject packageCardPrefab;
    public Button btnClose;

    void Start()
    {
        if (btnClose != null) btnClose.onClick.AddListener(Close);
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
        Debug.Log($"[SHOP] El jugador intento comprar {pkg.gems} Gemas por ${pkg.priceMXN} MXN. " +
                  "Aqui se integraria el sistema de pago real con la pasarela de Cruz Roja.");
        AudioManager.Instance.Play("sfx_shop_click");
    }

    public void Close()
    {
        gameObject.SetActive(false);
        AudioManager.Instance.Play("sfx_ui_close");
    }
}

public class GemPackageCard : MonoBehaviour
{
    public TextMeshProUGUI gemsText, priceText, bonusText;
    public Button buyButton;

    public void Setup(ShopManager.GemPackage pkg, System.Action<ShopManager.GemPackage> onBuy)
    {
        if (gemsText != null) gemsText.text = pkg.gems.ToString("N0") + " Gemas";
        if (priceText != null) priceText.text = "$" + pkg.priceMXN.ToString("N0") + " MXN";
        if (bonusText != null) bonusText.text = pkg.bonusText;
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => onBuy(pkg));
    }
}
