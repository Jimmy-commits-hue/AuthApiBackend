namespace AuthApi.Exceptions
{

    public class FailedToRandomizeCodeException : Exception
    {

        public FailedToRandomizeCodeException(string message) : base(message) { }

    }

}
