namespace BackEnd.src.web_api.DTOs
{
    public class ElectionResponseDTO
    {
        public string? ID_CuTri { get; set; } = "null";
        public string? NoiDung { get; set; } = "null";
        public DateTime? ngayBD { get; set; }
        public DateTime? ThoiDiem { get; set; }
    }
}