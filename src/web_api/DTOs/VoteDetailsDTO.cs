using System.Numerics;

namespace BackEnd.src.web_api.DTOs
{
    public class VoteDetailsDTO
    {
        public string? ID_Phieu{set;get;} = "null";
        public BigInteger GiaTriPhieuBau{set;get;} = 0;
        public DateTime ThoiDiem { get; set; }
        public string? ID_user { set; get;} = "null";
        public string? HoTen { set; get;} = "null";
    }
}