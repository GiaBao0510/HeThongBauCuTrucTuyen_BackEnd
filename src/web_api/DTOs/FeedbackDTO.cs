
namespace BackEnd.src.web_api.DTOs
{
    public class FeedbackDTO
    {
        public string? yKien { get; set; } = "null";
        public DateTime ThoiDiem { get; set; }
        public string? UserID { get; set; } = "null";
        public string? HoTen { get; set; } = "null";
    }
}