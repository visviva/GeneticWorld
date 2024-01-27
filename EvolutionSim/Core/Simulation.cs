using GeneticWorld.Model;
using GeometRi;

namespace GeneticWorld.Core;

public class Simulation(IRandomGenerator rng)
{
    double SpeedMin { get; set; } = 0.001;
    double SpeedMax { get; set; } = 0.005;
    double SpeedAccel { get; set; } = 0.2;
    double RotationAccel { get; set; } = Math.PI / 2.0;

    public World World { get; } = new(rng);

    public void step()
    {
        ProcessCollisions();
        ProcessBrain();
        ProcessMovements();
    }

    private void ProcessBrain()
    {
        foreach (var animal in World.Animals)
        {
            var vision = animal.ProcessVision(World.Foods);
            var response = animal.Brain.Propagate(vision);

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
