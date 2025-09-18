using System.ComponentModel.DataAnnotations;

namespace AuthApi.DTOs
{

    public class UpdateEmailDto
    {

        [Required(ErrorMessage ="Field required")]
        [StringLength(9, MinimumLength = 9)]
        public string customNumber {  get; set; } = string.Empty;

        [Required(ErrorMessage ="Field required")]
        [DataType(DataType.Password)]
        public string password { get; set; } = string.Empty;

        [Required(ErrorMessage="Field required")]
        [EmailAddress(ErrorMessage ="Invalid Email")]
        public string newEmailAddress { get; set; } = string.Empty;

        [Required]
        [Compare("newEmailAddress", ErrorMessage = "Email mismatch")]
        public string confirmEmail { get; set; } = string.Empty;

    }

}
