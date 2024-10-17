using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using BackEnd.core.Entities;

namespace BackEnd.src.core.Entities
{
    [Table("Khoa")]
    public class Lock
    {
        [Key]
        public int ID_Khoa {set;get;}
        [DisplayName("Ngay Tao")]
        public DateTime NgayTao{set; get;}
        [DisplayName("Modulo N")]

        public BigInteger N{set;get;} = 0;
        [DisplayName("Semi random G")]
        public BigInteger G{set;get;} = 0;
        [DisplayName("path private key")]
        public string path_PK{set;get;} = "null";
        
        //Khóa ngoại
        public DateTime ngayBD{set; get;}
        [ForeignKey("ngayBD")]
        public Elections elections{get; set;}

        //Truy xuất ngược
        public virtual ICollection<BallotDetails> ballotDetails{get;set;}

        //Cho các lớp truy xuất ngược trả về mảng rỗng khi mảng rỗng
        public Lock(){
            ballotDetails = new List<BallotDetails>();
        }
    }
}