using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.src.core.Entities
{
    [Table("Khoa")]
    public class Lock
    {
        [Key]
        public int ID_Khoa {set;get;}
        [DisplayName("Public key")]
        public DateTime NgayTao{set; get;}
        [DisplayName("Private key")]
        public DateTime NgayHetHan{set; get;}

        //Truy xuất ngược
        public virtual ICollection<PrivateKey> privateKey{get;set;}
        public virtual ICollection<PublicKey> publicKey{get;set;}

        //Cho các lớp truy xuất ngược trả về mảng rỗng khi mảng rỗng
        public Lock(){
            privateKey = new List<PrivateKey>();
            publicKey = new List<PublicKey>();
        }
    }
}