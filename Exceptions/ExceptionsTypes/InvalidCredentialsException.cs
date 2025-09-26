namespace AuthApi.Exceptions.ExceptionsTypes
{

    public class InvalidCredentialsException : Exception
    {

        public InvalidCredentialsException(string message) : base(message) { }

    }

}
