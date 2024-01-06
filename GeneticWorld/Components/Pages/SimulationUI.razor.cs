using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using Blazor.Extensions;
using Blazor.Extensions.Canvas;
using Blazor.Extensions.Canvas.Canvas2D;
using GeneticWorld.Core;
using GeometRi;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GeneticWorld.Components.Pages;

public partial class SimulationUI
{
    private BECanvas? _canvasReference;
    private Canvas2DContext? _context;
    private int _canvasWidth => (int)(_canvasReference == null ? 0 : _canvasReference.Width);
    private int _canvasHeight => (int)(_canvasReference == null ? 0 : _canvasReference.Height);

    private Simulation _simulation = new(new RandomGen());

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _context = await _canvasReference.CreateCanvas2DAsync();
            await JSRuntime.InvokeAsync<object>("initSimulation", DotNetObjectReference.Create(this));
        }

        await Render();
    }

    [JSInvokable]
    public async Task ResizeCanvas(int width, int height)
    {
        ArgumentNullException.ThrowIfNull(_canvasReference);
        _canvasReference.Width = width;
        _canvasReference.Height = height;
        StateHasChanged();
    }

    private async Task Render()
    {
        await _context!.SetFillStyleAsync(ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(0, 0, 0)));

        var size = _canvasWidth * 0.03;
        foreach (var animal in _simulation.World.Animals)
        {
            var visualizedAnimal = ConstructTriangleFromIncenter(ScalePointToCanvas(animal.Position), size);
            visualizedAnimal = visualizedAnimal.Rotate(animal.Rotation, visualizedAnimal.Incenter); 
            await DrawTriangle(visualizedAnimal);
        }
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

    private async Task DrawTriangle(Triangle triangle)
    {
        await _context!.BeginPathAsync();
        await _context!.MoveToAsync(triangle.A.X, triangle.A.Y);
        await _context!.LineToAsync(triangle.B.X, triangle.B.Y);
        await _context!.LineToAsync(triangle.C.X, triangle.C.Y);
        await _context!.LineToAsync(triangle.A.X, triangle.A.Y);
        await _context!.SetFillStyleAsync(ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(0, 0, 0)));
        await _context!.FillAsync();
    }
}
