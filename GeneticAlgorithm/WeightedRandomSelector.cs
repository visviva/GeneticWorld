using System;
using System.Collections.Generic;

namespace Evolution;

public class WeightedRandomSelector<T>(IRandomHelper rngGen, Dictionary<T, double> weights) where T : notnull
{
    private readonly Dictionary<T, double> Weights = weights ?? throw new ArgumentNullException(nameof(weights));

    public T SelectRandomItem()
    {
        double totalWeight = 0;
        foreach (var weight in Weights.Values)
        {
            totalWeight += weight;
        }

        double randomNumber = rngGen.GetRandom_0_1() * totalWeight;

        foreach (var item in Weights)
        {
            randomNumber -= item.Value;
            if (randomNumber <= 0)
            {
                return item.Key;
            }
        }

        throw new InvalidOperationException("Negative weights is not possible");
    }
}


