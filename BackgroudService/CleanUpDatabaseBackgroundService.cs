using Web.Database;
using Microsoft.EntityFrameworkCore;

namespace Web.BackgroudService
{

    public class CleanUpDatabaseBackgroundService : BackgroundService
    {

        private readonly ILogger<CleanUpDatabaseBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan delay = TimeSpan.FromDays(1);

        public CleanUpDatabaseBackgroundService(ILogger<CleanUpDatabaseBackgroundService> _logger, IServiceProvider serviceProvider)
        {
            
            this._logger = _logger;
            this._serviceProvider = serviceProvider;

        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            while (!cancellationToken.IsCancellationRequested)
            {

                using var scope = _serviceProvider.CreateScope();
                
                var db = scope.ServiceProvider.GetRequiredService<AuthApiDbContext>();

                var cutOfftimer = TimeSpan.FromHours(-24);

                var notActiveUsers = await db.User.Where(u => u.isVerified == false).ToListAsync(cancellationToken);

                if(notActiveUsers.Any())
                {

                    db.User.RemoveRange(notActiveUsers);
                    await db.SaveChangesAsync();

                }

                var activeUserVerificationCodes = await db.EmailVerification.Where(u => u.codeStatus == false).ToListAsync(cancellationToken);

                var oldTokens = await db.RefreshToken.Where(u => u.isActive == false).ToListAsync(cancellationToken);

                var reachedDailyLoginAttempts = await db.LoginAttempt.Where(u => u.isUserActive == false)
                                                .ToListAsync(cancellationToken);

                if (reachedDailyLoginAttempts.Any())
                    db.LoginAttempt.RemoveRange(reachedDailyLoginAttempts);

                if (activeUserVerificationCodes.Any())
                    db.EmailVerification.RemoveRange(activeUserVerificationCodes);

                if (oldTokens.Any())
                    db.RefreshToken.RemoveRange(oldTokens);
                
                await db.SaveChangesAsync();

                await Task.Delay(delay, cancellationToken);

            }

        }

    }

}