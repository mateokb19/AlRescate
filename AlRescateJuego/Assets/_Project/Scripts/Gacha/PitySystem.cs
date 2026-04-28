using UnityEngine;

public class PitySystem
{
    private readonly GachaPool _pool;

    public PitySystem(GachaPool pool) { _pool = pool; }

    public int LegendaryCounter
    {
        get => SaveSystem.Current.pityLegendaryCounter;
        set => SaveSystem.Current.pityLegendaryCounter = value;
    }

    public int EpicCounter
    {
        get => SaveSystem.Current.pityEpicCounter;
        set => SaveSystem.Current.pityEpicCounter = value;
    }

    public float GetEffectiveLegendaryChance()
    {
        int n = LegendaryCounter;
        if ((n + 1) >= _pool.hardPity) return 1f;
        if ((n + 1) > _pool.softPityStart)
        {
            float extra = ((n + 1) - _pool.softPityStart) * _pool.softPityIncrement;
            return Mathf.Min(1f, _pool.pLegendary + extra);
        }
        return _pool.pLegendary;
    }

    public bool ShouldForceEpic() => (EpicCounter + 1) >= _pool.epicPityThreshold;

    public void OnRarityRolled(Rarity r)
    {
        if (r == Rarity.Legendary) { LegendaryCounter = 0; EpicCounter = 0; }
        else if (r == Rarity.Epic) { LegendaryCounter += 1; EpicCounter = 0; }
        else { LegendaryCounter += 1; EpicCounter += 1; }
    }
}
