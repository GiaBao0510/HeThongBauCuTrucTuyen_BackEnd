using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.core.Entities
{
    [Table("ChiTietThongBaoCuTri")]
    public class VoterNoticeDetails
    {
        [ForeignKey("Notifications")]
        public int ID_ThongBao { set; get; }
        [ForeignKey("Vouter")]
        public string ID_CuTri { set; get; }
    }
}
