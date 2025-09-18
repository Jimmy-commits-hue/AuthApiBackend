using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models
{

    public class RefreshToken
    {

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid userId { get; set; }

        public User User { get; set; } = null!;

        [Required]
        public string token {  get; set; } = string.Empty;

        [Required]
        public DateTime? expires { get; set; }

        public bool isActive { get; set; } = true;

    }

}
