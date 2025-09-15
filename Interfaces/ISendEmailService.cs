namespace Web.Interfaces
{

    public interface ISendEmailService
    {

        Task SendEmailWithCodeAsync(string toEmail, string FirstName, string Surname, Guid codeId);

        Task SendEmailWithNumberAsync(string toEmail, string FirstName, string Surname, string customNumber);

    }

}
