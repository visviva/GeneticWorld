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
    public Network Brain { get; set; }

    public List<double> ProcessVision(IReadOnlyList<Food> foods) => Eye.ProcessVision(Position, Rotation, foods);

    public Animal(IRandomGenerator rng)
    {
        Position = new(rng.GetRandomNumberInRange(0, 1), rng.GetRandomNumberInRange(0, 1), 0);
        Rotation = Rotation.FromEulerAngles(0, 0, rng.GetRandomNumberInRange(0, Math.PI * 2), "xyz");

        Eye = new Eye();

        List<LayerTopology> topology = [new LayerTopology(Eye.Cells), new LayerTopology(2 * Eye.Cells), new LayerTopology(2)];

        Brain = new(rng, topology);
    }
}
