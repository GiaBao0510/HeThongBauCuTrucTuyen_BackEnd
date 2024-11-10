

namespace BackEnd.src.web_api.DTOs
{
    public class UserInfoDTO
    {
        public string? ID_user{set;get;}
        public string? HoTen { set; get; }
        public string? GioiTinh { set; get; } = "1";
        public string? NgaySinh { set; get; }
        public string? DiaChiLienLac { set; get; }
        public string? SDT { get; set; }
        public string? Email { get; set; }
        public string? HinhAnh { set; get; }
        public string? PublicID { set; get; }
        public int ID_ChucVu { get; set; } = 0;
 
        //Khóa ngoại
        public int ID_DanToc{set; get;} = 1;
        public int RoleID{set; get;} = -1;
    }
}