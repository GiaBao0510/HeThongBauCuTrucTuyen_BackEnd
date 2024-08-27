using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.core.Entities{

    [Table("PhieuBau")]
    public class Vote{
        [Key]
        public string ID_Phieu{set;get;}
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Start Date")]
        public DateTime ngayBD { get; set; } = DateTime.Now;
    }
} 
