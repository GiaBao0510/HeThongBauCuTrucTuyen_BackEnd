

namespace BackEnd.src.web_api.DTOs
{
    public class UserInfoDTO
    {
        public string? ID_user{set;get;} = "null";
        public string? HoTen { set; get; } = "null";
        public string? GioiTinh { set; get; } = "1";
        public string? NgaySinh { set; get; }
        public string? DiaChiLienLac { set; get; }
        public string? SDT { get; set; } = "null";
        public string? Email { get; set; } = "null";
        public string? HinhAnh { set; get; } = "null";
        public string? PublicID { set; get; } = "null";
        public int ID_ChucVu { get; set; } = 0;
 
        //Khóa ngoại
        public int ID_DanToc{set; get;} = 1;
        public int RoleID{set; get;} = -1;
    }
}