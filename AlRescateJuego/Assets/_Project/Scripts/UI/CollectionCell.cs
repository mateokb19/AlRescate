using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CollectionCell : MonoBehaviour
{
    public Image artwork, frame;
    public TextMeshProUGUI label;
    public Button button;
    public GameObject silhouetteOverlay;

    public void SetPet(PetData p, bool owned, Action onClick)
    {
        if (artwork != null)
        {
            artwork.sprite = p.iconCollection;
            artwork.color = owned ? Color.white : Color.black;
        }
        if (silhouetteOverlay != null) silhouetteOverlay.SetActive(!owned);
        if (label != null) label.text = owned ? p.displayName : "???";
        if (frame != null) frame.color = RarityPalette.For(p.rarity);
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke());
    }

    public void SetItem(GachaItemData i, int qty, Action onClick)
    {
        if (artwork != null)
        {
            artwork.sprite = i.icon;
            artwork.color = qty > 0 ? Color.white : Color.black;
        }
        if (silhouetteOverlay != null) silhouetteOverlay.SetActive(qty <= 0);
        if (label != null) label.text = qty > 0 ? $"{i.displayName} x{qty}" : "???";
        if (frame != null) frame.color = RarityPalette.For(i.rarity);
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke());
    }
}
