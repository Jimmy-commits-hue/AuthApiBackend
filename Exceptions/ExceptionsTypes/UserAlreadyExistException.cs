namespace AuthApi.Exceptions.ExceptionsTypes
{

    public class UserAlreadyExistException : Exception
    {

        public UserAlreadyExistException(string message) : base(message) { }

    }

}
