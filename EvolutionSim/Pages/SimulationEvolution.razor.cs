﻿using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using EvolutionSim.Core;
using GeneticWorld.Core;
using GeometRi;
using Microsoft.FluentUI.AspNetCore.Components;
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

        double size = _canvasWidth * 0.02;
        int radius = (int)(_canvasWidth * 0.006);

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
        await JSRuntime.InvokeAsync<object>("pauseSimulation");
        while (true)
        {
            var result = await Task.Run(() => _simulation.step());

            StateHasChanged();
            if (result == Simulation.SimulationResult.NewGeneration)
            {
                break;
            }
            await Task.Delay(1);
        }
        await JSRuntime.InvokeAsync<object>("resumeSimulation");
    }

    public async Task Restart()
    {
        _simulation = new(new RandomGen());
        StateHasChanged();
    }

    private Point3d ScalePointToCanvas(Point3d p) => new(p.X * _canvasWidth, p.Y * _canvasHeight, 0);

    private Triangle ConstructTriangleFromIncenter(Point3d incenter, double scale)
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

    string? activeid = "tab-1";
    FluentTab? changedto;

    private void HandleOnTabChange(FluentTab tab)
    {
        changedto = tab;
    }

    private int NumberOfBirds
    {
        get => Parameters.NumberOfBirds;
        set
        {
            Parameters.NumberOfBirds = value;
        }
    }

    private int NumberOfFoods
    {
        get => Parameters.NumberOfFoods;
        set
        {
            Parameters.NumberOfFoods = value;
        }
    }

    private double SpeedMin
    {
        get => Parameters.SpeedMin;
        set
        {
            Parameters.SpeedMin = value;
        }
    }
    private double SpeedMax
    {
        get => Parameters.SpeedMax;
        set
        {
            Parameters.SpeedMax = value;
        }
    }
    private double SpeedAccel
    {
        get => Parameters.SpeedAccel;
        set
        {
            Parameters.SpeedAccel = value;
        }
    }
    private double RotationAccel
    {
        get => Parameters.RotationAccel;
        set
        {
            Parameters.RotationAccel = value;
        }
    }
    private int GenerationLength
    {
        get => Parameters.GenerationLength;
        set
        {
            Parameters.GenerationLength = value;
        }
    }
    private int Neurons
    {
        get => Parameters.Neurons;
        set
        {
            Parameters.Neurons = value;
        }
    }
    private int EyeCells
    {
        get => Parameters.EyeCells;
        set
        {
            Parameters.EyeCells = value;
        }
    }
    private double FovRange
    {
        get => Parameters.FovRange;
        set
        {
            Parameters.FovRange = value;
        }
    }
    private double FovAngle
    {
        get => Parameters.FovAngle;
        set
        {
            Parameters.FovAngle = value;
        }
    }
}
