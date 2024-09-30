
namespace BackEnd.src.web_api.DTOs
{
    public class CandidateListInElectionDto
    {
        public DateTime ThoiDiemDangKy{set; get;} = DateTime.Now;
        public int SoLuotBinhChon{get; set;} = 0;
        public float TyLeBinhChon{set; get;} = 0f;
        public DateTime ngayBD{set; get;}
        public int ID_Cap {set; get;}
        public List<string> listIDCandidate{set; get;}
    }
}