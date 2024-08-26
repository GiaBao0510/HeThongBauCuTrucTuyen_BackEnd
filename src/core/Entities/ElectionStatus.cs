using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.core.Entities{
    public class ElectionStatus{
        [DisplayName("Note")]
        [StringLength(1)] 
        public string GhiNhan{set; get;}="0";
        [ForeignKey("Voters")]
        public string ID_CuTri { set; get; }
        [ForeignKey("Constituency")]
        public int ID_DonViBauCu { get; set; }
        [ForeignKey("Elections")]
        public DateTime ngayBD { get; set; }
        
    }
}

