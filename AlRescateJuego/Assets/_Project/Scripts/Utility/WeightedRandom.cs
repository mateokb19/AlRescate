using System.Collections.Generic;
using UnityEngine;

public static class WeightedRandom
{
    public static int PickIndex(IList<float> weights)
    {
        float total = 0f;
        for (int i = 0; i < weights.Count; i++) total += weights[i];
        float r = Random.value * total;
        float cumulative = 0f;
        for (int i = 0; i < weights.Count; i++)
        {
            cumulative += weights[i];
            if (r <= cumulative) return i;
        }
        return weights.Count - 1;
    }

    public static T Pick<T>(IList<T> options, IList<float> weights)
    {
        int idx = PickIndex(weights);
        return options[idx];
    }
}
