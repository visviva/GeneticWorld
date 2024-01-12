using GeneticWorld.Model;
using GeometRi;

namespace GeneticWorld.Core;

public class Food
{
    private readonly IRandomGenerator _rng;
    public Point3d Position { get; set; } = new Point3d(0, 0, 0);

    public Food(IRandomGenerator rng)
    {
        _rng = rng;
        RandomFoodPosition();
    }

    public void RandomFoodPosition()
    {
        Position = new(_rng.GetRandomNumberInRange(0, 1), _rng.GetRandomNumberInRange(0, 1), 0);
    }
}
