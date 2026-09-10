using Evolution;
using GeometRi;

namespace Simulation;

public class Animal
{
    private double[] _vision = [];
    private double _heading;
    private Rotation? _rotation;

    public Point3d Position { get; set; } = new Point3d(0, 0, 0);
    public Rotation Rotation
    {
        get => _rotation ??= Rotation.FromEulerAngles(0, 0, _heading, "xyz");
        set
        {
            _rotation = value;
            _heading = value.ToEulerAngles("xyz")[2];
        }
    }

    // The world is planar. Materialize a 3D rotation only for callers that need one.
    public double Heading
    {
        get => _heading;
        internal set
        {
            _heading = Math.IEEERemainder(value, 2 * Math.PI);
            _rotation = null;
        }
    }

    public double Speed { get; set; } = 0.005;
    public Eye Eye { get; set; }
    public Brain Brain { get; set; }
    public int Satiation { get; set; } = 0;

    public List<double> ProcessVision(IReadOnlyList<Food> foods) => Eye.ProcessVision(Position, Rotation, foods);

    internal ReadOnlySpan<double> ProcessVisionBuffered(IReadOnlyList<Food> foods)
    {
        if (_vision.Length != Eye.Cells)
            _vision = new double[Eye.Cells];
        Eye.ProcessVisionInto(Position, Heading, foods, _vision);
        return _vision;
    }

    internal static Animal FromChromosome(IRandomGenerator rng, Chromosome chromosome)
    {
        var eye = new Eye();
        var brain = Brain.FromChromosome(chromosome, eye);
        return new Animal(rng, eye, brain);
    }

    public Chromosome Chromosome => Brain.Chromosome;

    public Animal(IRandomGenerator rng)
    {
        RandomizePosition(rng);
        Eye = new Eye();
        Brain = new(rng, Eye);
    }

    public Animal(IRandomGenerator rng, Eye eye, Brain brain)
    {
        RandomizePosition(rng);
        Eye = eye;
        Brain = brain;
    }

    private void RandomizePosition(IRandomGenerator rng)
    {
        Position = new(rng.GetRandomNumberInRange(0, 1), rng.GetRandomNumberInRange(0, 1), 0);
        Rotation = Rotation.FromEulerAngles(0, 0, rng.GetRandomNumberInRange(0, Math.PI * 2), "xyz");
    }
}
