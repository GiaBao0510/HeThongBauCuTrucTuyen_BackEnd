using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.src.core.Entities
{
    public class PrivateKey
    {
        [DisplayName("Camichanel funtion")]
        public double HamCamichanel {set;get;} = 0;
        [DisplayName("b -value division")]
        public double GiaTriB_Phan {set;get;} = 0;
        [ForeignKey("Lock")]
        public double ID_Khoa {set;get;}
    }
}