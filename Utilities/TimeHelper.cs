namespace AuthApi.Utilities
{

    public static class TimeHelper
    {

        public static DateTime currentTime()
        {

            return DateTime.UtcNow.AddMinutes(10);

        }


        public static bool isCodeActive(DateTime time, bool isActive)
        {

            if(time < DateTime.UtcNow && isActive == true)
            {
                return false;
            }

            return true;

        }

    }

}
