
using System.ComponentModel.DataAnnotations;

namespace BackEnd.src.core.Models
{
    public class LoginModel
    {
        [Required]
        public string account{set; get;}
        [Required]
        public string password {set; get;}
        public string? Email{set; get;} = "null";
        public string? Role{set; get;}  = "null";
        public string? BiKhoa{set; get;} = "null";
        public int SuDung{set; get;} = 1;
        public bool GhiNhoDangNhap {set;get;} = false;
    }
}