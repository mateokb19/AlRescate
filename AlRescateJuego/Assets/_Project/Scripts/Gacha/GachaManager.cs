using System;
using System.Collections.Generic;
using UnityEngine;

public class GachaManager : MonoBehaviour
{
    public static GachaManager Instance { get; private set; }

    [SerializeField] private GachaPool pool;
    public GachaPool Pool => pool;

    private PitySystem _pity;
    public event Action<List<GachaResult>> OnPullCompleted;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (pool == null) Debug.LogError("[GachaManager] El campo 'Pool' no está asignado en el Inspector.");
        else _pity = new PitySystem(pool);
    }

    public bool TryPullX1(out GachaResult result)
    {
        result = default;
        if (pool == null) { Debug.LogError("[GachaManager] Pool es null. Asigna MainGachaPool en el Inspector."); return false; }
        if (PlayerInventory.Instance == null) { Debug.LogError("[GachaManager] PlayerInventory.Instance es null."); return false; }
        if (!PlayerInventory.Instance.TrySpendGems(pool.singlePullCost)) return false;
        result = SinglePull();
        PlayerProfile.RegisterPull(1);
        SaveSystem.Save();
        OnPullCompleted?.Invoke(new List<GachaResult> { result });
        return true;
    }

    public bool TryPullX10(out List<GachaResult> results)
    {
        results = new List<GachaResult>();
        if (!PlayerInventory.Instance.TrySpendGems(pool.multiPullCost)) return false;
        bool anyRareOrAbove = false;
        for (int i = 0; i < 10; i++)
        {
            bool isLast = (i == 9);
            GachaResult r = SinglePull(forceAtLeastRare: isLast && !anyRareOrAbove);
            if (r.rarity != Rarity.Common) anyRareOrAbove = true;
            results.Add(r);
        }
        PlayerProfile.RegisterPull(10);
        SaveSystem.Save();
        OnPullCompleted?.Invoke(results);
        return true;
    }

    private GachaResult SinglePull(bool forceAtLeastRare = false)
    {
        Rarity rar;

        if ((_pity.LegendaryCounter + 1) >= pool.hardPity)
        {
            rar = Rarity.Legendary;
        }
        else
        {
            float pLeg = _pity.GetEffectiveLegendaryChance();
            float delta = pLeg - pool.pLegendary;
            float sumaResto = pool.pCommon + pool.pRare + pool.pEpic;
            float factor = (sumaResto - delta) / sumaResto;
            float[] weights = new float[]
            {
                pool.pCommon * factor,
                pool.pRare * factor,
                pool.pEpic * factor,
                pLeg
            };
            int idx = WeightedRandom.PickIndex(weights);
            rar = (Rarity)idx;

            if (_pity.ShouldForceEpic() && (rar == Rarity.Common || rar == Rarity.Rare))
                rar = Rarity.Epic;
            if (forceAtLeastRare && rar == Rarity.Common)
                rar = Rarity.Rare;
        }

        _pity.OnRarityRolled(rar);

        bool isPet = UnityEngine.Random.value < pool.petChance;
        var result = new GachaResult { rarity = rar, isPet = isPet };

        if (isPet)
        {
            var list = pool.GetPetsByRarity(rar);
            if (list == null || list.Count == 0) isPet = false;
            else
            {
                result.pet = list[UnityEngine.Random.Range(0, list.Count)];
                if (PlayerInventory.Instance.HasPet(result.pet.id))
                {
                    result.isDuplicate = true;
                    result.gemsAwarded = result.pet.duplicateGemValue;
                }
                PlayerInventory.Instance.GrantPet(result.pet, out _);
            }
        }
        if (!isPet)
        {
            result.isPet = false;
            var list = pool.GetItemsByRarity(rar);
            if (list != null && list.Count > 0)
            {
                result.item = list[UnityEngine.Random.Range(0, list.Count)];
                PlayerInventory.Instance.GrantItem(result.item, 1);
            }
        }
        return result;
    }
}
