namespace BackEnd.src.web_api.DTOs
{
    public class CandidateDto : UserDto
    {
        public string? ID_ucv{set;get;}
        public string? TrangThai{set;get;} = "acctive";
    }
}