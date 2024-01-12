using GeometRi;

namespace EvolutionSim.Core;

public record struct RenderPoint(int x, int y)
{
    public RenderPoint(Point3d point) : this((int)point.X, (int)point.Y) { }
}

public record struct RenderTriangle(RenderPoint a, RenderPoint b, RenderPoint c);

public record struct RenderCircle(RenderPoint m, int radius);

public record struct RenderInformation(List<RenderTriangle> triangles, List<RenderCircle> circles);
