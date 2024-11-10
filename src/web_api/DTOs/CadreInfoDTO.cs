namespace BackEnd.src.web_api.DTOs
{
    public class CadreInfoDTO: UserInfoDTO
    {
        public DateTime? NgayCongTac { get; set; } = DateTime.Now;
        public string? GhiChu { get; set; } = "null";
    }
}