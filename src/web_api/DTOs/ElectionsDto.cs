
namespace BackEnd.src.web_api.DTOs
{
    public class ElectionsDto
    {
        public DateTime ngayBD { get; set; }
        public DateTime ngayKt { get; set; }
        public string? TenKyBauCu { get; set; } = "null";
        public string? Mota { get; set; } = "null";
        public int? ID_DonViBauCu { get; set; } = 0;
        public int? ID_Cap { get; set; } = 0;
        
        public string? GhiNhan {get; set;}
        public string? TenDonViBauCu {get; set;}
        public int? SoLuongToiDaCuTri{set; get;} = 0;
        public int? SoLuongToiDaUngCuVien{set; get;} = 0;
        public int? SoLuotBinhChonToiDa{set; get;} = 0;
    }
}