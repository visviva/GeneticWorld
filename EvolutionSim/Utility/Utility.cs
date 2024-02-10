using GeometRi;

namespace EvolutionSim.Utility;

public record struct RenderPoint(int x, int y)
{
    public RenderPoint(Point3d point) : this((int)point.X, (int)point.Y) { }
}

public record struct RenderTriangle(RenderPoint a, RenderPoint b, RenderPoint c);

public record struct RenderCircle(RenderPoint m, int radius);

public record struct RenderInformation(List<RenderTriangle> triangles, List<RenderCircle> circles);

public static class Utility
{
    public static Triangle ConstructTriangleFromIncenter(Point3d incenter, double scale)
    {
        // Calculate the base length as a function of the scale
        double baseLength = scale / 1.3;

        // Calculate the height using Pythagorean theorem (assuming equal sides are 'scale' length)
        double height = Math.Sqrt(scale * scale - (baseLength / 2.0f) * (baseLength / 2.0f));

        // Vertex A (top vertex, along the line of symmetry)
        var vertexA = new Point3d(incenter.X, incenter.Y - height, 0);

        // Vertex B and C (base vertices)
        var vertexB = new Point3d(incenter.X - baseLength / 2.0f, incenter.Y + (scale - height), 0);
        var vertexC = new Point3d(incenter.X + baseLength / 2.0f, incenter.Y + (scale - height), 0);

        return new Triangle(vertexA, vertexB, vertexC);
    }

    public static Point3d ScalePointToCanvas(Point3d p, int width, int height) => new(p.X * width, p.Y * height, 0);

}
