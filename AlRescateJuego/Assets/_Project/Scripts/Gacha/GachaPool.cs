using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AlRescate/Gacha Pool", fileName = "Pool_New")]
public class GachaPool : ScriptableObject
{
    [Header("Costos")]
    public int singlePullCost = 160;
    public int multiPullCost = 1440;

    [Header("Probabilidades por rareza")]
    [Range(0, 1)] public float pCommon = 0.783f;
    [Range(0, 1)] public float pRare = 0.150f;
    [Range(0, 1)] public float pEpic = 0.061f;
    [Range(0, 1)] public float pLegendary = 0.006f;

    [Header("Pity")]
    public int softPityStart = 75;
    public int hardPity = 90;
    public float softPityIncrement = 0.06f;
    public int epicPityThreshold = 10;

    [Header("Sub-pool")]
    [Range(0, 1)] public float petChance = 0.30f;

    [Header("Contenido por rareza")]
    public List<PetData> petsCommon = new();
    public List<PetData> petsRare = new();
    public List<PetData> petsEpic = new();
    public List<PetData> petsLegendary = new();
    public List<GachaItemData> itemsCommon = new();
    public List<GachaItemData> itemsRare = new();
    public List<GachaItemData> itemsEpic = new();
    public List<GachaItemData> itemsLegendary = new();

    public List<PetData> GetPetsByRarity(Rarity r) => r switch
    {
        Rarity.Common => petsCommon,
        Rarity.Rare => petsRare,
        Rarity.Epic => petsEpic,
        Rarity.Legendary => petsLegendary,
        _ => petsCommon
    };

    public List<GachaItemData> GetItemsByRarity(Rarity r) => r switch
    {
        Rarity.Common => itemsCommon,
        Rarity.Rare => itemsRare,
        Rarity.Epic => itemsEpic,
        Rarity.Legendary => itemsLegendary,
        _ => itemsCommon
    };
}
