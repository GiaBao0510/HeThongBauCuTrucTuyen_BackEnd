
namespace BackEnd.src.web_api.DTOs
{
    public class ElectionResultDetailsDTO
    {
        public DateTime ngayBD { get; set; }
        public DateTime ngayKT { get; set; }
        public string? TenKyBauCu { get; set; }
        public string? TenCapUngCu { get; set; }
        public string? TenDonViBauCu { get; set; }
        public string? MoTa { get; set; }
        public string? CongBo { get; set; }
    }
}