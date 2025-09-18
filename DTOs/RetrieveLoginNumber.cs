using System.ComponentModel.DataAnnotations;

namespace AuthApi.DTOs
{

    public class RetrieveLoginNumber
    {

        [Required(ErrorMessage ="Field required")]
        [EmailAddress(ErrorMessage ="Invalid email")]
        public string Email { get; set; } = string.Empty;

    }

}
