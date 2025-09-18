using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models
{

    [Index(nameof(verificationCode), IsUnique = true)]
    public class EmailVerification
    {
         
        [Key]
        public Guid Id { get; set; }  = Guid.NewGuid();

        [Required]
        public Guid userId { get; set; }

        public User User { get; set; } = null!;

        [Required]
        public string? verificationCode {  get; set; }

        [Required]
        public DateTime? ExpiresAt { get; set; }

        public bool EmailSent { get; set; } = false;

        public int currentAttemptNumber { get; set; }

        public int maxNumberOfAttempts { get; set; } = 3;

        public bool isActive { get; set; } = true;
        
    }

}
