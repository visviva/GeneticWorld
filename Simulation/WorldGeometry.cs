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
        return ShortestDisplacement(first, second).Norm;
    }

    public static Point3d Wrap(Point3d point)
    {
        return new Point3d(WrapCoordinate(point.X), WrapCoordinate(point.Y), point.Z);
    }

    private static double ShortestDelta(double delta)
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

    private static double WrapCoordinate(double coordinate)
    {
        coordinate %= WorldSize;
        return coordinate < 0 ? coordinate + WorldSize : coordinate;
    }
}
