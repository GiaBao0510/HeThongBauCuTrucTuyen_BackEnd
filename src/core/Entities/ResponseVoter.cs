using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.core.Entities;

namespace BackEnd.src.core.Entities{
    [Table("PhanHoiCuTri")]
    public class ResponseVoter{
        [DisplayName("Opinion")]
        [StringLength(255)]
        public required string YKien {set; get;} 
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Time")]
        public DateTime ThoiDiem { get; set; }
        public string? ID_CuTri { set; get; }
        [ForeignKey("Voter")]
        public Voter voter { set; get; }
    }
}
