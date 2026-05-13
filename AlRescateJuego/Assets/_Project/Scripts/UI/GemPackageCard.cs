using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GemPackageCard : MonoBehaviour
{
    public TextMeshProUGUI gemsText, priceText, bonusText;
    public Button buyButton;

    public void Setup(ShopManager.GemPackage pkg, System.Action<ShopManager.GemPackage> onBuy)
    {
        if (gemsText != null) gemsText.text = pkg.gems.ToString("N0") + " Gemas";
        if (priceText != null) priceText.text = "$" + pkg.priceCOP.ToString("N0") + " COP";
        if (bonusText != null) bonusText.text = pkg.bonusText;
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => onBuy(pkg));
    }
}
