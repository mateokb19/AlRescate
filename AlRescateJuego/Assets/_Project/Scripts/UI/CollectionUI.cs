using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionUI : MonoBehaviour
{
    public GachaDatabase database;
    public GameObject cellPrefab;
    public CollectionDetailView detailView;

    [Header("Secciones")]
    public GameObject sectionPets;   // GameObject padre de la sección mascotas
    public Transform gridPets;       // Grid Layout Group donde van las celdas de mascotas
    public GameObject sectionItems;  // GameObject padre de la sección objetos
    public Transform gridItems;      // Grid Layout Group donde van las celdas de objetos

    [Header("Filtros")]
    public Toggle togglePets, toggleItems;
    public Toggle toggleCommon, toggleRare, toggleEpic, toggleLegendary;

    void OnEnable()
    {
        Rebuild();
    }

    public void Rebuild()
    {
        ClearGrid(gridPets);
        ClearGrid(gridItems);

        var inv = PlayerInventory.Instance;
        bool showPets = togglePets == null || togglePets.isOn || (toggleItems != null && !toggleItems.isOn);
        bool showItems = toggleItems == null || toggleItems.isOn || (togglePets != null && !togglePets.isOn);

        var petList = new List<PetData>();
        if (showPets)
            foreach (var p in database.AllPets())
                if (PassesRarityFilter(p.rarity)) petList.Add(p);

        var itemList = new List<GachaItemData>();
        if (showItems)
            foreach (var i in database.AllItems())
                if (PassesRarityFilter(i.rarity)) itemList.Add(i);

        if (sectionPets != null) sectionPets.SetActive(petList.Count > 0);
        foreach (var p in petList)
            AddCellPet(p, inv.HasPet(p.id));

        if (sectionItems != null) sectionItems.SetActive(itemList.Count > 0);
        foreach (var i in itemList)
            AddCellItem(i, inv.GetItemQty(i.id));

        if (gridPets != null)
        {
            var contentRT = gridPets.parent?.parent as RectTransform;
            if (contentRT != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(contentRT);
        }
    }

    private void ClearGrid(Transform grid)
    {
        if (grid == null) return;
        for (int i = grid.childCount - 1; i >= 0; i--)
        {
            var child = grid.GetChild(i);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
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
        var go = Instantiate(cellPrefab, gridPets);
        go.GetComponent<CollectionCell>().SetPet(p, owned, () =>
        {
            if (detailView != null) detailView.ShowPet(p, owned);
            else if (owned) PlayerInventory.Instance.EquipPet(p);
        });
    }

    private void AddCellItem(GachaItemData i, int qty)
    {
        var go = Instantiate(cellPrefab, gridItems);
        go.GetComponent<CollectionCell>().SetItem(i, qty, () => detailView?.ShowItem(i, qty));
    }
}
