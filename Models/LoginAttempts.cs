using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{

    public class LoginAttempts
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public Guid userId {  get; set; }

        public User user { get; set; } = null!;

        public int currentAttemptNumber { get; set; }

        public int maxNumberOfAttempts { get; set; } = 3;

        public bool isUserActive { get; set; } = false;

    }

}
