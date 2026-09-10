using GeometRi;

namespace Simulation;

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
    public double FovRange { get; set; } = Parameters.FovRange;

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
    public double FovAngle { get; set; } = Parameters.FovAngle;

    /// How much photoreceptors there are in a single eye.
    ///
    /// More cells means our birds will have more "crisp" vision, allowing
    /// them to locate the food more precisely - but the trade-off is that
    /// the evolution process will then take longer, or even fail, unable
    /// to find any solution.
    ///
    /// I've found values between 3~11 sufficient, with eyes having more
    /// than ~20 photoreceptors yielding progressively worse results.
    public int Cells { get; set; } = Parameters.EyeCells;

    public List<double> ProcessVision(Point3d position, Rotation rotation, IReadOnlyList<Food> foods)
    {
        var cells = new double[Cells];
        ProcessVisionInto(position, rotation, foods, cells);
        return cells.ToList();
    }

    public void ProcessVisionInto(Point3d position, Rotation rotation, IReadOnlyList<Food> foods, Span<double> cells)
        => ProcessVisionInto(position, rotation.ToEulerAngles("xyz")[2], foods, cells);

    internal void ProcessVisionInto(Point3d position, double heading, IReadOnlyList<Food> foods, Span<double> cells)
    {
        if (cells.Length != Cells)
            throw new ArgumentException("Vision buffer must match the eye cell count", nameof(cells));

        cells.Clear();
        var halfFovAngle = FovAngle / 2.0;
        var rangeSquared = FovRange * FovRange;

        foreach (var food in foods)
        {
            var dx = WorldGeometry.ShortestDelta(food.Position.X - position.X);
            var dy = WorldGeometry.ShortestDelta(food.Position.Y - position.Y);
            var dz = food.Position.Z - position.Z;
            var distanceSquared = dx * dx + dy * dy + dz * dz;

            if (FovRange <= 0 || distanceSquared >= rangeSquared || distanceSquared == 0)
            {
                continue;
            }

            var distance = Math.Sqrt(distanceSquared);
            // Same signed angle to +Y, without allocating 3D vectors.
            var angle = Math.Acos(Math.Clamp(dy / distance, -1.0, 1.0)) * (dx > 0 ? -1.0 : 1.0);

            angle -= heading;
            angle = WrapAngle(angle, -Math.PI, Math.PI);

            if (DoubleComparer.LessThan(angle, -halfFovAngle) || DoubleComparer.GreaterThan(angle, halfFovAngle))
            {
                continue;
            }

            angle = angle + halfFovAngle;
            var cell = angle / FovAngle;
            cell *= Cells;

            int selectedCell = Math.Min((int)cell, cells.Length - 1);

            var energy = (FovRange - distance) / FovRange;

            cells[selectedCell] += energy;
        }
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
