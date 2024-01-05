using GeneticWorld.Model;

namespace GeneticWorld.Core;

public class World(IRandomGenerator rng)
{
    public List<Animal> Animals { get; } = Enumerable.Range(0, 40).Select(x => new Animal(rng)).ToList();
    public List<Food> Foods { get; } = Enumerable.Range(0, 40).Select(x => new Food(rng)).ToList();
}
