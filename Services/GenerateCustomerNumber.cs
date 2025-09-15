using Web.Interfaces;

namespace Web.Services
{

    public class GenerateCustomerNumber : ICustomNumberService
    {

        private readonly ICustomNumberRepo cust;

        public GenerateCustomerNumber(ICustomNumberRepo cust) => this.cust = cust; 

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