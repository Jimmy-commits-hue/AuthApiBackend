using Web.Models;

namespace Web.Interfaces
{

    public interface IEmailService
    {

        Task<Guid> CreateCodeAsync(Guid userid);

        Task<Guid> ReCreateCodeAsync(Guid userId);

        Task<EmailVerification> GetCodeAsync(Guid id);

        Task UpdateCodeAsync(EmailVerification verification);

    }

}
