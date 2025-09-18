using System.ComponentModel.DataAnnotations;

namespace AuthApi.DTOs
{

    public class LoginDtos
    {

        [Required(ErrorMessage ="Field required")]
        [RegularExpression(@"\d{9}")]
        public string customNumber {  get; set; } = string.Empty;

        [Required(ErrorMessage = "Field required")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[@!&?])(?=.*[1-9]).{8}$")]
        public string password { get; set; } = string.Empty;

    }

}
