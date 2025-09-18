namespace AuthApi.Exceptions
{
    public class MaximumAttemptsException : Exception
    {
        public MaximumAttemptsException(string message) : base(message) { }

    }

}
