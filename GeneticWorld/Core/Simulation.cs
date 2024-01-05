using GeneticWorld.Model;

namespace GeneticWorld.Core;

public class Simulation(IRandomGenerator rng)
{
    public World World { get; } = new(rng);
}
