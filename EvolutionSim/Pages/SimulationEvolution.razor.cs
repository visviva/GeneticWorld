using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using EvolutionSim.Core;
using GeneticWorld.Core;
using GeometRi;
using Microsoft.JSInterop;

namespace EvolutionSim.Pages;

public partial class SimulationEvolution
{
    private int _canvasWidth { get; set; } = 0;
    private int _canvasHeight { get; set; } = 0;
    private int _progress => (int)(_simulation.Percentage * 100.0);

    private Simulation _simulation = new(new RandomGen());

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeAsync<object>("initSimulation", DotNetObjectReference.Create(this));
        }
    }

    [JSInvokable]
    public void ResizeCanvas(int width, int height)
    {
        _canvasWidth = width;
        _canvasHeight = height;
        StateHasChanged();
    }

    [JSInvokable]
    public string Update(float time)
    {
        _simulation.step();

        double size = _canvasWidth * 0.03;
        int radius = (int)(_canvasWidth * 0.008);

        var triangles = _simulation.World.Animals.Select(animal =>
        {
            var visualizedAnimal = ConstructTriangleFromIncenter(ScalePointToCanvas(animal.Position), size);
            visualizedAnimal = visualizedAnimal.Rotate(animal.Rotation, visualizedAnimal.Incenter);
            return new RenderTriangle(new(visualizedAnimal.A), new(visualizedAnimal.B), new(visualizedAnimal.C));
        }).ToList();


        var circles = _simulation.World.Foods.Select(f => new RenderCircle(new(ScalePointToCanvas(f.Position)), radius)).ToList();

        var newWorld = new RenderInformation(triangles, circles);

        StateHasChanged();

        var serializedWorld = JsonSerializer.Serialize(newWorld);
        return serializedWorld;
    }

    public async Task Train()
    {
        _simulation.Train();
    }

    private Point3d ScalePointToCanvas(Point3d p) => new(p.X * _canvasWidth, p.Y * _canvasHeight, 0);

    private Triangle ConstructTriangleFromIncenter(Point3d incenter, double scale)
    {
        // Calculate the base length as a function of the scale
        double baseLength = scale / 1.2;

        // Calculate the height using Pythagorean theorem (assuming equal sides are 'scale' length)
        double height = Math.Sqrt(scale * scale - (baseLength / 2.0f) * (baseLength / 2.0f));

        // Vertex A (top vertex, along the line of symmetry)
        var vertexA = new Point3d(incenter.X, incenter.Y - height, 0);

        // Vertex B and C (base vertices)
        var vertexB = new Point3d(incenter.X - baseLength / 2.0f, incenter.Y + (scale - height), 0);
        var vertexC = new Point3d(incenter.X + baseLength / 2.0f, incenter.Y + (scale - height), 0);

        return new Triangle(vertexA, vertexB, vertexC);
    }
}
