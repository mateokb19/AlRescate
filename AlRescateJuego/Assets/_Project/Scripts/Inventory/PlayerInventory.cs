using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    public event Action<int> OnGemsChanged;
    public event Action<PetData> OnPetObtained;
    public event Action<GachaItemData> OnItemObtained;
    public event Action<PetData> OnPetEquipped;

    [SerializeField] private GachaDatabase database;

    public int Gems
    {
        get => SaveSystem.Current.gems;
        private set
        {
            SaveSystem.Current.gems = Mathf.Max(0, value);
            OnGemsChanged?.Invoke(SaveSystem.Current.gems);
        }
    }

    public PetData EquippedPet { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(SaveSystem.Current.equippedPetId))
        {
            EquippedPet = database.GetPetById(SaveSystem.Current.equippedPetId);
            if (EquippedPet != null) OnPetEquipped?.Invoke(EquippedPet);
        }
        OnGemsChanged?.Invoke(Gems);
    }

    public bool TrySpendGems(int amount)
    {
        if (Gems < amount) { AudioManager.Instance.Play("sfx_ui_error"); return false; }
        Gems -= amount;
        SaveSystem.Save();
        return true;
    }

    public void AddGems(int amount)
    {
        if (amount <= 0) return;
        Gems += amount;
        SaveSystem.Save();
    }

    public bool HasPet(string petId) => SaveSystem.Current.ownedPetIds.Contains(petId);

    public void GrantPet(PetData pet, out int gemsEquivalent)
    {
        gemsEquivalent = 0;
        if (pet == null) return;
        if (HasPet(pet.id))
        {
            gemsEquivalent = pet.duplicateGemValue;
            AddGems(gemsEquivalent);
            return;
        }
        SaveSystem.Current.ownedPetIds.Add(pet.id);
        OnPetObtained?.Invoke(pet);
        SaveSystem.Save();
    }

    public void GrantItem(GachaItemData item, int qty = 1)
    {
        if (item == null) return;
        var stack = SaveSystem.Current.itemStacks.Find(s => s.itemId == item.id);
        if (stack == null)
        {
            stack = new SaveData.ItemStack { itemId = item.id, quantity = 0 };
            SaveSystem.Current.itemStacks.Add(stack);
        }
        stack.quantity += qty;
        OnItemObtained?.Invoke(item);
        SaveSystem.Save();
    }

    public void EquipPet(PetData pet)
    {
        if (pet == null || !HasPet(pet.id)) return;
        EquippedPet = pet;
        SaveSystem.Current.equippedPetId = pet.id;
        OnPetEquipped?.Invoke(pet);
        SaveSystem.Save();
        AudioManager.Instance.Play("sfx_pet_equip");
    }

    public int GetItemQty(string itemId)
    {
        var st = SaveSystem.Current.itemStacks.Find(s => s.itemId == itemId);
        return st?.quantity ?? 0;
    }
}
