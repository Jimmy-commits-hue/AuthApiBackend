using System.ComponentModel.DataAnnotations;

namespace Web.DTOs
{

    public class RetrieveLoginNumber
    {

        [Required(ErrorMessage ="Field required")]
        [EmailAddress(ErrorMessage ="Invalid email")]
        public string Email { get; set; } = string.Empty;

    }

}
