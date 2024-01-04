namespace GeneticWorld.Core;

public class Time
{

    private float _totalTime = 0.0f;

    public float TotalTime
    {
        get => _totalTime;
        set
        {
            ElapsedTime = value - _totalTime;
            _totalTime = value;
        }
    }
    public float ElapsedTime { get; private set; }
}
