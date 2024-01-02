namespace GeneticWorld.Model
{
    public class MismatchedInputSizeException : Exception
    {
        public MismatchedInputSizeException()
        {
        }

        public MismatchedInputSizeException(string? message) : base(message)
        {
        }

        public MismatchedInputSizeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
