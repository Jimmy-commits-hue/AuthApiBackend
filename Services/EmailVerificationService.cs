using AuthApi.Exceptions.ExceptionsTypes;
using AuthApi.GenerateCustomNumber;
using AuthApi.Interfaces;
using AuthApi.Models;
using AuthApi.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Services
{
    /// <summary>
    /// Email verification service to handle email verification code creation, retrieval, and updates.
    /// </summary>
    public class EmailVerificationService : IEmailService
    {

        public readonly IEmailVerifRepo _repo;

        public EmailVerificationService(IEmailVerifRepo _repo)
        {

            this._repo = _repo;

        }

        /// <summary>
        /// Method to create a new email verification code for a user.
        /// </summary>
        /// <param name="userID">user primary key</param>
        /// 
        /// <remarks>
        /// <para>User primary key is used to associate the code with the user</para>
        /// <para>The method attempts to create a unique verification code up to 3 times in case of collisions</para>
        /// <para>
        /// Catches DbUpdateException to handle potential database update issues that may arise from code collisions
        /// </para>
        /// <para>If all attempts fail, a FailedToRandomizeCodeException is thrown</para>
        /// </remarks>
        /// <returns>verification code</returns>
        /// <exception cref="FailedToRandomizeCodeException"></exception>
        public async Task<string> CreateCodeAsync(Guid userID)
        {

            int maxAttempts = 3;
            int minAttempts = 0;

            string code = GenerateVerificationCode.VerificationCode();

            var email = new EmailVerification
            {

                userId = userID,
                ExpiresAt = TimeHelper.currentTime(),
                currentAttemptNumber = 1,
                verificationCode = code

            };

            while (minAttempts < maxAttempts)
            {

                try
                {

                    minAttempts++;

                    await _repo.CreateCodeAsync(email);
                    break;

                }
                catch (DbUpdateException)
                {

                    if(minAttempts == maxAttempts)
                        throw new FailedToRandomizeCodeException("Failed to generate verification code");

                    //reattempt to generate a new code
                    email.verificationCode = GenerateVerificationCode.VerificationCode();

                    //wait 50 milliseconds before retrying to avoid rapid collisions
                    await Task.Delay(50);

                }

            }

            return email.verificationCode;

        }


        /// <summary>
        /// Retrieves an email verification object based on the provided code.
        /// </summary>
        /// <param name="code">verification code (unique)</param>
        /// 
        /// <remarks>
        /// If no matching email verification object is found, a DailyAttemptsReachedException is thrown
        /// 
        /// <para>
        /// <example>
        /// If the user has exceeded the allowed daily attempts for verification code requests, the code will
        /// try to retrieve a code that is associated with the user and still active. If no such code exists,
        /// then the exception is thrown.
        /// </example>
        /// </para>
        /// 
        /// </remarks>
        /// <returns> email verification object</returns>
        /// <exception cref="DailyAttemptsReachedException">thrown if user failed to verify themselves 
        /// within the allowed daily attempts
        /// </exception>
        public async Task<EmailVerification> GetCodeAsync(string code)
        {

            var emailObj = await _repo.GetCodeAsync(code);

            if (emailObj == null)
                throw new DailyAttemptsReachedException("Daily attempts for verification code request reached," +
                                                        "please try again tomorrow");

            return emailObj;

        }

        /// <summary>
        /// It is used to re-create a new verification code for a user if they have not exceeded their daily attempt limit.
        /// and the old one is no longer valid.
        /// </summary>
        /// <remarks>
        /// Similar to <see cref="CreateCodeAsync(Guid)"/>, this method attempts to create a unique verification 
        /// code up to 3 times
        /// </remarks>
        /// <param name="userId">Primary key of the user whom the code will be associated with</param>
        /// <returns>a new verification code</returns>
        /// <exception cref="DailyAttemptsReachedException"></exception>
        public async Task<string> ReCreateCodeAsync(Guid userId)
        {
            //get current number of attempts and max number of attempts
            int numberOfAttempts = await _repo.GetMinNumberOfAttempts(userId);
            int maxNumberOfAttempts = await _repo.GetMaxNumberOfAttempts(userId);

            //check if user has exceeded their daily attempts
            if (numberOfAttempts == maxNumberOfAttempts)
               throw new DailyAttemptsReachedException("Daily attempts reached, please try again tomorrow");

            //increment attempt number
            int currentAttempt = numberOfAttempts + 1;

            //generate new code
            string code = GenerateVerificationCode.VerificationCode();

            //create new email verification object
            var email = new EmailVerification
            {

                userId = userId,
                ExpiresAt = DateTime.UtcNow,
                currentAttemptNumber = currentAttempt,
                verificationCode = code

            };

            int maxAttempts = 3;
            int minAttempts = 0;

            while (minAttempts < maxAttempts)
            {
                try
                {
                    minAttempts++;
                    await _repo.CreateCodeAsync(email);
                    break;

                }
                catch (DbUpdateException)
                {

                    if (minAttempts == maxAttempts)
                        throw new FailedToRandomizeCodeException("Failed to generate verification code");

                    //reattempt to generate a new code
                    email.verificationCode = GenerateVerificationCode.VerificationCode();

                    //wait 50 milliseconds before retrying to avoid rapid collisions
                    await Task.Delay(50);

                }

            }

            return email.verificationCode;

        }

        /// <summary>
        /// It is used to mark an email verification code as sent.
        /// </summary>
        /// <param name="email"> EmailVerication object to update. The <see cref="EmailVerification.EmailSent"/>
        /// will be set to <see langword="true"/> to show that the email has been sent.
        /// </param>
        public async Task IsEmailSent(EmailVerification email)
        {

            //mark email as sent
            email.EmailSent = true;

            //update email status in the database
            await _repo.UpdateCodeAsync(email);

        }

        /// <summary>
        /// Updates the specified email verification record by deactivating it.
        /// </summary>
        /// <param name="email">The email verification record to update. The <see cref="EmailVerification.isActive"/> property will be set
        /// to <see langword="false"/> to deactive the code</param>
        public async Task UpdateCodeAsync(EmailVerification email)
        {

            email.isActive = false;

            await _repo.UpdateCodeAsync(email);

        }

       

    }

}