using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.core.Entities{
    [Table("ChiTietBauCu")]
    public class ElectionDetails{
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Start Date")]
        public DateTime ThoiDiemBau { get; set;}
       [ForeignKey("Vote")]
        public required string ID_Phieu{set;get;}
        [ForeignKey("Vouter")]
        public required string ID_CuTri { set; get; }
    }
}
