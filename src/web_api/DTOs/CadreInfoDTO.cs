namespace BackEnd.src.web_api.DTOs
{
    public class CadreInfoDTO: UserInfoDTO
    {
        public string? ID_CanBo { get; set; } = "null";
        public DateTime? NgayCongTac { get; set; } = DateTime.Now;
        public string? GhiChu { get; set; } = "null";
    }
}