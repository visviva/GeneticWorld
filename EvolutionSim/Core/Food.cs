using GeneticWorld.Model;
using GeometRi;

namespace GeneticWorld.Core;

public class Food(IRandomGenerator rng)
{
    public Point3d Position { get; set; } = new(rng.GetRandomNumberInRange(0, 1), rng.GetRandomNumberInRange(0, 1), 0);
}
