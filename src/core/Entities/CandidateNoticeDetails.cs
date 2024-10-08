 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.core.Interfaces;

namespace BackEnd.core.Entities{

    [Table("ChiTietThongBaoUngCuVien")]
    public class CandidateNoticeDetails{
        //Khóa ngoại
        public int? ID_ThongBao { set; get; }
        [ForeignKey("ID_ThongBao")] 
        public Notifications notifications  { set; get; }
        public string? ID_ucv { set; get; }
        [ForeignKey("ID_ucv")]
        public  Candidate candidate{set;get;}
    }
}
