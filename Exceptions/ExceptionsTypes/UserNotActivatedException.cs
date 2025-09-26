namespace AuthApi.Exceptions.ExceptionsTypes
{

    public class UserNotActivatedException : Exception
    {

        public UserNotActivatedException(string message) : base(message) { }

    }

}
