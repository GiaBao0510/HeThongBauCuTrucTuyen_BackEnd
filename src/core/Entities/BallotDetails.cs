using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.core.Entities;

namespace BackEnd.src.core.Entities
{
    [Table("ChiTietPhieuBau")]
    public class BallotDetails
    {
        //Khóa ngoại
        public required string ID_Phieu{set;get;}
        [ForeignKey("ID_Phieu")]
        public Vote vote {set; get;}
        
        public required string ID_ucv { set; get; }
        [ForeignKey("ID_ucv")]
        public Candidate candidate{set;get;}
        public required string ID_CuTri { set; get; }
        [ForeignKey("ID_CuTri")]
        public Voter voter{set;get;}
        
        public int ID_Khoa { set; get; }
        [ForeignKey("ID_Khoa")]
        public Lock _lock{set;get;}
    }
}