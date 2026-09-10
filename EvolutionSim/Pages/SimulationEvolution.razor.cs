using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
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
    private int _lastRenderedProgress = -1;
    private bool _isTraining;
    private int _generation => _simulation.Cycle + 1;
    private int _bestScore => _simulation.World.Animals.Count == 0 ? 0 : _simulation.World.Animals.Max(animal => animal.Satiation);
    private double _meanScore => _simulation.World.Animals.Count == 0 ? 0 : _simulation.World.Animals.Average(animal => animal.Satiation);

    private Simulation.Simulation _simulation = new(new RandomGen());

    private EvolutionSim.Components.Chart _chart = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeAsync<object>("initSimulation", DotNetObjectReference.Create(this));
            _simulation.BeforeEvolutionHook += AddNewStatisticsSet;
        }
    }

    private async void AddNewStatisticsSet(object? sender, EvolutionStatistics statistics)
    {
        List<EvolutionStatistics> listStats = [statistics];
        await InvokeAsync(() => _chart.AddStatistics(listStats));
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
        // An animation callback may already be in flight when fast-forward starts.
        if (!_isTraining)
            _simulation.step();

        double size = _canvasWidth * 0.016;
        int radius = (int)(_canvasWidth * 0.006);

        var creatures = _simulation.World.Animals.Select(animal =>
        {
            var position = Utility.Utility.ScalePointToCanvas(animal.Position, _canvasWidth, _canvasHeight);
            var heading = animal.Heading;
            return new RenderCreature(new(position), heading, size);
        }).ToList();


        var circles = _simulation.World.Foods.Select(f => new RenderCircle(new(Utility.Utility.ScalePointToCanvas(f.Position, _canvasWidth, _canvasHeight)), radius)).ToList();

        var newWorld = new RenderInformation(creatures, circles);

        if (_lastRenderedProgress != _progress)
        {
            _lastRenderedProgress = _progress;
            StateHasChanged();
        }

        var serializedWorld = JsonSerializer.Serialize(newWorld);
        return serializedWorld;
    }

    public async Task Train()
    {
        if (_isTraining)
        {
            return;
        }

        _isTraining = true;
        StateHasChanged();

        try
        {
            await JSRuntime.InvokeAsync<object>("pauseSimulation");
            var generationComplete = false;
            var batchTimer = new Stopwatch();

            while (!generationComplete)
            {
                // Bound main-thread work by time, regardless of population or generation length.
                batchTimer.Restart();
                do
                {
                    generationComplete = _simulation.step() == Simulation.Simulation.SimulationResult.NewGeneration;
                }
                while (!generationComplete && batchTimer.Elapsed.TotalMilliseconds < 12);

                if (_lastRenderedProgress != _progress || generationComplete)
                {
                    _lastRenderedProgress = _progress;
                    StateHasChanged();
                }

                if (!generationComplete)
                {
                    await Task.Delay(1);
                }
            }
        }
        finally
        {
            _isTraining = false;
            StateHasChanged();
            await JSRuntime.InvokeAsync<object>("resumeSimulation");
        }
    }

    public async Task Restart()
    {
        if (_isTraining)
        {
            return;
        }

        _simulation = new(new RandomGen());
        _simulation.BeforeEvolutionHook += AddNewStatisticsSet;
        _lastRenderedProgress = -1;
        await _chart.Clear();
        StateHasChanged();
    }
}
