using GeneticWorld.Model;

namespace GeneticWorld.Core;

public class RandomGen : IRandomGenerator
{
    private readonly Random _random = new();
    public double GetRandomNumberInRange(double minNumber, double maxNumber)
    {
        return _random.NextDouble() * (maxNumber - minNumber) + minNumber;
    }
}
