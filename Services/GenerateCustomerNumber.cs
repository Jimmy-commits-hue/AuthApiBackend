using AuthApi.Interfaces;

namespace AuthApi.Services
{
    /// <summary>
    /// Class to generate customer numbers in the format YYYY00001, YYYY00002, etc.
    /// </summary>
    public class GenerateCustomerNumber : ICustomNumberService
    {

        private readonly ICustomNumberRepo cust;

        public GenerateCustomerNumber(ICustomNumberRepo cust) => this.cust = cust; 

        /// <summary>
        /// Generates a new custom number based on the current year and a sequential value. 
        /// </summary>
        /// <remarks>The generated number consists of the current year followed by a zero-padded sequence
        /// number. If a previous custom number exists, the sequence is incremented from the numeric part of that
        /// number. Otherwise, the sequence starts at 1.</remarks>
        /// <returns>A string representing the new custom number in the format "YYYY#####", where "YYYY" is the current year and
        /// "#####" is a zero-padded sequence number.</returns>
        public async Task<string> GetNumber()
        {

            string? lastNumber = await cust.GetLastCustomNumber();

            int sequence = 1;

            if (!string.IsNullOrEmpty(lastNumber))
            {
                             
                string numericPart = lastNumber.Substring(DateTime.Now.Year.ToString().Length);
                sequence = int.Parse(numericPart) + 1;

            }
  
            return $"{DateTime.Now.Year.ToString()}{sequence:D5}";

        }

    }
    
}