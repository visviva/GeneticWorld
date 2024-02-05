using GeneticWorld.Model;
using GeometRi;

namespace GeneticWorld.Core;

public partial class Simulation
{

    public double SpeedMin { get; set; } = 0.001;
    public double SpeedMax { get; set; } = 0.005;
    public double SpeedAccel { get; set; } = 0.2;
    public double RotationAccel { get; set; } = Math.PI / 2.0;
    public int GenerationLength { get; set; } = 2500;

    public GeneticAlgorithm GeneticAlgorithm { get; set; }
    int Age = 0;

    public World World { get; }
    public IRandomGenerator Rng { get; }

    public Simulation(IRandomGenerator rng)
    {
        World = new(rng);
        GeneticAlgorithm = new(rng, new RouletteWheelSelection(), new UniformCrossover(rng), new GaussianMutation(rng, 0.01, 0.3));
        Rng = rng;
    }

    public void step()
    {
        ProcessCollisions();
        ProcessBrain();
        ProcessMovements();

        Age++;

        if (Age > GenerationLength)
        {
            Evolve();
        }
    }

    private void Evolve()
    {
        var currentPopulation = World.Animals.Select(animal => AnimalIndividual.FromAnimal(animal)).ToList();
        var evolvedPopulation = GeneticAlgorithm.Evolve(currentPopulation);
        World.Animals = evolvedPopulation.Select(individual => individual.IntoAnimal(Rng)).ToList();
        World.RandomFood();
        Age = 0;
    }

    private void ProcessBrain()
    {
        foreach (var animal in World.Animals)
        {
            var vision = animal.ProcessVision(World.Foods);
            var response = animal.Brain.Network.Propagate(vision);

            var speed = Math.Clamp(response[0], -SpeedAccel, SpeedAccel);
            var rotation = Math.Clamp(response[1], -RotationAccel, RotationAccel);

            animal.Speed = Math.Clamp(animal.Speed + speed, SpeedMin, SpeedMax);
            animal.Rotation = Rotation.FromEulerAngles(0, 0, animal.Rotation.ToAngle + rotation, "xyz");
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
                var distance = animal.Position.DistanceTo(food.Position);

                if (distance <= 0.02)
                {
                    animal.Satiation++;
                    food.RandomFoodPosition();
                }
            }
        }
    }

    private static void MoveAnimal(Animal animal)
    {
        Vector3d movement = new Vector3d(0, -animal.Speed, 0);
        Vector3d rotatedMovement = animal.Rotation * movement;
        animal.Position = animal.Position.Translate(rotatedMovement);
        WrapPosition(animal);
    }

    private static void WrapPosition(Animal animal)
    {
        if (animal.Position.X < 0)
        {
            animal.Position.X = 1;
        }

        if (animal.Position.X > 1)
        {
            animal.Position.X = 0;
        }

        if (animal.Position.Y < 0)
        {
            animal.Position.Y = 1;
        }

        if (animal.Position.Y > 1)
        {
            animal.Position.Y = 0;
        }
    }
}
