namespace AuthApi.Exceptions.ExceptionsTypes
{

    public class FailedToRandomizeCodeException : Exception
    {

        public FailedToRandomizeCodeException(string message) : base(message) { }

    }

}
