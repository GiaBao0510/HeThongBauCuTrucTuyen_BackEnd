using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.core.Entities{

    [Table("PhieuBau")]
    public class Vote{
        [Key]
        public required string ID_Phieu{set;get;}
        [DisplayName("vote value")]
        public int GiaTriPhieuBau{set;get;} = 0;
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Start Date")]
        public DateTime ngayBD { get; set; } = DateTime.Now;
    }
} 
