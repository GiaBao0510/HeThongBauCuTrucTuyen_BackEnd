using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using BackEnd.src.core.Entities;

namespace BackEnd.core.Entities{

    [Table("PhieuBau")]
    public class Vote{
        [Key]
        public string? ID_Phieu{set;get;}
        [DisplayName("vote value")]
        public BigInteger GiaTriPhieuBau{set;get;} = 0;

        //Khóa ngoại
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Start Date")]
        public DateTime? ngayBD { get; set; }
        [ForeignKey("ngayBD")]
        public Elections elections{get;set;}
        
        //truy xuất ngược
        public virtual ICollection<ElectionDetails> electionDetails {set;get;}
        public virtual ICollection<BallotDetails> ballotDetails {set;get;}

        //Cho các lớp truy xuất ngược trả về mảng rỗng khi mảng rỗng
        public Vote(){
            electionDetails = new List<ElectionDetails>();
            ballotDetails = new List<BallotDetails>();
        }
    }
} 
