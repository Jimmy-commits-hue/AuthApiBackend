using Web.Exceptions;
using Web.GenerateCustomNumber;
using Web.Interfaces;
using Web.Models;
using Web.Utilities;

namespace Web.Services
{

    public class EmailVerificationService : IEmailService
    {

        public readonly IEmailVerifRepo _repo;

        public EmailVerificationService(IEmailVerifRepo _repo)
        {

            this._repo = _repo;

        }

        public async Task<Guid> CreateCodeAsync(Guid userID)
        {

            string code = GenerateVerificationCode.VerificationCode();

            var email = new EmailVerification
            {

                userId = userID,
                ExpiresAt = TimeHelper.currentTime(),
                currentAttemptNumber = 1,
                verificationCode = code

            };

            await _repo.CreateCodeAsync(email);

            return email.Id;

        }

        public async Task<EmailVerification> GetCodeAsync(Guid codeId)
        {

            var emailObj = await _repo.GetCodeAsync(codeId);

            if (emailObj == null)
                throw new DailyAttemptsReachedException("Daily attempts for verification code request reached," +
                                                        "please try again tomorrow");

            return emailObj;

        }

        public async Task<Guid> ReCreateCodeAsync(Guid userId)
        {

            int numberOfAttempts = await _repo.GetMinNumberOfAttempts(userId);
            int maxNumberOfAttempts = await _repo.GetMaxNumberOfAttempts(userId);

            if (numberOfAttempts == maxNumberOfAttempts)
               throw new DailyAttemptsReachedException("Daily attempts reached, please try again tomorrow");

            int currentAttempt = numberOfAttempts + 1;

            string code = GenerateVerificationCode.VerificationCode();

            var email = new EmailVerification
            {

                userId = userId,
                ExpiresAt = DateTime.UtcNow,
                currentAttemptNumber = currentAttempt,
                verificationCode = code

            };

            await _repo.CreateCodeAsync(email);

            return email.Id;

        }

        public async Task UpdateCodeAsync(EmailVerification email)
        {

            email.codeStatus = false;

            await _repo.UpdateCodeAsync(email);

        }

    }

}