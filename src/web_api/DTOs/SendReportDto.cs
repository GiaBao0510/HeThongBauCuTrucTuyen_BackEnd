using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.src.web_api.DTOs
{
    public class SendReportDto
    {
        public string? IDSender {set;get;}
        public string? YKien {set;get;}
        public DateTime ThoiDiem {set; get;} = DateTime.Now;
    }
}