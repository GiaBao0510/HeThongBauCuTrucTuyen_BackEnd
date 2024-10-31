namespace BackEnd.src.web_api.DTOs
{
    public class ElectionTempDTO
    {
        public DateTime ngayBD { get; set; }
        public DateTime ngayKT { get; set; }
        public DateTime NgayKT_UngCu { get; set; } = DateTime.Now.AddDays(5);
        public string? TenKyBauCu {get; set;} = "null";
        public string? MoTa { get; set; } = "null";
        public int SoLuongToiDaCuTri { get; set; } = 0;
        public int SoLuongToiDaUngCuVien { get; set; } = 0;
        public int SoLuotBinhChonToiDa { get; set; } = 0;

        //KHóa ngoại
        public int ID_Cap { get; set; }
    }
}