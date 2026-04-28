using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultCardView : MonoBehaviour
{
    public Image frameImage;
    public Image artworkImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI rarityText;
    public Sprite frameCommon, frameRare, frameEpic, frameLegendary;

    public void Setup(GachaResult r)
    {
        Color rarityColor = RarityPalette.For(r.rarity);

        if (r.isPet && r.pet != null)
        {
            if (artworkImage != null) artworkImage.sprite = r.pet.iconLarge;
            nameText.text = r.pet.displayName + (r.isDuplicate ? $"\n(+{r.gemsAwarded} Gemas)" : "");
        }
        else if (r.item != null)
        {
            if (artworkImage != null) artworkImage.sprite = r.item.icon;
            nameText.text = r.item.displayName;
        }

        nameText.color = Color.white;
        nameText.fontSize = 28;

        rarityText.text = r.rarity.ToString().ToUpper();
        rarityText.color = rarityColor;
        rarityText.fontSize = 22;

        if (frameImage != null)
        {
            frameImage.color = rarityColor;
            if (FrameByRarity(r.rarity) != null)
                frameImage.sprite = FrameByRarity(r.rarity);
        }

        // Fondo de la carta oscuro para que el texto sea visible
        var bg = GetComponent<UnityEngine.UI.Image>();
        if (bg != null) bg.color = new Color(0.1f, 0.1f, 0.18f, 0.95f);
    }

    private Sprite FrameByRarity(Rarity r) => r switch
    {
        Rarity.Common => frameCommon,
        Rarity.Rare => frameRare,
        Rarity.Epic => frameEpic,
        Rarity.Legendary => frameLegendary,
        _ => frameCommon
    };
}
