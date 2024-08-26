using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.core.Entities{
    public class PermanentAddress{
        [ForeignKey("Candidate")]
        public string ID_ucv { set; get; }
        [ForeignKey("Vouter")]
        public string ID_CuTri { set; get; }
        [ForeignKey("District")]
        public int ID_QH { get; set; }
    }
}