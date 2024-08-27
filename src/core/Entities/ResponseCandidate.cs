using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.src.core.Entities{
    [Table("PhanHoiUngCuVien")]
    public class ResponseCandidate{
        [DisplayName("Opinion")]
        [StringLength(255)]
        public string YKien {set; get;} 
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Time")]
        public DateTime ThoiDiem { get; set; }
        [ForeignKey("Candidate")]
        public string ID_ucv { set; get; }
    }
}

