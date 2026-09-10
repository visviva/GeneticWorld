using Cortex;
using GeometRi;

namespace SimulationTest;

[TestClass]
public class FastSimulationTest
{
    [TestMethod]
    public void BufferedVisionMatchesGeometryReferenceAndClearsPreviousSignals()
    {
        var random = new Random(42);
        var rng = new RandomGen();
        var eye = new Eye { Cells = 13, FovRange = 0.4, FovAngle = Math.PI * 1.25 };
        var output = new double[eye.Cells];
        var foods = Enumerable.Range(0, 40).Select(_ => new Food(rng)).ToList();
        for (var sample = 0; sample < 100; sample++)
        {
            var position = new Point3d(random.NextDouble(), random.NextDouble(), 0);
            var rotation = Rotation.FromEulerAngles(0, 0, random.NextDouble() * 2 * Math.PI, "xyz");
            var heading = rotation.ToEulerAngles("xyz")[2];
            var expected = new double[eye.Cells];
            foreach (var food in foods)
            {
                food.Position = new Point3d(random.NextDouble(), random.NextDouble(), 0);
                var dx = food.Position.X - position.X;
                var dy = food.Position.Y - position.Y;
                dx = dx > 0.5 ? dx - 1 : dx < -0.5 ? dx + 1 : dx;
                dy = dy > 0.5 ? dy - 1 : dy < -0.5 ? dy + 1 : dy;
                var vector = new Vector3d(dx, dy, 0);
                var distance = vector.Norm;
                if (distance >= eye.FovRange) continue;
                var angle = vector.AngleTo(new Vector3d(0, 1, 0)) * (dx > 0 ? -1 : 1) - heading;
                while (angle < -Math.PI) angle += 2 * Math.PI;
                while (angle > Math.PI) angle -= 2 * Math.PI;
                if (Eye.DoubleComparer.LessThan(angle, -eye.FovAngle / 2) ||
                    Eye.DoubleComparer.GreaterThan(angle, eye.FovAngle / 2)) continue;
                var cell = Math.Min((int)((angle + eye.FovAngle / 2) / eye.FovAngle * eye.Cells), eye.Cells - 1);
                expected[cell] += (eye.FovRange - distance) / eye.FovRange;
            }

            eye.ProcessVisionInto(position, rotation, foods, output);
            for (var cell = 0; cell < eye.Cells; cell++)
                Assert.AreEqual(expected[cell], output[cell], 1e-12);
        }

        eye.ProcessVisionInto(new Point3d(0, 0, 0), Rotation.FromEulerAngles(0, 0, 0, "xyz"), [], output);
        Assert.IsTrue(output.All(value => value == 0));
    }

    [TestMethod]
    public void CoincidentFoodHasNoDirectionalSignal()
    {
        var eye = new Eye();
        var position = new Point3d(0.5, 0.5, 0);
        var food = new Food(new RandomGen()) { Position = position };
        var vision = eye.ProcessVision(position, Rotation.FromEulerAngles(0, 0, 0, "xyz"), [food]);
        Assert.IsTrue(vision.All(value => value == 0));
    }

    [TestMethod]
    public void PlanarMovementMatchesRotationAcrossManyTurnsAndWraps()
    {
        var rng = new RandomGen();
        var simulation = new Simulation.Simulation(rng) { SpeedAccel = 0, RotationAccel = 0.2 };
        var animal = new Animal(rng)
        {
            Position = new Point3d(0.998, 0.999, 0),
            Rotation = Rotation.FromEulerAngles(0, 0, 3.1, "xyz"),
            Brain = new Brain(new Network([new Layer([
                new Neuron(0.75, Enumerable.Repeat(0.0, Parameters.EyeCells).ToList()),
                new Neuron(0.25, Enumerable.Repeat(0.0, Parameters.EyeCells).ToList())])]))
        };
        simulation.World.Animals = [animal];
        simulation.World.Foods = [];
        var expectedPosition = animal.Position;
        var expectedRotation = animal.Rotation;
        for (var step = 0; step < 200; step++)
        {
            expectedRotation = Rotation.FromEulerAngles(0, 0, expectedRotation.ToEulerAngles("xyz")[2] + 0.1, "xyz");
            var next = expectedPosition.Translate(expectedRotation * new Vector3d(0, animal.Speed, 0));
            expectedPosition = new Point3d((next.X % 1 + 1) % 1, (next.Y % 1 + 1) % 1, 0);
            simulation.step();
            Assert.AreEqual(expectedPosition.X, animal.Position.X, 1e-11);
            Assert.AreEqual(expectedPosition.Y, animal.Position.Y, 1e-11);
            Assert.AreEqual(expectedRotation.ToEulerAngles("xyz")[2], animal.Rotation.ToEulerAngles("xyz")[2], 1e-11);
        }
    }

    [TestMethod]
    public void GenerationBoundaryEmitsStatisticsOnceAndResetsProgress()
    {
        var simulation = new Simulation.Simulation(new RandomGen()) { GenerationLength = 3 };
        var events = 0;
        simulation.BeforeEvolutionHook += (_, _) => events++;
        for (var i = 0; i < 3; i++)
            Assert.AreEqual(Simulation.Simulation.SimulationResult.CurrentGeneration, simulation.step());
        Assert.AreEqual(Simulation.Simulation.SimulationResult.NewGeneration, simulation.step());
        Assert.AreEqual(1, events);
        Assert.AreEqual(1, simulation.Cycle);
        Assert.AreEqual(0.0, simulation.Percentage);
    }
}
