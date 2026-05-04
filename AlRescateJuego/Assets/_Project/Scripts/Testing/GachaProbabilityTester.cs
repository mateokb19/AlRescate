using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GachaProbabilityTester : MonoBehaviour
{
    [Header("Config")]
    public GachaPool pool;
    public int iterations = 10000;

    // -------------------------------------------------------------------------
    // HELPERS DE DEBUG - solo funcionan en Play Mode en la escena GachaHub
    // (donde PlayerInventory y GachaManager estan activos).
    // -------------------------------------------------------------------------

    [ContextMenu("DEBUG: Dar 99999 gemas")]
    public void DebugAddGems()
    {
        if (PlayerInventory.Instance == null) { Debug.LogError("[Tester] PlayerInventory no esta en la escena. Ejecuta esto desde GachaHub."); return; }
        PlayerInventory.Instance.AddGems(99999);
        Debug.Log("[Tester] Se agregaron 99999 gemas. Total: " + PlayerInventory.Instance.Gems);
    }

    [ContextMenu("DEBUG: Hacer 89 tiradas y loggear pity")]
    public void DebugDo89Pulls()
    {
        if (GachaManager.Instance == null) { Debug.LogError("[Tester] GachaManager no esta en la escena. Ejecuta esto desde GachaHub."); return; }
        if (PlayerInventory.Instance == null) { Debug.LogError("[Tester] PlayerInventory no esta en la escena."); return; }

        // Asegura suficientes gemas para 89 tiradas x1 (89 * 160 = 14240)
        PlayerInventory.Instance.AddGems(89 * GachaManager.Instance.Pool.singlePullCost + 1000);

        var sb = new StringBuilder();
        sb.AppendLine("[Tester] Realizando 89 tiradas de pity test...");

        for (int i = 1; i <= 89; i++)
        {
            if (GachaManager.Instance.TryPullX1(out GachaResult r))
            {
                int pityLeg = SaveSystem.Current.pityLegendaryCounter;
                if (r.rarity != Rarity.Common)
                    sb.AppendLine($"  Tirada {i,2}: {r.rarity} | pityLeg={pityLeg}");
            }
        }

        int finalPity = SaveSystem.Current.pityLegendaryCounter;
        sb.AppendLine($"[Tester] Listo. pityLegendaryCounter actual = {finalPity}");
        sb.AppendLine($"[Tester] La proxima tirada x1 deberia ser la #{finalPity + 1}. Si es 90 -> debe ser Legendario.");
        Debug.Log(sb.ToString());
    }

    [ContextMenu("DEBUG: Verificar tirada 90 (hard pity)")]
    public void DebugVerifyHardPity()
    {
        if (GachaManager.Instance == null) { Debug.LogError("[Tester] GachaManager no esta en escena. Usa GachaHub."); return; }

        int currentPity = SaveSystem.Current.pityLegendaryCounter;
        Debug.Log($"[Tester] pityLegendaryCounter actual = {currentPity}. " +
                  $"Hard pity se activa en la tirada {GachaManager.Instance.Pool.hardPity}.");

        if (currentPity < GachaManager.Instance.Pool.hardPity - 1)
        {
            Debug.LogWarning($"[Tester] Aun faltan {GachaManager.Instance.Pool.hardPity - 1 - currentPity} tiradas para llegar al hard pity. " +
                             "Ejecuta primero 'DEBUG: Hacer 89 tiradas y loggear pity'.");
            return;
        }

        PlayerInventory.Instance.AddGems(GachaManager.Instance.Pool.singlePullCost + 10);
        if (GachaManager.Instance.TryPullX1(out GachaResult result))
        {
            bool ok = result.rarity == Rarity.Legendary;
            Debug.Log($"[Tester] Tirada de hard pity -> {result.rarity}. " +
                      (ok ? "CORRECTO: es Legendario." : "ERROR: deberia ser Legendario."));
        }
    }

    [ContextMenu("DEBUG: Mostrar pity actual")]
    public void DebugShowCurrentPity()
    {
        Debug.Log($"[Tester] pityLegendaryCounter = {SaveSystem.Current.pityLegendaryCounter} | " +
                  $"pityEpicCounter = {SaveSystem.Current.pityEpicCounter} | " +
                  $"Gemas = {SaveSystem.Current.gems}");
    }

    [ContextMenu("DEBUG: Resetear pity a cero")]
    public void DebugResetPity()
    {
        SaveSystem.Current.pityLegendaryCounter = 0;
        SaveSystem.Current.pityEpicCounter = 0;
        SaveSystem.Save();
        Debug.Log("[Tester] Pity reseteado a 0/0 y guardado.");
    }

    // -------------------------------------------------------------------------
    // Simula tiradas directamente sin tocar PlayerInventory ni la escena.
    // Replica la logica de GachaManager.SinglePull de forma aislada.
    // -------------------------------------------------------------------------

    [ContextMenu("Test 1a: Tasas BASE sin pity (WeightedRandom puro)")]
    public void RunBaseRarityTest()
    {
        if (pool == null) { Debug.LogError("[Tester] Asigna el campo Pool en el Inspector."); return; }

        var counts = new Dictionary<Rarity, int>
        {
            { Rarity.Common, 0 }, { Rarity.Rare, 0 },
            { Rarity.Epic, 0 }, { Rarity.Legendary, 0 }
        };

        // Cada tirada es independiente: pity reseteado a 0 cada vez.
        // Esto verifica que los pesos en GachaPool esten bien configurados.
        float[] weights = new float[] { pool.pCommon, pool.pRare, pool.pEpic, pool.pLegendary };
        for (int i = 0; i < iterations; i++)
        {
            int idx = WeightedRandom.PickIndex(weights);
            counts[(Rarity)idx]++;
        }

        bool ok = CheckMargin(counts[Rarity.Common],   iterations, pool.pCommon,    "Comun")
                & CheckMargin(counts[Rarity.Rare],     iterations, pool.pRare,      "Raro")
                & CheckMargin(counts[Rarity.Epic],     iterations, pool.pEpic,      "Epico")
                & CheckMargin(counts[Rarity.Legendary],iterations, pool.pLegendary, "Legendario");

        var sb = new StringBuilder();
        sb.AppendLine($"[Tester] Tasas BASE (sin pity) en {iterations} tiradas — {(ok ? "TODO OK" : "HAY DESVIACION")}:");
        sb.AppendLine($"  Comun     : {counts[Rarity.Common],6}  ({100f * counts[Rarity.Common] / iterations:F2}%)  teorico {pool.pCommon * 100:F2}%");
        sb.AppendLine($"  Raro      : {counts[Rarity.Rare],6}  ({100f * counts[Rarity.Rare] / iterations:F2}%)  teorico {pool.pRare * 100:F2}%");
        sb.AppendLine($"  Epico     : {counts[Rarity.Epic],6}  ({100f * counts[Rarity.Epic] / iterations:F2}%)  teorico {pool.pEpic * 100:F2}%");
        sb.AppendLine($"  Legendario: {counts[Rarity.Legendary],6}  ({100f * counts[Rarity.Legendary] / iterations:F2}%)  teorico {pool.pLegendary * 100:F2}%");
        sb.AppendLine($"  Margen tolerado: ±1%. Los valores deben estar dentro del rango.");
        if (ok) Debug.Log(sb.ToString()); else Debug.LogWarning(sb.ToString());
    }

    [ContextMenu("Test 1b: Tasas EFECTIVAS con pity acumulado")]
    public void RunEffectiveRarityTest()
    {
        if (pool == null) { Debug.LogError("[Tester] Asigna el campo Pool en el Inspector."); return; }

        var counts = new Dictionary<Rarity, int>
        {
            { Rarity.Common, 0 }, { Rarity.Rare, 0 },
            { Rarity.Epic, 0 }, { Rarity.Legendary, 0 }
        };

        int legCounter = 0, epicCounter = 0;

        for (int i = 0; i < iterations; i++)
        {
            Rarity r = SimulatePull(ref legCounter, ref epicCounter);
            counts[r]++;
        }

        // Con pity activo las tasas efectivas son MAYORES para Epico y Legendario.
        // Epic pity (cada 10): suma ~+5% a Epico. Soft/hard pity: suma ~+0.8-1.2% a Legendario.
        // Rangos esperados con pity en 10000 tiradas:
        //   Comun: ~70-76%  Raro: ~13-16%  Epico: ~10-14%  Legendario: ~1.2-2.0%
        var sb = new StringBuilder();
        sb.AppendLine($"[Tester] Tasas EFECTIVAS (pity acumulado) en {iterations} tiradas:");
        sb.AppendLine($"  Comun     : {counts[Rarity.Common],6}  ({100f * counts[Rarity.Common] / iterations:F2}%)  rango esperado ~70-76%");
        sb.AppendLine($"  Raro      : {counts[Rarity.Rare],6}  ({100f * counts[Rarity.Rare] / iterations:F2}%)  rango esperado ~13-16%");
        sb.AppendLine($"  Epico     : {counts[Rarity.Epic],6}  ({100f * counts[Rarity.Epic] / iterations:F2}%)  rango esperado ~10-14%  (pity +~5%)");
        sb.AppendLine($"  Legendario: {counts[Rarity.Legendary],6}  ({100f * counts[Rarity.Legendary] / iterations:F2}%)  rango esperado ~1.2-2.0%  (pity +~1%)");
        sb.AppendLine($"  NOTA: valores altos en Epico/Legendario son CORRECTOS — el pity funciona.");
        Debug.Log(sb.ToString());
    }

    [ContextMenu("Test 2: Hard pity en tirada 90")]
    public void RunHardPityTest()
    {
        if (pool == null) { Debug.LogError("[Tester] Asigna el campo Pool."); return; }

        const int runs = 100;
        int allLegendary = 0;

        for (int run = 0; run < runs; run++)
        {
            int legCounter = pool.hardPity - 1;
            int epicCounter = 0;
            Rarity r = SimulatePull(ref legCounter, ref epicCounter);
            if (r == Rarity.Legendary) allLegendary++;
        }

        Debug.Log($"[Tester] Hard pity (pityLegendaryCounter=89): {allLegendary}/{runs} tiradas fueron Legendario. " +
                  $"Esperado: 100/100.");
    }

    [ContextMenu("Test 3: Soft pity activo (pity=70)")]
    public void RunSoftPityTest()
    {
        if (pool == null) { Debug.LogError("[Tester] Asigna el campo Pool."); return; }

        const int runs = 1000;
        int firstLegTotal = 0;

        for (int run = 0; run < runs; run++)
        {
            int legCounter = 70;
            int epicCounter = 0;
            int extraPulls = 0;

            while (true)
            {
                extraPulls++;
                Rarity r = SimulatePull(ref legCounter, ref epicCounter);
                if (r == Rarity.Legendary) break;
                if (extraPulls > 30) break; // salvaguarda (max 20 extras hasta hard pity)
            }
            firstLegTotal += extraPulls;
        }

        float avg = (float)firstLegTotal / runs;
        Debug.Log($"[Tester] Soft pity (inicio en 70): promedio de {avg:F1} tiradas extra hasta Legendario. " +
                  $"Esperado entre 1 y 20 extras (soft pity arranca en 75, hard pity en 90).");
    }

    [ContextMenu("Test 4: Pity de Epico cada 10 tiradas")]
    public void RunEpicPityTest()
    {
        if (pool == null) { Debug.LogError("[Tester] Asigna el campo Pool."); return; }

        const int runs = 200;
        int epicForced = 0;

        for (int run = 0; run < runs; run++)
        {
            int legCounter = 0;
            int epicCounter = pool.epicPityThreshold - 1; // tirada 10
            Rarity r = SimulatePull(ref legCounter, ref epicCounter);
            if (r == Rarity.Epic || r == Rarity.Legendary) epicForced++;
        }

        Debug.Log($"[Tester] Pity de Epico (epicCounter=9): {epicForced}/{runs} tiradas fueron Epico+. " +
                  $"Esperado: 200/200 (garantia absoluta).");
    }

    [ContextMenu("Test 5: Garantia Raro+ en x10")]
    public void RunX10GuaranteeTest()
    {
        if (pool == null) { Debug.LogError("[Tester] Asigna el campo Pool."); return; }

        const int batches = 500;
        int batchesWithRarePlus = 0;

        for (int b = 0; b < batches; b++)
        {
            int legCounter = 0, epicCounter = 0;
            bool anyRarePlus = false;

            for (int i = 0; i < 10; i++)
            {
                bool isLast = (i == 9);
                bool forceRare = isLast && !anyRarePlus;
                Rarity r = SimulatePull(ref legCounter, ref epicCounter, forceRare);
                if (r != Rarity.Common) anyRarePlus = true;
            }

            if (anyRarePlus) batchesWithRarePlus++;
        }

        Debug.Log($"[Tester] Garantia Raro+ en x10: {batchesWithRarePlus}/{batches} lotes tuvieron al menos 1 Raro+. " +
                  $"Esperado: {batches}/{batches}.");
    }

    // Replica GachaManager.SinglePull sin depender de PlayerInventory ni SaveSystem.
    private Rarity SimulatePull(ref int legCounter, ref int epicCounter, bool forceAtLeastRare = false)
    {
        Rarity rar;

        if ((legCounter + 1) >= pool.hardPity)
        {
            rar = Rarity.Legendary;
        }
        else
        {
            float pLeg = pool.pLegendary;
            if ((legCounter + 1) > pool.softPityStart)
            {
                float extra = ((legCounter + 1) - pool.softPityStart) * pool.softPityIncrement;
                pLeg = Mathf.Min(1f, pool.pLegendary + extra);
            }

            float delta = pLeg - pool.pLegendary;
            float sumaResto = pool.pCommon + pool.pRare + pool.pEpic;
            float factor = sumaResto > 0f ? (sumaResto - delta) / sumaResto : 1f;

            float[] weights = new float[]
            {
                pool.pCommon * factor,
                pool.pRare * factor,
                pool.pEpic * factor,
                pLeg
            };

            int idx = WeightedRandom.PickIndex(weights);
            rar = (Rarity)idx;

            if ((epicCounter + 1) >= pool.epicPityThreshold && (rar == Rarity.Common || rar == Rarity.Rare))
                rar = Rarity.Epic;

            if (forceAtLeastRare && rar == Rarity.Common)
                rar = Rarity.Rare;
        }

        // Actualizar contadores locales
        if (rar == Rarity.Legendary) { legCounter = 0; epicCounter = 0; }
        else if (rar == Rarity.Epic) { legCounter++; epicCounter = 0; }
        else { legCounter++; epicCounter++; }

        return rar;
    }

    // Devuelve true si el resultado esta dentro del margen de ±1% de la tasa teorica.
    private bool CheckMargin(int count, int total, float expectedRate, string label)
    {
        float actual = (float)count / total;
        float diff = Mathf.Abs(actual - expectedRate);
        if (diff > 0.01f)
        {
            Debug.LogWarning($"[Tester] DESVIACION en {label}: obtenido {actual * 100:F2}%, " +
                             $"esperado {expectedRate * 100:F2}%, diferencia {diff * 100:F2}% (limite 1%)");
            return false;
        }
        return true;
    }
}
