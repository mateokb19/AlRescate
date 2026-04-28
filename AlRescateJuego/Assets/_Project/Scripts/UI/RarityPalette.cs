using UnityEngine;

public static class RarityPalette
{
    public static Color For(Rarity r) => r switch
    {
        Rarity.Common => new Color(0.61f, 0.64f, 0.69f),
        Rarity.Rare => new Color(0.06f, 0.73f, 0.51f),
        Rarity.Epic => new Color(0.55f, 0.36f, 0.96f),
        Rarity.Legendary => new Color(0.96f, 0.62f, 0.04f),
        _ => Color.white
    };
}
