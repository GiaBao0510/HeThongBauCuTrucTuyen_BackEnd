using System.Numerics;

namespace BackEnd.src.web_api.DTOs
{
    public class LockDTO
    {
        public int ID_Khoa {set;get;}
        public DateTime NgayTao{set; get;}

        public BigInteger N{set;get;} = -1;
        public BigInteger G{set;get;} = -1;
        public string path_PK{set;get;} = "null";
    }
}