using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.core.Entities{
    public class ElectionDetails{
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Start Date")]
        public DateTime ThoiDiemBau { get; set;}
        [DisplayName("Vote")]
        [StringLength(1)]
        public string BinhChon{get;set;} = "0";
        [ForeignKey("Vote")]
        public string ID_Phieu{set;get;}
        [ForeignKey("Candidate")]
        public string ID_ucv { set; get; }
        [ForeignKey("Vouter")]
        public string ID_CuTri { set; get; }
    }
}
