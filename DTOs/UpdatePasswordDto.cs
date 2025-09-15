using System.ComponentModel.DataAnnotations;

namespace Web.DTOs
{

    public class UpdatePasswordDto
    {

        [Required(ErrorMessage ="Field required")]
        [StringLength(9, MinimumLength =9)]
        public string customNumber {  get; set; } = string.Empty;

        [Required(ErrorMessage = "Field required")]
        [DataType(DataType.Password)]
        [StringLength(8, MinimumLength =8)]
        public string oldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Field required")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[@!&?])(?=.*[1-9]).{8}$",
            ErrorMessage = "Password should contains atleast:" +
                            "one special character (@!&?) and should be 8 characters long, must not have a 0")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password mismatch")]
        public string newPassword { get; set; } = string.Empty;

    }

}
