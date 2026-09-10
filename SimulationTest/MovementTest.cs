using Cortex;
using GeometRi;

namespace SimulationTest;

[TestClass]
public class MovementTest
{
    private static Brain CreateConstantBrain(int inputCount, double firstOutput, double secondOutput)
    {
        var firstNeuron = new Neuron(firstOutput, Enumerable.Repeat(0.0, inputCount).ToList());
        var secondNeuron = new Neuron(secondOutput, Enumerable.Repeat(0.0, inputCount).ToList());
        return new Brain(new Network([new Layer([firstNeuron, secondNeuron])]));
    }

    [TestMethod]
    public void ZeroRotationMovesAnimalTowardEyesForwardAxis()
    {
        var random = new RandomGen();
        var simulation = new Simulation.Simulation(random)
        {
            SpeedAccel = 0,
            RotationAccel = 0
        };
        var animal = new Animal(random)
        {
            Position = new Point3d(0.5, 0.5, 0),
            Rotation = Rotation.FromEulerAngles(0, 0, 0, "xyz"),
            Speed = 0.005
        };
        simulation.World.Animals = [animal];
        simulation.World.Foods = [];

        simulation.step();

        Assert.AreEqual(0.5, animal.Position.X, 1e-12);
        Assert.AreEqual(0.505, animal.Position.Y, 1e-12);
    }

    [TestMethod]
    public void NegativeRotationRemainsNegativeWithoutSteering()
    {
        var random = new RandomGen();
        var simulation = new Simulation.Simulation(random)
        {
            SpeedAccel = 0,
            RotationAccel = 0
        };
        var animal = new Animal(random)
        {
            Position = new Point3d(0.5, 0.5, 0),
            Rotation = Rotation.FromEulerAngles(0, 0, -1.0, "xyz"),
            Speed = 0.005
        };
        simulation.World.Animals = [animal];
        simulation.World.Foods = [];

        simulation.step();

        Assert.AreEqual(-1.0, animal.Rotation.ToEulerAngles("xyz")[2], 1e-12);
    }

    [TestMethod]
    public void MotorOutputScalesSpeedAcceleration()
    {
        var random = new RandomGen();
        var simulation = new Simulation.Simulation(random)
        {
            SpeedMin = 0,
            SpeedMax = 1,
            SpeedAccel = 0.2,
            RotationAccel = 0
        };
        var animal = new Animal(random)
        {
            Brain = CreateConstantBrain(Parameters.EyeCells, 0.75, 0.75),
            Position = new Point3d(0.5, 0.5, 0),
            Speed = 0.5
        };
        simulation.World.Animals = [animal];
        simulation.World.Foods = [];

        simulation.step();

        Assert.AreEqual(0.6, animal.Speed, 1e-12);
    }

    [TestMethod]
    public void MotorOutputScalesRotationAcceleration()
    {
        var random = new RandomGen();
        var simulation = new Simulation.Simulation(random)
        {
            SpeedAccel = 0,
            RotationAccel = 0.2
        };
        var animal = new Animal(random)
        {
            Brain = CreateConstantBrain(Parameters.EyeCells, 0.75, 0.25),
            Position = new Point3d(0.5, 0.5, 0),
            Rotation = Rotation.FromEulerAngles(0, 0, 0, "xyz")
        };
        simulation.World.Animals = [animal];
        simulation.World.Foods = [];

        simulation.step();

        Assert.AreEqual(0.1, animal.Rotation.ToEulerAngles("xyz")[2], 1e-12);
    }

    [TestMethod]
    public void MovementPreservesOvershootWhenWrapping()
    {
        var random = new RandomGen();
        var simulation = new Simulation.Simulation(random)
        {
            SpeedAccel = 0,
            RotationAccel = 0
        };
        var animal = new Animal(random)
        {
            Position = new Point3d(0.5, 0.999, 0),
            Rotation = Rotation.FromEulerAngles(0, 0, 0, "xyz"),
            Speed = 0.005
        };
        simulation.World.Animals = [animal];
        simulation.World.Foods = [];

        simulation.step();

        Assert.AreEqual(0.004, animal.Position.Y, 1e-12);
    }

    [TestMethod]
    public void CollisionAcrossWorldEdgeConsumesFood()
    {
        var random = new RandomGen();
        var simulation = new Simulation.Simulation(random)
        {
            SpeedAccel = 0,
            RotationAccel = 0
        };
        var animal = new Animal(random)
        {
            Position = new Point3d(0.99, 0.5, 0)
        };
        var food = new Food(random)
        {
            Position = new Point3d(0.005, 0.5, 0)
        };
        simulation.World.Animals = [animal];
        simulation.World.Foods = [food];

        simulation.step();

        Assert.AreEqual(1, animal.Satiation);
    }
}
