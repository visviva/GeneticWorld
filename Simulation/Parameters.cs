namespace Simulation;

public static class Parameters
{
    public static int NumberOfBirds { get; set; } = 30;
    public static int NumberOfFoods { get; set; } = 40;
    public static double SpeedMin { get; set; } = 0.001;
    public static double SpeedMax { get; set; } = 0.005;
    public static double SpeedAccel { get; set; } = 0.2;
    public static double RotationAccel { get; set; } = Math.PI / 2.0;
    public static int GenerationLength { get; set; } = 2500;
    public static int Neurons { get; set; } = 18;
    public static int EyeCells { get; set; } = 9;
    public static double FovRange { get; set; } = 0.25;
    public static double FovAngle { get; set; } = Math.PI + Math.PI / 4;
}
