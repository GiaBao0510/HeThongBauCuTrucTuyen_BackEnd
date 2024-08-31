using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public DateTime ThoiDiem { set;get; }
    }
}