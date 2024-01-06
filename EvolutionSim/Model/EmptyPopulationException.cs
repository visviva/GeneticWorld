using System.Runtime.Serialization;

namespace GeneticWorld.Model;

[Serializable]
public class EmptyPopulationException : Exception
{
    public EmptyPopulationException()
    {
    }

    public EmptyPopulationException(string? message) : base(message)
    {
    }

    public EmptyPopulationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
