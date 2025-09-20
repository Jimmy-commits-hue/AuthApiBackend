using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models
{

    public class User
    {

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string IdNumber {  get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string Surname { get; set; } = string.Empty;

        public string customNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public DateOnly RegistrationDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public bool isVerified { get; set; } = false;

        public bool sentLoginNumber { get; set; } = false;

        public List<EmailVerification> EmailVerification { get; set; } = new();

        public List<LoginAttempts> LoginAttempts { get; set; } = new();

        public List<RefreshToken> RefreshToken { get; set; } = new();

    }

}
