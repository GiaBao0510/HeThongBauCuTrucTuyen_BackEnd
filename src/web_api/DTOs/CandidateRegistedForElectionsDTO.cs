namespace BackEnd.src.web_api.DTOs
{
    public class CandidateRegistedForElectionsDTO
    {
        public string? TenKyBauCu { get; set; } = "null";
        public string? MoTa { get; set; } = "null";
        public string? TenCapUngCu { get; set; } = "null";
        public string? TenDonViBauCu { get; set; } = "null";
        public string GhiNhan {set; get;} = "0";
        public int ID_Cap { get; set; } = -1;
        public int ID_DonViBauCu { get; set; } = -1;
        public int SoLuongToiDaCuTri { get; set; } = -1;
        public int SoLuotBinhChonToiDa { get; set; } = -1;
        public int SoLuongToiDaUngCuVien { get; set; } = -1;
        public DateTime ngayBD { get; set;}
        public DateTime ngayKT { get; set;}
    }
}