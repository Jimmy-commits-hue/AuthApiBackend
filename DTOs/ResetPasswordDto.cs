using System.ComponentModel.DataAnnotations;

namespace Web.DTOs
{

    public class ResetPasswordDto
    {

        [Required]
        public string customNumber { get; set; } = string.Empty;

    }

}
