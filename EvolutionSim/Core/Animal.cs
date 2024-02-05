using EvolutionSim.Core;
using GeneticWorld.Model;
using GeometRi;

namespace GeneticWorld.Core;

public class Animal
{
    public Point3d Position { get; set; }
    public Rotation Rotation { get; set; }
    public double Speed { get; set; } = 0.005;
    public Eye Eye { get; set; }
    public Brain Brain { get; set; }
    public int Satiation { get; set; } = 0;

    public List<double> ProcessVision(IReadOnlyList<Food> foods) => Eye.ProcessVision(Position, Rotation, foods);
    internal static Animal FromChromosome(IRandomGenerator rng, Chromosome chromosome)
    {
        var eye = new Eye();
        var brain = Brain.FromChromosome(chromosome, eye);
        return new Animal(rng, eye, brain);
    }

    public Chromosome Chromosome => Brain.Chromosome;

    public Animal(IRandomGenerator rng)
    {
        RandomizePosition(rng);
        Eye = new Eye();
        Brain = new(rng, Eye);
    }

    public Animal(IRandomGenerator rng, Eye eye, Brain brain)
    {
        RandomizePosition(rng);
        Eye = eye;
        Brain = brain;
    }

    private void RandomizePosition(IRandomGenerator rng)
    {
        Position = new(rng.GetRandomNumberInRange(0, 1), rng.GetRandomNumberInRange(0, 1), 0);
        Rotation = Rotation.FromEulerAngles(0, 0, rng.GetRandomNumberInRange(0, Math.PI * 2), "xyz");
    }
}
