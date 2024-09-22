
namespace BackEnd.src.web_api.DTOs
{
    public class ElectionsDto
    {
        public DateTime ngayBD { get; set; }
        public DateTime ngayKt { get; set; }
        public string? TenKyBauCu { get; set; } = "null";
        public string? Mota { get; set; } = "null";
    }
}