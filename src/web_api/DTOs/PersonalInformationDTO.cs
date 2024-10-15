namespace BackEnd.src.web_api.DTOs
{
    public class PersonalInformationDTO
    {
        public string? HoTen{ get; set; } = "null";
        public string? GioiTinh{ get; set; }
        public DateTime NgaySinh{ get; set; }
        public string? DiaChiLienLac{ get; set; } = "null";
        public string? Email{ get; set; } = "null";
        public string? SDT{ get; set; } = "null";
        public string? old_SDT{ get; set; } = "null";
        public string? HinhAnh{ get; set; } = "null";
        public string? TenDanToc{ get; set; } = "null";
        public string? ID_Object{ get; set; } = "null";
        public string? ID_user{ get; set; } = "null";
        public string? ID_DanToc{ get; set; } = "null";
    }
}