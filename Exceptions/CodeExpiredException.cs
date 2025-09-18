namespace AuthApi.Exceptions
{

    public class CodeExpiredException : Exception
    {

        public CodeExpiredException(string message) : base(message) { }

    }

}
