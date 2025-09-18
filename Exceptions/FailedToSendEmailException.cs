namespace AuthApi.Exceptions
{

    public class FailedToSendEmailException : Exception
    {

        public FailedToSendEmailException(string message) : base(message) { }

    }

}
