namespace AuthApi.GenerateCustomNumber
{

    public static class GenerateVerificationCode
    {

        private static readonly Random number = new Random(); 

        public static string VerificationCode() 
        {

            string code = string.Empty;

            for(int i = 0; i < 6; i++)
            {

                code += number.Next(0, 9);

            }
            
            return code;

        }

    }

}
