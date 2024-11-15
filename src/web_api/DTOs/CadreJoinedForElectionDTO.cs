namespace BackEnd.src.web_api.DTOs
{
    public class CadreJoinedForElectionDTO
    {
        public string? TenKyBauCu { get; set; } = "null";
        public string? MoTa { get; set; } = "null";
        public string? TenCapUngCu { get; set; } = "null";
        public string? TenDonViBauCu { get; set; } = "null";
        public DateTime ngayBD { get; set;}
        public DateTime ngayKT { get; set;}
        public int SoLuongToiDaCuTri {set; get;} = 0;
        public int SoLuongToiDaUngCuVien {set; get;} = 0;
        public int SoLuotBinhChonToiDa {set; get;} = 0;
        public string CongBo {set; get;} = "0";
        public int ID_DonViBauCu {set; get;} = -1;
        public int ID_Cap {set; get;} = -1;
        public int SoLuongCuTriHienTai {set; get;} = 0;
        public int SoLuongUngCuVienHienTai {set; get;} = 0;
        public int SoLuongCanBoHienTai {set; get;} = 0;
    }
}