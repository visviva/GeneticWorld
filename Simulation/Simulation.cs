﻿using Evolution;
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
        foreach (var animal in World.Animals)
        {
            var vision = animal.ProcessVision(World.Foods);
            var response = animal.Brain.Network.Propagate(vision);

            var r0 = Math.Clamp(response[0], 0.0, 1.0) - 0.5;
            var r1 = Math.Clamp(response[1], 0.0, 1.0) - 0.5;

            var speed = Math.Clamp(r0 + r1, -SpeedAccel, SpeedAccel);
            var rotation = Math.Clamp(r0 - r1, -RotationAccel, RotationAccel);

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
