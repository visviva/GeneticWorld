using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using ApexCharts;
using EvolutionSim.Components;
using EvolutionSim.Utility;
using GeometRi;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using Simulation;

namespace EvolutionSim.Pages;

public partial class SimulationEvolution
{
    private int _canvasWidth { get; set; } = 0;
    private int _canvasHeight { get; set; } = 0;
    private int _progress => (int)(_simulation.Percentage * 100.0);

    private Simulation.Simulation _simulation = new(new RandomGen());

    EvolutionSim.Components.Chart _chart = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeAsync<object>("initSimulation", DotNetObjectReference.Create(this));
            _simulation.BeforeEvolutionHook += AddNewStatisticsSet;
        }
    }

    private void AddNewStatisticsSet(object? sender, EvolutionStatistics statistics)
    {
        List<EvolutionStatistics> listStats = [statistics];
        _chart.AddStatistics(listStats);
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

        double size = _canvasWidth * 0.02;
        int radius = (int)(_canvasWidth * 0.006);

        var triangles = _simulation.World.Animals.Select(animal =>
        {
            var visualizedAnimal = Utility.Utility.ConstructTriangleFromIncenter(Utility.Utility.ScalePointToCanvas(animal.Position, _canvasWidth, _canvasHeight), size);
            visualizedAnimal = visualizedAnimal.Rotate(animal.Rotation, visualizedAnimal.Incenter);
            return new RenderTriangle(new(visualizedAnimal.A), new(visualizedAnimal.B), new(visualizedAnimal.C));
        }).ToList();


        var circles = _simulation.World.Foods.Select(f => new RenderCircle(new(Utility.Utility.ScalePointToCanvas(f.Position, _canvasWidth, _canvasHeight)), radius)).ToList();

        var newWorld = new RenderInformation(triangles, circles);

        StateHasChanged();

        var serializedWorld = JsonSerializer.Serialize(newWorld);
        return serializedWorld;
    }

    public async Task Train()
    {
        await JSRuntime.InvokeAsync<object>("pauseSimulation");
        while (true)
        {
            var result = await Task.Run(() => _simulation.step());

            StateHasChanged();
            if (result == Simulation.Simulation.SimulationResult.NewGeneration)
            {
                break;
            }
            await Task.Delay(1);
        }
        await JSRuntime.InvokeAsync<object>("resumeSimulation");
    }

    public void Restart()
    {
        _simulation = new(new RandomGen());
        _simulation.BeforeEvolutionHook += AddNewStatisticsSet;
        _chart.Clear();
        StateHasChanged();
    }
}
