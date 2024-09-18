using System.ComponentModel.DataAnnotations;

namespace BackEnd.src.web_api.DTOs
{
    public class VerifyOtpDto
    {
        [EmailAddress]
        public string Email{get; set;}
        public string Otp{get; set;}
    }
}