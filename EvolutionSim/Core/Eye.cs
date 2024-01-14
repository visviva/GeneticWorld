using GeneticWorld.Core;
using GeometRi;

namespace EvolutionSim.Core;

public class Eye
{
    /// How far our eye can see:
    ///
    /// -----------------
    /// |               |
    /// |               |
    /// |               |
    /// |@      %      %|
    /// |               |
    /// |               |
    /// |               |
    /// -----------------
    ///
    /// If @ marks our birdie and % marks food, then a FOV_RANGE of:
    ///
    /// - 0.1 = 10% of the map = bird sees no foods (at least in this case)
    /// - 0.5 = 50% of the map = bird sees one of the foods
    /// - 1.0 = 100% of the map = bird sees both foods
    public double FovRange { get; set; } = 0.25;

    /// How wide our eye can see.
    ///
    /// If @> marks our birdie (rotated to the right) and . marks the area
    /// our birdie sees, then a FOV_ANGLE of:
    ///
    /// - PI/2 = 90° =
    ///   -----------------
    ///   |             /.|
    ///   |           /...|
    ///   |         /.....|
    ///   |       @>......|
    ///   |         \.....|
    ///   |           \...|
    ///   |             \.|
    ///   -----------------
    ///
    /// - PI = 180° =
    ///   -----------------
    ///   |       |.......|
    ///   |       |.......|
    ///   |       |.......|
    ///   |       @>......|
    ///   |       |.......|
    ///   |       |.......|
    ///   |       |.......|
    ///   -----------------
    ///
    /// - 2 * PI = 360° =
    ///   -----------------
    ///   |...............|
    ///   |...............|
    ///   |...............|
    ///   |.......@>......|
    ///   |...............|
    ///   |...............|
    ///   |...............|
    ///   -----------------
    ///
    /// Field of view depends on both FOV_RANGE and FOV_ANGLE:
    ///
    /// - FOV_RANGE=0.4, FOV_ANGLE=PI/2:
    ///   -----------------
    ///   |       @       |
    ///   |     /.v.\     |
    ///   |   /.......\   |
    ///   |   ---------   |
    ///   |               |
    ///   |               |
    ///   |               |
    ///   -----------------
    ///
    /// - FOV_RANGE=0.5, FOV_ANGLE=2*PI:
    ///   -----------------
    ///   |               |
    ///   |      ---      |
    ///   |     /...\     |
    ///   |    |..@..|    |
    ///   |     \.../     |
    ///   |      ---      |
    ///   |               |
    ///   ----------------- <summary>
    public double FovAngle { get; set; } = Math.PI + Math.PI / 4;

    /// How much photoreceptors there are in a single eye.
    ///
    /// More cells means our birds will have more "crisp" vision, allowing
    /// them to locate the food more precisely - but the trade-off is that
    /// the evolution process will then take longer, or even fail, unable
    /// to find any solution.
    ///
    /// I've found values between 3~11 sufficient, with eyes having more
    /// than ~20 photoreceptors yielding progressively worse results.
    public int Cells { get; set; } = 9;

    public List<double> ProcessVision(Point3d position, Rotation rotation, IReadOnlyList<Food> foods)
    {
        var cells = Enumerable.Repeat(0.0, Cells).ToList();

        foreach (var food in foods)
        {
            var vec = food.Position - position;
            var distance = vec.ToVector.Norm;

            if (distance >= FovAngle)
            {
                continue;
            }

            var angle = AngleToYAxis(vec.ToVector);

            angle -= rotation.ToEulerAngles("xyz")[2];
            angle = WrapAngle(angle, -Math.PI, Math.PI);

            var halfFovAngle = FovAngle / 2.0;

            if (DoubleComparer.LessThan(angle, -halfFovAngle) || DoubleComparer.GreaterThan(angle, halfFovAngle))
            {
                continue;
            }

            angle = angle + halfFovAngle;
            var cell = angle / FovAngle;
            cell *= Cells;

            int selectedCell = Math.Min((int)cell, cells.Count - 1);

            var energy = (FovRange - distance) / FovRange;

            cells[selectedCell] += energy;
        }

        return cells;
    }

    private static double WrapAngle(double val, double min, double max)
    {
        double width = max - min;

        if (val < min)
        {
            val += width;
            while (val < min) val += width;
        }
        else if (val > max)
        {
            val -= width;
            while (val > max) val -= width;
        }

        return val;
    }

    private static double AngleToYAxis(Vector3d v)
    {
        var x = new Vector3d(0.0, 1.00, 0);
        var sign = v.X > 0.0 ? -1.0 : 1.0;
        return v.AngleTo(x) * sign;
    }

    public static class DoubleComparer
    {
        private const double Epsilon = 10E-15; // Epsilon value, can be adjusted based on required precision

        public static bool Equals(double a, double b, double epsilon = Epsilon)
        {
            return Math.Abs(a - b) < epsilon;
        }

        public static bool GreaterThan(double a, double b, double epsilon = Epsilon)
        {
            return a - b > epsilon;
        }

        public static bool LessThan(double a, double b, double epsilon = Epsilon)
        {
            return b - a > epsilon;
        }
    }
}
