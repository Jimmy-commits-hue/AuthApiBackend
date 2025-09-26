namespace AuthApi.Exceptions.ExceptionsTypes
{
    public class MaximumAttemptsException : Exception
    {
        public MaximumAttemptsException(string message) : base(message) { }

    }

}
