﻿using GeneticWorld.Model;
using GeometRi;

namespace GeneticWorld.Core;

public class Simulation(IRandomGenerator rng)
{
    public World World { get; } = new(rng);

    public void step()
    {
        ProcessCollisions();
        ProcessMovements();
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
