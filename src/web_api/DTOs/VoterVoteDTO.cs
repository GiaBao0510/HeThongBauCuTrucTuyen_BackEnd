
using System.Numerics;

namespace BackEnd.src.web_api.DTOs
{
    public class VoterVoteDTO
    {
        public string? ID_CuTri;
        public BigInteger GiaTriPhieuBau;
        public DateTime ngayBD;
        public int ID_Cap;
        public int ID_DonViBauCu;
    }
}