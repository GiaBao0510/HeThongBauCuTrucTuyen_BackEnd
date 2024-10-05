using System.Numerics;

namespace BackEnd.src.web_api.DTOs
{
    public class VoteDto
    {
        public string? ID_Phieu{set;get;}
        public BigInteger GiaTriPhieuBau{set;get;} = 0;

        //Khóa ngoại
        public string? ngayBD { get; set; }
        public int ID_cap { set; get;}
    }
}