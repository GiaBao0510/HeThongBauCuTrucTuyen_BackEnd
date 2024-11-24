namespace BackEnd.src.web_api.DTOs
{
    public class VotingHistoryDTO
    {
        public string? ID_Phieu { get; set; }
        public DateTime ThoiDiemBoPhieu{set; get;}
        public int XacThuc{set; get;}
        public string? TenCapUngCu{set; get;}
        public DateTime ngayBD { get; set; }
    }
}