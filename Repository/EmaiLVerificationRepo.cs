using Web.Database;
using Web.Interfaces;
using Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Web.Repository
{

    public class EmaiLVerificationRepo : IEmailVerifRepo
    {

        public readonly AuthApiDbContext context;

        public EmaiLVerificationRepo(AuthApiDbContext context)
        {

            this.context = context;

        }

        public async Task CreateCodeAsync(EmailVerification emailVerification)
        {

            context.EmailVerification.Add(emailVerification);
            await context.SaveChangesAsync();

        }

        public async Task<EmailVerification?> GetCodeAsync(Guid Id)
        {

            return await context.EmailVerification.FindAsync(Id);

        }

        public async Task<int> GetMinNumberOfAttempts(Guid userId)
        {

            return await context.EmailVerification.Where(u => u.userId == userId).
                                   OrderByDescending(u => u.currentAttemptNumber).Select(u => u.currentAttemptNumber)
                                   .FirstOrDefaultAsync();

        }

        public async Task<int> GetMaxNumberOfAttempts(Guid userId)
        {

            return await context.EmailVerification.Where(u => u.userId ==userId).
                         OrderByDescending(u => u.currentAttemptNumber).Select(u => u.maxNumberOfAttempts)
                         .FirstOrDefaultAsync();

        }

        public async Task UpdateCodeAsync(EmailVerification emailverification)
        {

            context.EmailVerification.Update(emailverification);
            await context.SaveChangesAsync();

        }

    }

}