using System;
using System.Collections.Generic;

namespace Evolution;

public class WeightedRandomSelector<T> where T : notnull
{
    private readonly IRandomHelper _randomGenerator;
    private readonly IReadOnlyList<KeyValuePair<T, double>> _weights;

    public WeightedRandomSelector(IRandomHelper randomGenerator, Dictionary<T, double> weights)
    {
        _randomGenerator = randomGenerator ?? throw new ArgumentNullException(nameof(randomGenerator));
        _weights = (weights ?? throw new ArgumentNullException(nameof(weights))).ToList();
    }

    public T SelectRandomItem()
    {
        if (_weights.Count == 0)
        {
            throw new InvalidOperationException("Selection from an empty collection is not possible");
        }

        double totalWeight = 0;
        foreach (var item in _weights)
        {
            if (!double.IsFinite(item.Value) || item.Value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(_weights), "Weights must be finite and non-negative");
            }

            totalWeight += item.Value;
        }

        if (!double.IsFinite(totalWeight))
        {
            throw new ArgumentOutOfRangeException(nameof(_weights), "The total weight must be finite");
        }

        var randomValue = _randomGenerator.GetRandom_0_1();
        if (!double.IsFinite(randomValue) || randomValue < 0 || randomValue > 1)
        {
            throw new InvalidOperationException("The random generator must return a value between zero and one");
        }

        if (totalWeight == 0)
        {
            var index = Math.Min((int)(randomValue * _weights.Count), _weights.Count - 1);
            return _weights[index].Key;
        }

        var target = randomValue * totalWeight;
        double cumulativeWeight = 0;
        foreach (var item in _weights)
        {
            cumulativeWeight += item.Value;
            if (target < cumulativeWeight)
            {
                return item.Key;
            }
        }

        return _weights.Last(item => item.Value > 0).Key;
    }
}


