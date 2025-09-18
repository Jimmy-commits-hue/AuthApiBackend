namespace AuthApi.Exceptions
{
    public class DailyAttemptsReachedException : Exception
    {
        public DailyAttemptsReachedException(string message) : base(message) { }
    }
}
