using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.src.web_api.DTOs
{
    public class VoteDto
    {
        public string? ID_Phieu{set;get;}
        public int GiaTriPhieuBau{set;get;} = 0;

        //Khóa ngoại
        public string? ngayBD { get; set; }
    }
}