namespace AuthApi.Interfaces
{

    public interface ISendEmailService
    {

        Task SendEmailWithCodeAsync(string toEmail, string FirstName, string Surname, string code);

        Task SendEmailWithNumberAsync(string toEmail, string FirstName, string Surname, string customNumber);

    }

}
