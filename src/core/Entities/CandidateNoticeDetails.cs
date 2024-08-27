 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.core.Interfaces;

namespace BackEnd.core.Entities{

    [Table("ChiTietThongBaoUngCuVien")]
    public class CandidateNoticeDetails{
        [ForeignKey("Notifications")]
        public int ID_ThongBao { set; get; }
        [ForeignKey("Candidate")]
        public string ID_ucv { set; get; }
    }
}
