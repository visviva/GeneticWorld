using Cortex;
using Evolution;

namespace Simulation;

public class RandomGen : IRandomGenerator, IRandomNumber, IRandomHelper
{
    private readonly Random _random = new();

    public double GetRand_minus1_1() => GetRandomNumberInRange(-1.0, 1.0);
    public double GetRandom_0_1() => GetRandomNumberInRange(0, 1.0);
    public double GetRandomNumberInRange(double minimal, double maximal) => _random.NextDouble() * (maximal - minimal) + minimal;
    public double GetRandomSign() => GetRandom_0_1() >= 0.5 ? 1.0 : -1.0;
}
