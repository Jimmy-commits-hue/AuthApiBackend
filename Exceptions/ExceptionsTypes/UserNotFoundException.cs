namespace AuthApi.Exceptions.ExceptionsTypes
{

    public class UserNotFoundException : Exception
    {

        public UserNotFoundException(string message) : base(message) { }

    }

}
