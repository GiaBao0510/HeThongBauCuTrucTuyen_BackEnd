namespace BackEnd.src.web_api.DTOs
{
    public class CandidateDto : UserDto
    {
        public string? ID_ucv{set;get;}
        public string? TrangThai{set;get;} = "acctive";
        public int SoLuotBinhChon{set;get;} = 0;
        public DateTime ThoiDiemDangKy{set;get;} = DateTime.Now;
        public float TyLeBinhChon{set;get;} = 0f;
        public DateTime ngayBD {set;get;}
        public int ID_Cap {set;get;}
    }
}