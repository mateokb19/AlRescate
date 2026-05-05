using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int gems = 0;
    public int totalPulls = 0;
    public int pityLegendaryCounter = 0;
    public int pityEpicCounter = 0;
    public string equippedPetId = "";
    public bool firstLaunch = true;
    public List<string> ownedPetIds = new List<string>();
    public List<ItemStack> itemStacks = new List<ItemStack>();

    // Volúmenes (0..1, linear)
    public float volumeMaster = 1f;
    public float volumeMusic  = 1f;
    public float volumeSfx    = 1f;

    [Serializable]
    public class ItemStack
    {
        public string itemId;
        public int quantity;
    }
}
