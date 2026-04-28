using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionDetailView : MonoBehaviour
{
    public Image bigArt;
    public TextMeshProUGUI titleText, rarityText, descriptionText, powerupText;
    public Button btnEquip;

    public void ShowPet(PetData p, bool owned)
    {
        gameObject.SetActive(true);
        if (bigArt != null) bigArt.sprite = p.iconLarge;
        if (titleText != null) titleText.text = owned ? p.displayName : "???";
        if (rarityText != null)
        {
            rarityText.text = p.rarity.ToString();
            rarityText.color = RarityPalette.For(p.rarity);
        }
        if (descriptionText != null)
            descriptionText.text = owned ? p.description : "Sigue jugando para descubrir esta mascota.";
        if (powerupText != null)
            powerupText.text = owned ? p.powerupDescription : "";
        if (btnEquip != null)
        {
            btnEquip.gameObject.SetActive(owned);
            btnEquip.onClick.RemoveAllListeners();
            btnEquip.onClick.AddListener(() => PlayerInventory.Instance.EquipPet(p));
        }
    }

    public void ShowItem(GachaItemData i, int qty)
    {
        gameObject.SetActive(true);
        if (bigArt != null) bigArt.sprite = i.icon;
        if (titleText != null) titleText.text = qty > 0 ? i.displayName : "???";
        if (rarityText != null)
        {
            rarityText.text = i.rarity.ToString();
            rarityText.color = RarityPalette.For(i.rarity);
        }
        if (descriptionText != null)
            descriptionText.text = qty > 0 ? i.description : "Aun no has obtenido este item.";
        if (powerupText != null) powerupText.text = $"Cantidad: {qty}";
        if (btnEquip != null) btnEquip.gameObject.SetActive(false);
    }
}
