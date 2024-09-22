using Microsoft.AspNetCore.Mvc;

namespace BackEnd.src.web_api.DTOs
{
    public class UserDto
    {
        public string? ID_user{set;get;}
        [FromForm(Name ="HoTen")]
        public string? HoTen { set; get; }
        [FromForm(Name ="GioiTinh")]
        public string? GioiTinh { set; get; } = "1";
        [FromForm(Name ="NgaySinh")]
        public string? NgaySinh { set; get; }
        [FromForm(Name ="DiaChiLienLac")]
        public string? DiaChiLienLac { set; get; }
        [FromForm(Name ="CCCD")]
        public string? CCCD { set; get; }
        [FromForm(Name ="SDT")]
        public string? SDT { get; set; }
        [FromForm(Name ="Email")]
        public string? Email { get; set; }
        public string? HinhAnh { set; get; }
        public string? PublicID { set; get; }
        [FromForm(Name ="TaiKhoan")]
        public string? TaiKhoan { get; set; }
        [FromForm(Name ="MatKhau")]
        public string? MatKhau { get; set; }
        [FromForm(Name ="BiKhoa")]
        public string BiKhoa { get; set; } = "0";
        [FromForm(Name ="LyDoKhoa")]
        public string? LyDoKhoa { get; set; }
        [FromForm(Name ="NgayTao")]
        public string? NgayTao { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); 
        [FromForm(Name ="SuDung")]
        public int SuDung { get; set; } = 1;
        [FromForm(Name ="ID_ChucVu")]
        public int ID_ChucVu { get; set; } = 0;
 
        //Khóa ngoại
        [FromForm(Name ="ID_DanToc")]
        public int ID_DanToc{set; get;}
        [FromForm(Name ="RoleID")]
        public int RoleID{set; get;}

    }
}