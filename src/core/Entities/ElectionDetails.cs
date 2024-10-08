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

        //Khóa ngoại
        public string? ID_Phieu{set;get;}
        [ForeignKey("ID_Phieu")]
        public Vote vote {set;get;} 
        public string? ID_CuTri { set; get; }
        [ForeignKey("ID_CuTri")]
        public Voter voter {get; set;}
    }
}
