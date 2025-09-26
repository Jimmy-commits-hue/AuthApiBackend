namespace AuthApi.Exceptions.ExceptionsTypes
{

    public class CodeExpiredException : Exception
    {

        public CodeExpiredException(string message) : base(message) { }

    }

}
