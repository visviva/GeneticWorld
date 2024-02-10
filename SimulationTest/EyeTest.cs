using System;
using Cortex;
using GeometRi;

namespace SimulationTest;

[TestClass]
public partial class EyeTest
{
    class MockRandom : IRandomGenerator
    {
        public double GetRandomNumberInRange(double minimal, double maximal) => 1.0;
    }

    private string VisualizeVision(IReadOnlyList<double> vision)
    {
        return string.Concat(vision.Select(cell => cell switch
        {
            _ when cell >= 0.7 => "#",
            _ when cell >= 0.3 => "+",
            _ when cell > 0.0 => ".",
            _ => " "
        }));
    }

    Eye _testEye = new();

    [TestInitialize]
    public void Initialize()
    {
        _testEye = new();
        _testEye.Cells = 13;
        _testEye.FovRange = 1.0;
        _testEye.FovAngle = Math.PI / 2;
    }

    [DataTestMethod]
    [DataRow(1.0, "      +      ")]
    [DataRow(0.9, "      +      ")]
    [DataRow(0.8, "      +      ")]
    [DataRow(0.7, "      .      ")]
    [DataRow(0.6, "      .      ")]
    [DataRow(0.5, "             ")]
    [DataRow(0.4, "             ")]
    [DataRow(0.3, "             ")]
    [DataRow(0.2, "             ")]
    [DataRow(0.1, "             ")]
    public void TestDifferentFovRanges(double FovRange, string expectedVision)
    {
        _testEye.FovRange = FovRange;

        var eyePosition = new Point3d(0.5, 0.5, 0);
        var eyeRotation = Rotation.FromEulerAngles(0, 0, 0, "xyz");

        List<Food> foods = [new Food(new MockRandom())];
        foods[0].Position = new Point3d(0.5, 1.0, 0);

        var vision = _testEye.ProcessVision(eyePosition, eyeRotation, foods);

        var actualVision = VisualizeVision(vision);
        Assert.AreEqual(expectedVision, actualVision);
    }

    [DataTestMethod]
    [DataRow(Math.PI * 0.00, "         +   ")]
    [DataRow(Math.PI * 0.25, "        +    ")]
    [DataRow(Math.PI * 0.50, "      +      ")]
    [DataRow(Math.PI * 0.75, "    +        ")]
    [DataRow(Math.PI * 1.00, "   +         ")]
    [DataRow(Math.PI * 1.25, " +           ")]
    [DataRow(Math.PI * 1.50, "            +")]
    [DataRow(Math.PI * 1.75, "           + ")]
    [DataRow(Math.PI * 2.00, "         +   ")]
    [DataRow(Math.PI * 2.25, "        +    ")]
    [DataRow(Math.PI * 2.50, "      +      ")]
    public void TestDifferentRotations(double rotation, string expectedVision)
    {
        _testEye.FovAngle = 2 * Math.PI;

        var eyePosition = new Point3d(0.5, 0.5, 0);
        var eyeRotation = Rotation.FromEulerAngles(0, 0, rotation, "xyz");

        List<Food> foods = [new Food(new MockRandom())];
        foods[0].Position = new Point3d(0.0, 0.5, 0);

        var vision = _testEye.ProcessVision(eyePosition, eyeRotation, foods);

        var actualVision = VisualizeVision(vision);
        Assert.AreEqual(expectedVision, actualVision);
    }

    [DataTestMethod]
    // Checking the X axis:
    [DataRow(0.9, 0.5, "#           #")]
    [DataRow(0.8, 0.5, "  #       #  ")]
    [DataRow(0.7, 0.5, "   +     +   ")]
    [DataRow(0.6, 0.5, "    +   +    ")]
    [DataRow(0.5, 0.5, "    +   +    ")]
    [DataRow(0.4, 0.5, "     + +     ")]
    [DataRow(0.3, 0.5, "     . .     ")]
    [DataRow(0.2, 0.5, "     . .     ")]
    [DataRow(0.1, 0.5, "     . .     ")]
    [DataRow(0.0, 0.5, "             ")]

    // Checking the Y axis:
    [DataRow(0.5, 0.0, "            +")]
    [DataRow(0.5, 0.1, "          + .")]
    [DataRow(0.5, 0.2, "         +  +")]
    [DataRow(0.5, 0.3, "        + +  ")]
    [DataRow(0.5, 0.4, "      +  +   ")]
    [DataRow(0.5, 0.6, "   +  +      ")]
    [DataRow(0.5, 0.7, "  + +        ")]
    [DataRow(0.5, 0.8, "+  +         ")]
    [DataRow(0.5, 0.9, ". +          ")]
    [DataRow(0.5, 1.0, "+            ")]
    public void TestDifferentPositions(double x, double y, string expectedVision)
    {
        var eyePosition = new Point3d(x, y, 0);
        var eyeRotation = Rotation.FromEulerAngles(0, 0, 3.0 * Math.PI / 2.0, "xyz");

        List<Food> foods = [new Food(new MockRandom()), new Food(new MockRandom())];
        foods[0].Position = new Point3d(1.0, 0.4, 0);
        foods[1].Position = new Point3d(1.0, 0.6, 0);

        var vision = _testEye.ProcessVision(eyePosition, eyeRotation, foods);

        var actualVision = VisualizeVision(vision);
        Assert.AreEqual(expectedVision, actualVision);
    }

    [DataTestMethod]
    [DataRow(0.25 * Math.PI, " +         + ")] // FOV is narrow = 2 foods
    [DataRow(0.50 * Math.PI, ".  +     +  .")]
    [DataRow(0.75 * Math.PI, "  . +   + .  ")] // FOV gets progressively
    [DataRow(1.00 * Math.PI, "   . + + .   ")] // wider and wider...
    [DataRow(1.25 * Math.PI, "   . + + .   ")]
    [DataRow(1.50 * Math.PI, ".   .+ +.   .")]
    [DataRow(1.75 * Math.PI, ".   .+ +.   .")]
    [DataRow(2.00 * Math.PI, "+.  .+ +.  .+")] // FOV is the widest = 8 foods
    public void TestDifferentFovAngles(double fov, string expectedVision)
    {
        _testEye.FovAngle = fov;

        var eyePosition = new Point3d(0.5, 0.5, 0);
        var eyeRotation = Rotation.FromEulerAngles(0, 0, 3.0 * Math.PI / 2.0, "xyz");

        List<Food> foods = [
            new Food(new MockRandom()),
            new Food(new MockRandom()),
            new Food(new MockRandom()),
            new Food(new MockRandom()),
            new Food(new MockRandom()),
            new Food(new MockRandom()),
            new Food(new MockRandom()),
            new Food(new MockRandom()),
        ];
        foods[0].Position = new Point3d(0.0, 0.0, 0);
        foods[1].Position = new Point3d(0.0, 0.33, 0);
        foods[2].Position = new Point3d(0.0, 0.66, 0);
        foods[3].Position = new Point3d(0.0, 1.0, 0);
        foods[4].Position = new Point3d(1.0, 0.0, 0);
        foods[5].Position = new Point3d(1.0, 0.33, 0);
        foods[6].Position = new Point3d(1.0, 0.66, 0);
        foods[7].Position = new Point3d(1.0, 1.0, 0);

        var vision = _testEye.ProcessVision(eyePosition, eyeRotation, foods);

        var actualVision = VisualizeVision(vision);
        Assert.AreEqual(expectedVision, actualVision);
    }
}

