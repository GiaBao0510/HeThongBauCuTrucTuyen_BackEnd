namespace BackEnd.src.web_api.DTOs
{
    public class CandidateInfoDTO: UserInfoDTO
    {
        public string? ID_ucv { get; set; } = "null"; 
        public string? TrangThai { get; set; } = "null";
        public string? GioiThieu { get; set; } = "null";
    }
}