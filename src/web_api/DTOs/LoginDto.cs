

using System.ComponentModel.DataAnnotations;

namespace BackEnd.src.web_api.DTOs
{
    public class LoginDto
    {
        [Required]
        public required string account{set; get;}
        [Required]
        public required string password {set; get;}
    }
}