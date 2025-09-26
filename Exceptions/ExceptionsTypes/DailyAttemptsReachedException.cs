namespace AuthApi.Exceptions.ExceptionsTypes
{
    public class DailyAttemptsReachedException : Exception
    {
        public DailyAttemptsReachedException(string message) : base(message) { }
    }
}
