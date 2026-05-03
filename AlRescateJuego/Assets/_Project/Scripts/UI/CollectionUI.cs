using UnityEngine;
using UnityEngine.UI;

public class CollectionUI : MonoBehaviour
{
    public GachaDatabase database;
    public Transform gridContent;
    public GameObject cellPrefab;
    public CollectionDetailView detailView;

    [Header("Filtros")]
    public Toggle togglePets, toggleItems;
    public Toggle toggleCommon, toggleRare, toggleEpic, toggleLegendary;

    void OnEnable()
    {
        Rebuild();
    }

    public void Rebuild()
    {
        for (int i = gridContent.childCount - 1; i >= 0; i--)
            Destroy(gridContent.GetChild(i).gameObject);

        var inv = PlayerInventory.Instance;
        bool showPets = togglePets == null || togglePets.isOn || (toggleItems != null && !toggleItems.isOn);
        bool showItems = toggleItems == null || toggleItems.isOn || (togglePets != null && !togglePets.isOn);

        if (showPets)
            foreach (var p in database.AllPets())
                if (PassesRarityFilter(p.rarity)) AddCellPet(p, inv.HasPet(p.id));

        if (showItems)
            foreach (var i in database.AllItems())
                if (PassesRarityFilter(i.rarity)) AddCellItem(i, inv.GetItemQty(i.id));
    }

    private bool PassesRarityFilter(Rarity r)
    {
        bool any = (toggleCommon != null && toggleCommon.isOn) ||
                   (toggleRare != null && toggleRare.isOn) ||
                   (toggleEpic != null && toggleEpic.isOn) ||
                   (toggleLegendary != null && toggleLegendary.isOn);
        if (!any) return true;
        return (r == Rarity.Common && toggleCommon != null && toggleCommon.isOn) ||
               (r == Rarity.Rare && toggleRare != null && toggleRare.isOn) ||
               (r == Rarity.Epic && toggleEpic != null && toggleEpic.isOn) ||
               (r == Rarity.Legendary && toggleLegendary != null && toggleLegendary.isOn);
    }

    private void AddCellPet(PetData p, bool owned)
    {
        var go = Instantiate(cellPrefab, gridContent);
        go.GetComponent<CollectionCell>().SetPet(p, owned, () =>
        {
            if (detailView != null) detailView.ShowPet(p, owned);
            else if (owned) PlayerInventory.Instance.EquipPet(p);
        });
    }

    private void AddCellItem(GachaItemData i, int qty)
    {
        var go = Instantiate(cellPrefab, gridContent);
        go.GetComponent<CollectionCell>().SetItem(i, qty, () => detailView?.ShowItem(i, qty));
    }

}
