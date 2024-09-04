using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.src.core.Entities
{
    public class PublicKey
    {
        [DisplayName("Modulo")]
        public int Modulo {set;get;} = 0;
        [DisplayName("Semi Random g")]
        public int SemiRandom_g {set;get;} = 0;

        //Khóa ngoại
        public int? ID_Khoa {set;get;}
        [ForeignKey("ID_Khoa")]
        public Lock _lock {set;get;}
    }
}