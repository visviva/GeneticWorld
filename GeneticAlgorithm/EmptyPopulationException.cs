using System.Runtime.Serialization;

namespace Evolution;

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
