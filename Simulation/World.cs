namespace Simulation;

public class World
{
    public List<Animal> Animals { get; set; } = [];
    public List<Food> Foods { get; set; } = [];
    public IRandomGenerator Rng { get; }

    public World(IRandomGenerator rng)
    {
        Rng = rng;
        RandomAnimals();
        RandomFood();
    }

    public void RandomFood()
    {
        Foods = Enumerable.Range(0, Parameters.NumberOfFoods).Select(x => new Food(Rng)).ToList();
    }

    public void RandomAnimals()
    {
        Animals = Enumerable.Range(0, Parameters.NumberOfBirds).Select(x => new Animal(Rng)).ToList();
    }
}
