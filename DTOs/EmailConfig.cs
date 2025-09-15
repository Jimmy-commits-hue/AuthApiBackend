using System.ComponentModel.DataAnnotations;

namespace Web.DTOs
{

    public class EmailConfig
    {

        [Required]
        public string Host { get; set; } = string.Empty;

        [Required]
        public string Port {  get; set; } = string.Empty;

        [Required]
        public string Security {  get; set; } = string.Empty;

    }

}
