using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.src.core.Entities
{
    [Table("KhoaBiMat")]
    public class PrivateKey
    {
        [DisplayName("Camichanel funtion")]
        public int HamCamichanel {set;get;} = 0;
        [DisplayName("b -value division")]
        public int GiaTriB_Phan {set;get;} = 0;

        //Khóa ngoại
        public int? ID_Khoa {set;get;}
        [ForeignKey("ID_Khoa")]
        public Lock _lock {set;get;}
    }
}