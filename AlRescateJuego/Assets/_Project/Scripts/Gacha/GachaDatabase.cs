using UnityEngine;
using System.Collections.Generic;

public class GachaDatabase : MonoBehaviour
{
    [SerializeField] private GachaPool pool;

    private Dictionary<string, PetData> _petsById;
    private Dictionary<string, GachaItemData> _itemsById;

    void Awake()
    {
        _petsById = new Dictionary<string, PetData>();
        foreach (var list in new[] { pool.petsCommon, pool.petsRare, pool.petsEpic, pool.petsLegendary })
            foreach (var p in list)
                if (p != null && !string.IsNullOrEmpty(p.id)) _petsById[p.id] = p;

        _itemsById = new Dictionary<string, GachaItemData>();
        foreach (var list in new[] { pool.itemsCommon, pool.itemsRare, pool.itemsEpic, pool.itemsLegendary })
            foreach (var i in list)
                if (i != null && !string.IsNullOrEmpty(i.id)) _itemsById[i.id] = i;
    }

    public PetData GetPetById(string id) => _petsById.TryGetValue(id, out var p) ? p : null;
    public GachaItemData GetItemById(string id) => _itemsById.TryGetValue(id, out var i) ? i : null;
    public IEnumerable<PetData> AllPets() => _petsById.Values;
    public IEnumerable<GachaItemData> AllItems() => _itemsById.Values;
}
