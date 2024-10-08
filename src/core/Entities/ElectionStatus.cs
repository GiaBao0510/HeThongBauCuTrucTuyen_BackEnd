using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.core.Entities{

    [Table("TrangThaiBauCu")]
    public class ElectionStatus{
        [DisplayName("Note")]
        [StringLength(1)] 
        public string GhiNhan{set; get;}="0";
        public string? ID_CuTri { set; get; }
        [ForeignKey("ID_CuTri")]
        public Voter voter{get;set;}
        public int ID_DonViBauCu { get; set; }
        [ForeignKey("ID_DonViBauCu")]
        public Constituency constituency{set;get;}
        public DateTime ngayBD { get; set; }
        [ForeignKey("ngayBD")]
        public Elections elections{get; set;}
    }
}

