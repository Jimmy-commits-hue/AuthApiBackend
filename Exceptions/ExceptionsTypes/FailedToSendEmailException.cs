namespace AuthApi.Exceptions.ExceptionsTypes
{

    public class FailedToSendEmailException : Exception
    {

        public FailedToSendEmailException(string message) : base(message) { }

    }

}
