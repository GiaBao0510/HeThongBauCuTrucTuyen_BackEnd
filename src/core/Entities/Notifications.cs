using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.src.core.Entities;

namespace BackEnd.core.Entities
{
    [Table("ThongBao")] 
    public class Notifications
    {
        [Key]
        public int? ID_ThongBao { set; get; }
        [StringLength(100)]
        [DisplayName("Notification content")]
        public required string NoiDungThongBao { set; get; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Time")]
        public DateTime ThoiDiem { set;get; } = DateTime.Now;

        //Truy xuất ngược 
        public virtual ICollection<CandidateNoticeDetails>? candidateNoticeDetails{set;get;}
        public virtual ICollection<CadreNoiticeDetail>? cadreNoiticeDetail{set;get;}
        public virtual ICollection<VoterNoticeDetails>? voterNoticeDetails{set;get;}  
    }
}