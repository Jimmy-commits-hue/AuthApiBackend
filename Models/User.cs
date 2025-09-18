using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models
{

    public class User
    {

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [RegularExpression(@"^\d{13}$")]
        public string IdNumber {  get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string Surname { get; set; } = string.Empty;

        [RegularExpression(@"^\d{9}$")]
        public string customNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[@!&#?])(?=.*[1-9]).{8}$")]
        public string Password { get; set; } = string.Empty;

        public DateOnly RegistrationDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public bool isVerified { get; set; } = false;

        public bool sentLoginNumber { get; set; } = false;

        public List<EmailVerification> EmailVerification { get; set; } = new();

        public List<LoginAttempts> LoginAttempts { get; set; } = new();

        public List<RefreshToken> RefreshToken { get; set; } = new();

    }

}
