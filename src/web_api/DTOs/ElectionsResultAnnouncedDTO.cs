

namespace BackEnd.src.web_api.DTOs
{
    public class ElectionsResultAnnouncedDTO
    {
        public string? ID_ucv { get; set; } = "null";
        public string? HoTen { get; set; } = "null";
        public string? Email { get; set; } = "null";
        public int SoLuotBinhChon { get; set; } = -1;
        public float TyLeBinhChon { get; set; } = -1f;
        public int ID_Cap { get; set; } = -1;
        public string? TenCapUngCu { get; set; } = "null";
        public DateTime ThoiDiemDangKy = DateTime.Now;
        public DateTime ngayBD = DateTime.Now;
    }
}