namespace BackEnd.src.web_api.DTOs
{
    public class WorkPlaceDto
    {
        public int ID_ChucVu { get; set; }
        public int ID_Ban { get; set; }
        public string? ID_canbo{set; get;}
        public string? TenBan{set; get;}
        public string? HoTen{set; get;}
        public string? TenChucVu{set; get;}
        public DateTime ngayBD { get; set; }
    }
}