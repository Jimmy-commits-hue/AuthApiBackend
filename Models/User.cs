using System.ComponentModel.DataAnnotations;

namespace Web.Models
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

        public bool isVerified { get; set; } = false;

        public List<EmailVerification> emailVerification { get; set; } = new();

        public List<LoginAttempts> loginAttempts { get; set; } = new();

        public List<RefreshToken> refreshToken { get; set; } = new();

    }

}
