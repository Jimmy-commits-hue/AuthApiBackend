using Microsoft.EntityFrameworkCore;
using AuthApi.Database;

namespace AuthApi.BackgroudService
{

    public class CleanUpUnActiveUsers : BackgroundService
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan delay = TimeSpan.FromHours(24);

        public CleanUpUnActiveUsers(IServiceProvider serviceProvider)
        {
            
            _serviceProvider = serviceProvider;

        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while(stoppingToken.IsCancellationRequested)
            {

                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AuthApiDbContext>();

                var notActiveUsers = await db.User.Where(u => u.isVerified == false &&
                                     u.RegistrationDate >= DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-3))
                                     .ToListAsync(stoppingToken);

                if (notActiveUsers.Any())
                {

                    db.User.RemoveRange(notActiveUsers);
                    await db.SaveChangesAsync(stoppingToken);

                }

                await Task.Delay(delay, stoppingToken);

            }

        }

    }

}
