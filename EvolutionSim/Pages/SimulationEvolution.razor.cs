using System.Collections.Concurrent;
using System.Drawing;
using System.Text.Json;
using Blazor.Extensions;
using Blazor.Extensions.Canvas;
using Blazor.Extensions.Canvas.Canvas2D;
using GeneticWorld.Core;
using GeometRi;
using Microsoft.JSInterop;

namespace EvolutionSim.Pages;

public partial class SimulationEvolution
{
    private BECanvas? _canvasReference;
    private Canvas2DContext? _context;
    private int _canvasWidth => (int)(_canvasReference == null ? 0 : _canvasReference.Width);
    private int _canvasHeight => (int)(_canvasReference == null ? 0 : _canvasReference.Height);

    private Simulation _simulation = new(new RandomGen());

    readonly private List<Triangle> _animalVisualizations = new();

    readonly private ConcurrentBag<Triangle> _animalsBag = new();


    record struct RenderPoint(int x, int y);
    record struct RenderTriangle(RenderPoint a, RenderPoint b, RenderPoint c);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _context = await _canvasReference.CreateCanvas2DAsync();
            await JSRuntime.InvokeAsync<object>("initSimulation", DotNetObjectReference.Create(this));
        }
    }

    [JSInvokable]
    public async Task ResizeCanvas(int width, int height)
    {
        ArgumentNullException.ThrowIfNull(_canvasReference);
        _canvasReference.Width = width;
        _canvasReference.Height = height;
        StateHasChanged();
    }

    public async ValueTask<string> Update(float time)
    {
        _simulation.step();
        _animalVisualizations.Clear();
        _animalsBag.Clear();

        var size = _canvasWidth * 0.03;

        Parallel.ForEach(_simulation.World.Animals, (animal) =>
        {
            var visualizedAnimal = ConstructTriangleFromIncenter(ScalePointToCanvas(animal.Position), size);
            visualizedAnimal = visualizedAnimal.Rotate(animal.Rotation, visualizedAnimal.Incenter);
            _animalsBag.Add(visualizedAnimal);
        });

        var newWorld = new List<RenderTriangle>(_simulation.World.Animals.Count);
        foreach (var t in _animalsBag)
        {
            newWorld.Add(new RenderTriangle(new RenderPoint((int)t.A.X, (int)t.A.Y), new RenderPoint((int)t.B.X, (int)t.B.Y), new RenderPoint((int)t.C.X, (int)t.C.Y)));
        }

        var serializedWorld = JsonSerializer.Serialize(newWorld);
        return serializedWorld;
    }

    [JSInvokable]
    public async ValueTask<string> Loop(float time)
    {
        return await Update(time);
    }

    private Point3d ScalePointToCanvas(Point3d p) => new(p.X * _canvasWidth, p.Y * _canvasHeight, 0);

    private Triangle ConstructTriangleFromIncenter(Point3d incenter, double scale)
    {
        // Calculate the base length as a function of the scale
        double baseLength = scale / 1.2;

        // Calculate the height using Pythagorean theorem (assuming equal sides are 'scale' length)
        double height = (float)Math.Sqrt(scale * scale - (baseLength / 2.0f) * (baseLength / 2.0f));

        // Vertex A (top vertex, along the line of symmetry)
        var vertexA = new Point3d(incenter.X, incenter.Y - height, 0);

        // Vertex B and C (base vertices)
        var vertexB = new Point3d(incenter.X - baseLength / 2.0f, incenter.Y + (scale - height), 0);
        var vertexC = new Point3d(incenter.X + baseLength / 2.0f, incenter.Y + (scale - height), 0);

        return new Triangle(vertexA, vertexB, vertexC);
    }
}
