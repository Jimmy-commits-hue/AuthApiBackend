namespace AuthApi.Exceptions.ExceptionsTypes
{

    public class UserNotVerifiedException : Exception
    {

        public UserNotVerifiedException(string message) : base(message) { }

    }

}
