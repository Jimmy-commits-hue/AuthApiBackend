using AuthApi.Models;

namespace AuthApi.Interfaces
{

    public interface IEmailVerifRepo
    {

        Task CreateCodeAsync(EmailVerification emailverification);

        Task<int> GetMaxNumberOfAttempts(Guid userId);

        Task<int> GetMinNumberOfAttempts(Guid userId);

        Task<EmailVerification?> GetCodeAsync(string code);

        Task UpdateCodeAsync(EmailVerification emailverification);
            
    }

}
