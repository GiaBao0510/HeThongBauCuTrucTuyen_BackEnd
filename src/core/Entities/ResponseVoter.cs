using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.src.core.Entities{
    public class ResponseVoter{
        [DisplayName("Opinion")]
        [StringLength(255)]
        public string YKien {set; get;} 
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Time")]
        public DateTime ThoiDiem { get; set; }
        [ForeignKey("Vouter")]
        public string ID_CuTri { set; get; }
    }
}
