using GeneticWorld.Model;
using GeometRi;

namespace GeneticWorld.Core;

public class Animal(IRandomGenerator rng)
{
    public Point3d Position { get; set; } = new(rng.GetRandomNumberInRange(0, 1), rng.GetRandomNumberInRange(0, 1), 0);
    public Rotation Rotation { get; set; } = Rotation.FromEulerAngles(0, 0, rng.GetRandomNumberInRange(0, Math.PI * 2), "xyz");
    public double Speed { get; set; } = 0.01;
}
