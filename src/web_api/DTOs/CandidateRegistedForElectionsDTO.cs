namespace BackEnd.src.web_api.DTOs
{
    public class CandidateRegistedForElectionsDTO
    {
        public string? TenKyBauCu { get; set; } = "null";
        public string? MoTa { get; set; } = "null";
        public string? TenCapUngCu { get; set; } = "null";
        public string? TenDonViBauCu { get; set; } = "null";
        public DateTime ngayBD { get; set;}
        public DateTime ngayKT { get; set;}
    }
}