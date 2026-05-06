using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultCardView : MonoBehaviour
{
    public Image frameImage;
    public Image frameGlow;
    public Image shimmer;
    public Image artworkImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI rarityText;
    public Sprite frameCommon, frameRare, frameEpic, frameLegendary;

    public void Setup(GachaResult r)
    {
        Color rarityColor = RarityPalette.For(r.rarity);

        if (r.isPet && r.pet != null)
        {
            if (artworkImage != null)
            {
                artworkImage.sprite = r.pet.iconLarge;
                artworkImage.color = Color.white;
            }
            nameText.text = r.pet.displayName + (r.isDuplicate ? $"\n(+{r.gemsAwarded} Gemas)" : "");
        }
        else if (r.item != null)
        {
            if (artworkImage != null)
            {
                artworkImage.sprite = r.item.icon;
                artworkImage.color = Color.white;
            }
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

        if (frameGlow != null)
        {
            frameGlow.color = new Color(rarityColor.r, rarityColor.g, rarityColor.b, 0.45f);
            frameGlow.gameObject.SetActive(r.rarity >= Rarity.Rare);
        }

        var bg = GetComponent<Image>();
        if (bg != null) bg.color = new Color(0.1f, 0.1f, 0.18f, 0.95f);

        if (shimmer != null && r.rarity >= Rarity.Rare)
        {
            shimmer.gameObject.SetActive(true);
            StartCoroutine(PlayShimmer());
        }
        else if (shimmer != null)
        {
            shimmer.gameObject.SetActive(false);
        }
    }

    private IEnumerator PlayShimmer()
    {
        if (shimmer == null) yield break;
        var rt = shimmer.rectTransform;
        var cardRt = GetComponent<RectTransform>();
        float halfWidth = cardRt.rect.width * 0.5f;
        float duration = 0.7f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float x = Mathf.Lerp(-halfWidth - 60f, halfWidth + 60f, t / duration);
            rt.anchoredPosition = new Vector2(x, 0f);
            yield return null;
        }
        shimmer.gameObject.SetActive(false);
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
