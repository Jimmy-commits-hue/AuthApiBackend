using AuthApi.Models;

namespace AuthApi.Interfaces
{

    public interface IEmailService
    {

        Task<string> CreateCodeAsync(Guid userid);

        Task<string> ReCreateCodeAsync(Guid userId);

        Task<EmailVerification> GetCodeAsync(string code);

        Task IsEmailSent(EmailVerification email);

        Task UpdateCodeAsync(EmailVerification verification);

    }

}
