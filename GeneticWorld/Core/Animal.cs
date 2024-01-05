using GeneticWorld.Model;
using GeometRi;

namespace GeneticWorld.Core;

public class Animal(IRandomGenerator rng)
{
    public Point3d Position { get; set; } = new(rng.GetRandomNumberInRange(0, 1), rng.GetRandomNumberInRange(0, 1), 0);
    public Rotation Rotation { get; set; } = Rotation.Random();
    public double Speed { get; set; } = 0.002;
}
