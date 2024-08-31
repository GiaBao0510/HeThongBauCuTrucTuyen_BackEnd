using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.src.core.Entities
{
    [Table("ChiTietPhieuBau")]
    public class BallotDetails
    {
        [DisplayName("Vote")]
        [StringLength(1)]
        public string BinhChon{get;set;} = "0";
        [ForeignKey("Vote")]
        public required string ID_Phieu{set;get;}
        [ForeignKey("Candidate")]
        public required string ID_ucv { set; get; }
        [ForeignKey("Vouter")]
        public required string ID_CuTri { set; get; }
        [ForeignKey("Lock")]
        public int ID_Khoa { set; get; }
    }
}