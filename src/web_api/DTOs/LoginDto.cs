

using System.ComponentModel.DataAnnotations;

namespace BackEnd.src.web_api.DTOs
{
    public class LoginDto
    {
        [Required]
        public string account{set; get;}
        [Required]
        public string password {set; get;}
        public string? Role{set; get; }
    }
}