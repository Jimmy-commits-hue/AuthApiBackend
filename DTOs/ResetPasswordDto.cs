using System.ComponentModel.DataAnnotations;

namespace AuthApi.DTOs
{

    public class ResetPasswordDto
    {

        [Required]
        public string customNumber { get; set; } = string.Empty;

    }

}
