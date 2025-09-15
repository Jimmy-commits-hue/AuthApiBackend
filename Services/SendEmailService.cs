using Web.DTOs;
using Web.Interfaces;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using Web.Exceptions;

namespace Web.Services
{
    public class SendEmailService : ISendEmailService
    {

        private readonly EmailConfig config;
        private readonly ILogger<SendEmailService> logger;

        public SendEmailService(IOptions<EmailConfig> config, ILogger<SendEmailService> logger)
        {

            this.config = config.Value;
            this.logger = logger;

        }

        public async Task SendEmailWithCodeAsync(string toEmail, string FirstName, string Surname, Guid codeId)
        {

            int attempt = 0;
            int maxAttempts = 3;

            while (attempt < maxAttempts)
            {

                try
                {

                    attempt++;
                    var email = new MimeMessage();
                    email.From.Add(new MailboxAddress("AuthApi", Environment.GetEnvironmentVariable("FROM_EMAIL")));
                    email.To.Add(MailboxAddress.Parse(toEmail));
                    email.Subject = "AuthApi Verification";

                    var bodyBuilder = new BodyBuilder
                    {

                        HtmlBody = $@"
                                  <html>
                                     <body>
                                        <h1> Welcome to AuthApi, {FirstName} {Surname} </h1><hr>
                                        <p> Please click on <q> Verify Below </q> to verify your email </p>
                                        <p><a href={$"https://localhost:7287/api/User/verifyEmail?codeId={codeId}"}>
                                           Verify Email</a></p>
                                        <hr>
                                        <p> Email verification code will expire after 5 minutes </p>
                                   </html>
                                   "

                    };

                    email.Body = bodyBuilder.ToMessageBody();

                    var client = new SmtpClient(); 
                    await client.ConnectAsync(config.Host, int.Parse(config.Port), MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(Environment.GetEnvironmentVariable("FROM_EMAIL"), Environment.GetEnvironmentVariable("EMAIL_PASSWORD"));
                    await client.SendAsync(email);
                    await client.DisconnectAsync(true);

                }
                catch (Exception ex)
                {

                    logger.LogWarning("Attempt {attempt} failed to send email to {toEmail}. Error: {error}", attempt, toEmail, ex.Message);

                    if (attempt == maxAttempts)
                    {

                        throw new FailedToSendEmailException($"Invalid email : {toEmail}");
                       

                    }

                    await Task.Delay(TimeSpan.FromMinutes(2));
                }

            }

        }

        public async Task SendEmailWithNumberAsync(string toEmail, string FirstName, string Surname, string customNumber)
        {

            int attempt = 0;
            int maxAttempts = 3;

            while(attempt < maxAttempts)
            {

                try
                {

                    attempt++;
                    var email = new MimeMessage();
                    email.From.Add(new MailboxAddress("AuthApi", Environment.GetEnvironmentVariable("FROM_EMAIL")));
                    email.To.Add(MailboxAddress.Parse(toEmail));
                    email.Subject = "AuthApi Verification";

                    var bodyBuilder = new BodyBuilder
                    {

                        HtmlBody = $@"
                 
                                  <html>
                                     <body>
                                        <h1> Welcome to AuthApi, {FirstName} {Surname} </h1>
                                        <p> Login Number : {customNumber} </p>
                    
                                        <p> Congrats, you have been allocated the above login number, <br> 
                                         Please use it when you login or include it in an email as your email subject <br> 
                                         when you have an enquiry</p>
                    
                                        <p> Login Sample </p>
                                        <p> CustomNumber : 213300338 <br>
                                            Password : use the password you created during registration </p>
                      
                                        <p> Thank you, <br> Sincere, <br> Khabana JJ </p>
                                        <br>
                                        <hr>
                                       <footer>
                                           For Enquiries, You can contact me via: <br>
                                           WhatsApp : Click <a href=""https://wa.me/27711626735"" target=""_blank"">WhatsApp me</a><br>
                                           Email :Click <a href=""mailto:khabanajimmy3@gmail.com"">Send Email</a>
                                       </footer>
                                   </html>
                                   "

                    };

                    email.Body = bodyBuilder.ToMessageBody();

                    var client = new SmtpClient(); 
                    await client.ConnectAsync(config.Host, int.Parse(config.Port), MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(Environment.GetEnvironmentVariable("FROM_EMAIL"), Environment.GetEnvironmentVariable("EMAIL_PASSWORD"));
                    await client.SendAsync(email);
                    await client.DisconnectAsync(true);

                }
                catch (Exception ex)
                {

                    logger.LogWarning("Attempt {attempt} failed to send email to {toEmail}. Error: {error}", attempt, toEmail, ex.Message);

                    if (attempt == maxAttempts)
                    {

                        throw new FailedToSendEmailException($"Invalid email : {toEmail}");

                    }

                    await Task.Delay(TimeSpan.FromMinutes(2));

                }

            }

        }
       
    }

}
