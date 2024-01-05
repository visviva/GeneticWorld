using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Blazor.Extensions;
using Blazor.Extensions.Canvas;
using Blazor.Extensions.Canvas.Canvas2D;
using GeneticWorld.Core;
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
        foreach (var animal in _simulation.World.Animals)
        {
            var x = animal.Position.X * _canvasWidth;
            var y = animal.Position.Y * _canvasHeight;
            await _context.FillRectAsync(x, y, 15, 15);
        }
    }
}
