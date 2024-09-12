

namespace BackEnd.src.web_api.DTOs
{
    public class ProfileDto
    {
        public int? MaSo{get; set;}
        public string TrangThaiDangKy{set; get;} = "0";

        public string? ID_user { get; set; }
        public string? HoTen { get; set; }
    }
}