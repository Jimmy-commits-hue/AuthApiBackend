using System.ComponentModel.DataAnnotations;

namespace Web.Models
{

    public class EmailVerification
    {
         
        [Key]
        public Guid Id { get; set; }  = Guid.NewGuid();

        [Required]
        public Guid userId { get; set; } 

        public User user { get; set; } = null!;

        [Required]
        public string? verificationCode {  get; set; }

        [Required]
        public DateTime? ExpiresAt { get; set; }

        public int currentAttemptNumber { get; set; }

        public int maxNumberOfAttempts { get; set; } = 3;

        public bool codeStatus { get; set; } = true;
        
    }

}
