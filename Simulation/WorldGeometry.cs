using GeometRi;

namespace Simulation;

internal static class WorldGeometry
{
    private const double WorldSize = 1.0;

    public static Vector3d ShortestDisplacement(Point3d origin, Point3d target)
    {
        return new Vector3d(
            ShortestDelta(target.X - origin.X),
            ShortestDelta(target.Y - origin.Y),
            target.Z - origin.Z);
    }

    public static double Distance(Point3d first, Point3d second)
    {
        return Math.Sqrt(DistanceSquared(first, second));
    }

    public static double DistanceSquared(Point3d first, Point3d second)
    {
        var dx = ShortestDelta(second.X - first.X);
        var dy = ShortestDelta(second.Y - first.Y);
        var dz = second.Z - first.Z;
        return dx * dx + dy * dy + dz * dz;
    }

    public static Point3d Wrap(Point3d point)
    {
        return new Point3d(WrapCoordinate(point.X), WrapCoordinate(point.Y), point.Z);
    }

    internal static double ShortestDelta(double delta)
    {
        if (delta > WorldSize / 2.0)
        {
            return delta - WorldSize;
        }

        if (delta < -WorldSize / 2.0)
        {
            return delta + WorldSize;
        }

        return delta;
    }

    internal static double WrapCoordinate(double coordinate)
    {
        coordinate %= WorldSize;
        return coordinate < 0 ? coordinate + WorldSize : coordinate;
    }
}
