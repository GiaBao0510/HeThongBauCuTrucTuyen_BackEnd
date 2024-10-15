namespace BackEnd.src.web_api.DTOs
{
    public class ListCandidateOnElectionDateDTO
    {
        public string? HoTen { get; set; }
        public string? GioiTinh { get; set; }
        public DateTime NgaySinh { get; set; }
        public string? Email { get; set; } = "null";
        public string? HinhAnh { get; set; } = "null";
        public string? TrangThai {get; set;}
        public string? GioiThieu{set;get;} = "null";
        public string? TenDanToc {get; set;}
        public int SoLuotBinhChon {get; set;} = 0;
        public int TyLeBinhChon {get; set;} = 0;
        public string TenTrinhDoHocVan{ get; set; } = "null";
    }
}