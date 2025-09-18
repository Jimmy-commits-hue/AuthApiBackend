using System.ComponentModel.DataAnnotations;

namespace AuthApi.DTOs
{

    public class UpdateDto
    {
             
        public string Name {  get; set; } = string.Empty;

        public string Surname {  get; set; } = string.Empty;

    }

}
