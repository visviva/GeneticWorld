using Evolution;
using EvolutionSim.Utility;
using GeometRi;

namespace Simulation;

public partial class Simulation
{
    public event EventHandler<EvolutionStatistics> BeforeEvolutionHook;
    public int Cycle { get; set; } = 0;
    public double SpeedMin { get; set; } = Parameters.SpeedMin;
    public double SpeedMax { get; set; } = Parameters.SpeedMax;
    public double SpeedAccel { get; set; } = Parameters.SpeedAccel;
    public double RotationAccel { get; set; } = Parameters.RotationAccel;
    public int GenerationLength { get; set; } = Parameters.GenerationLength;

    public GeneticAlgorithm GeneticAlgorithm { get; set; }
    int Age = 0;

    public World World { get; }
    public IRandomGenerator Rng { get; }

    public Simulation(IRandomGenerator rng)
    {
        World = new(rng);
        GeneticAlgorithm = new((IRandomHelper)rng, new RouletteWheelSelection(), new UniformCrossover((IRandomHelper)rng), new GaussianMutation((IRandomHelper)rng, 0.01, 0.3));
        Rng = rng;
    }

    public enum SimulationResult
    {
        NewGeneration,
        CurrentGeneration
    }

    public SimulationResult step()
    {
        ProcessCollisions();
        ProcessBrain();
        ProcessMovements();

        Age++;

        if (Age > GenerationLength)
        {
            BeforeEvolutionHook?.Invoke(this, GetStatistics());
            Cycle++;
            Evolve();
            return SimulationResult.NewGeneration;
        }

        return SimulationResult.CurrentGeneration;
    }

    private EvolutionStatistics GetStatistics()
    {
        double mean = World.Animals.Average((animal) => animal.Satiation);
        int max = World.Animals.MaxBy((animal) => animal.Satiation)?.Satiation ?? 0;
        int min = World.Animals.MinBy((animal) => animal.Satiation)?.Satiation ?? 0;
        return new(Cycle, mean, max, min);
    }

    public double Percentage => Age / (double)GenerationLength;

    private void Evolve()
    {
        var currentPopulation = World.Animals.Select(animal => (IIndividual)AnimalIndividual.FromAnimal(animal)).ToList();
        var evolvedPopulation = GeneticAlgorithm.Evolve(currentPopulation);
        World.Animals = evolvedPopulation.Select(individual => ((AnimalIndividual)individual).IntoAnimal(Rng)).ToList();
        World.RandomFood();
        Age = 0;
    }

    private void ProcessBrain()
    {
        Span<double> response = stackalloc double[2];
        foreach (var animal in World.Animals)
        {
            var vision = animal.ProcessVisionBuffered(World.Foods);
            animal.Brain.Network.PropagateInto(vision, response);

            var r0 = Math.Clamp(response[0], 0.0, 1.0) - 0.5;
            var r1 = Math.Clamp(response[1], 0.0, 1.0) - 0.5;

            var speed = (r0 + r1) * SpeedAccel;
            var rotation = (r0 - r1) * RotationAccel;

            animal.Speed = Math.Clamp(animal.Speed + speed, SpeedMin, SpeedMax);
            animal.Heading += rotation;
        }
    }

    private void ProcessMovements()
    {
        foreach (var animal in World.Animals)
        {
            MoveAnimal(animal);
        }
    }

    private void ProcessCollisions()
    {
        foreach (var animal in World.Animals)
        {
            foreach (var food in World.Foods)
            {
                var distanceSquared = WorldGeometry.DistanceSquared(animal.Position, food.Position);

                if (distanceSquared <= 0.02 * 0.02)
                {
                    animal.Satiation++;
                    food.RandomFoodPosition();
                }
            }
        }
    }

    private static void MoveAnimal(Animal animal)
    {
        animal.Position = new Point3d(
            WorldGeometry.WrapCoordinate(animal.Position.X - Math.Sin(animal.Heading) * animal.Speed),
            WorldGeometry.WrapCoordinate(animal.Position.Y + Math.Cos(animal.Heading) * animal.Speed),
            animal.Position.Z);
    }
}
