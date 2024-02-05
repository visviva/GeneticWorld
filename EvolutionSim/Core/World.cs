using System.Collections.Concurrent;
using GeneticWorld.Model;

namespace GeneticWorld.Core;

public class World
{
    public List<Animal> Animals { get; set; }
    public List<Food> Foods { get; set; }
    public IRandomGenerator Rng { get; }

    public World(IRandomGenerator rng)
    {
        Rng = rng;
        RandomAnimals();
        RandomFood();
    }

    public void RandomFood()
    {
        Foods = Enumerable.Range(0, 40).Select(x => new Food(Rng)).ToList();
    }

    public void RandomAnimals()
    {
        Animals = Enumerable.Range(0, 30).Select(x => new Animal(Rng)).ToList();
    }
}
