using UnityEngine;

[CreateAssetMenu(menuName = "AlRescate/Pet Data", fileName = "Pet_New")]
public class PetData : ScriptableObject
{
    [Header("Identificacion")]
    public string id;
    public string displayName;
    [TextArea(2, 5)] public string description;

    [Header("Rareza")]
    public Rarity rarity;

    [Header("Visuales")]
    public Sprite iconCollection;
    public Sprite iconLarge;
    public GameObject petPrefab;
    public RuntimeAnimatorController animatorController;

    [Header("Powerup pasivo")]
    public PowerupType powerupType;
    public float powerupValue;
    [TextArea(1, 3)] public string powerupDescription;

    [Header("Recompensas")]
    public int duplicateGemValue = 100;
}
