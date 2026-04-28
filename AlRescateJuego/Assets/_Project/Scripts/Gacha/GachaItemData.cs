using UnityEngine;

[CreateAssetMenu(menuName = "AlRescate/Gacha Item Data", fileName = "Item_New")]
public class GachaItemData : ScriptableObject
{
    [Header("Identificacion")]
    public string id;
    public string displayName;
    [TextArea(2, 4)] public string description;

    [Header("Rareza")]
    public Rarity rarity;

    [Header("Visuales")]
    public Sprite icon;

    [Header("Efecto")]
    public ItemEffectType effectType;
    public float duration;
    public float magnitude;

    [Header("Comportamiento al duplicar")]
    public int stackable = 1;
}
