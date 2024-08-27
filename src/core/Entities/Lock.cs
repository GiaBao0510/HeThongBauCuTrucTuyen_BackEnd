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
        public string KhoaCongKhai{set; get;}
        [DisplayName("Private key")]
        public string KhoaBiMat{set; get;}
    }
}