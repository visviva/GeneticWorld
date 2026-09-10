using System.Diagnostics;
using Cortex;
using Evolution;
using Simulation;

// Fixed seeds and fresh worlds make before/after runs comparable.
// This measures the simulation on desktop .NET, excluding browser scheduling/rendering.
RunGeneration(0);
var timings = new List<double>();
var allocations = new List<long>();
for (var seed = 1; seed <= 7; seed++)
{
    var result = RunGeneration(seed);
    timings.Add(result.Milliseconds);
    allocations.Add(result.Bytes);
    Console.WriteLine($"Seed {seed}: {result.Milliseconds:F1} ms, {result.Bytes / 1048576.0:F2} MB allocated");
}
timings.Sort();
allocations.Sort();
Console.WriteLine($"Median: {timings[3]:F1} ms/generation, {allocations[3] / 1048576.0:F2} MB allocated");

static (double Milliseconds, long Bytes) RunGeneration(int seed)
{
    var simulation = new Simulation.Simulation(new SeededRandom(seed));
    var before = GC.GetAllocatedBytesForCurrentThread();
    var timer = Stopwatch.StartNew();
    while (simulation.step() != Simulation.Simulation.SimulationResult.NewGeneration) { }
    timer.Stop();
    return (timer.Elapsed.TotalMilliseconds, GC.GetAllocatedBytesForCurrentThread() - before);
}

sealed class SeededRandom(int seed) : IRandomGenerator, IRandomNumber, IRandomHelper
{
    private readonly Random random = new(seed);
    public double GetRandomNumberInRange(double minimal, double maximal) => random.NextDouble() * (maximal - minimal) + minimal;
    public double GetRand_minus1_1() => GetRandomNumberInRange(-1, 1);
    public double GetRandom_0_1() => random.NextDouble();
    public double GetRandomSign() => GetRandom_0_1() >= 0.5 ? 1 : -1;
}
