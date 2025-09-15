using Web.Models;

namespace Web.Interfaces
{

    public interface IEmailVerifRepo
    {

        Task CreateCodeAsync(EmailVerification emailverification);

        Task<int> GetMaxNumberOfAttempts(Guid userId);

        Task<int> GetMinNumberOfAttempts(Guid userId);

        Task<EmailVerification?> GetCodeAsync(Guid codeId);

        Task UpdateCodeAsync(EmailVerification emailverification);
            
    }

}
