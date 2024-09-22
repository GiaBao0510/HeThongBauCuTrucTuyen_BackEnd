
namespace BackEnd.src.web_api.DTOs
{
    public class VoterListInElectionDto
    {
        public DateTime ngayBD{set; get;}
        public string? GhiNhan{get; set;} = "0";
        public string? ID_DonViBauCu{set; get;}
        public List<string> listIDVoter{set; get;}
    }
}