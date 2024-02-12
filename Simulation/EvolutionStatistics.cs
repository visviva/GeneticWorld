namespace EvolutionSim.Utility;

public class EvolutionStatistics
{
    public int Cycle { get; }
    public double Mean { get; }
    public double Max { get; }
    public double Min { get; }

    public EvolutionStatistics(int cycle, double mean, double max, double min)
    {
        Cycle = cycle;
        Mean = mean;
        Max = max;
        Min = min;
    }
}
