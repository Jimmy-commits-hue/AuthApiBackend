using AuthApi.Database;
using AuthApi.Exceptions;
using AuthApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.BackgroudService
{

    public class SendVerificationEmails : BackgroundService
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan delay = TimeSpan.FromMinutes(1);

        public SendVerificationEmails(IServiceProvider serviceProvider)
        {
         
            _serviceProvider = serviceProvider;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {

                Console.WriteLine("SendVerificationEmails running at: {0}", DateTimeOffset.Now);

                using var scope = _serviceProvider.CreateScope();

                var emailService = scope.ServiceProvider.GetService<IEmailService>();

                var userService = scope.ServiceProvider.GetService<IUserService>();

                var sendEmailService = scope.ServiceProvider.GetRequiredService<ISendEmailService>();

                var db = scope.ServiceProvider.GetRequiredService<AuthApiDbContext>();

                var pendingEmails = await db.EmailVerification.Include(ev => ev.User).
                                    Where(ev => ev.EmailSent == false).ToListAsync(stoppingToken);
                
                if (pendingEmails.Any())
                {

                    foreach(var info in pendingEmails)
                    {

                        try
                        {

                            await sendEmailService.SendEmailWithCodeAsync(info.User.Email, info.User.FirstName,
                                                                      info.User.Surname, info.verificationCode!);

                            await emailService!.IsEmailSent(info);

                        }
                        catch (FailedToSendEmailException)
                        {

                            continue;

                        }catch(Exception  ex)
                        {

                            continue;

                        }
                        
                    }

                }

                var pendingLoginNumberEmails = await db.User.Where(u => u.sentLoginNumber == false &&
                                               u.customNumber != string.Empty).ToListAsync(stoppingToken);

                if (pendingLoginNumberEmails.Any())
                {
                    foreach(var info in pendingLoginNumberEmails)
                    {

                        try
                        {

                            await sendEmailService.SendEmailWithNumberAsync(info.Email, info.FirstName, info.Surname,
                                                                            info.customNumber);

                            await userService!.IsLoginEmailSent(info);

                        }
                        catch (Exception)
                        {
                            continue;
                        }

                    }

                }
                
                Console.WriteLine("Waiting for next iteration");

                await Task.Delay(delay, stoppingToken);

            }

        }

    }

}