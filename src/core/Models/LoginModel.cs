
using System.ComponentModel.DataAnnotations;

namespace BackEnd.src.core.Models
{
    public class LoginModel
    {
        [Required]
        public string account{set; get;}
        [Required]
        public string password {set; get;}
        public string? Role{set; get; } 
        public string? BiKhoa{set; get;}
        public int SuDung{set; get;} = 1;
        public bool GhiNhoDangNhap {set;get;} = false;
    }
}