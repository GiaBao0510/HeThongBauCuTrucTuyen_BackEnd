namespace BackEnd.src.web_api.DTOs
{
    public class ElectionDetailsDTO
    {
        public string? ID_Phieu;
        public string? ID_CuTri { get; set; } ="null";
        public string? ID_ucv { get; set; } ="null";
        public string? ID_CanBo { get; set; } ="null";
        public DateTime ThoiDiem = DateTime.Now;
    }
}