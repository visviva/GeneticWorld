using GeometRi;

namespace SimulationTest;

[TestClass]
public class MovementTest
{
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
}
