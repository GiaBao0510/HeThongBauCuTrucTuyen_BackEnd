using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.core.Entities;

namespace BackEnd.src.core.Entities
{
    [Table("Congtac")]
    public class WorkPlace
    {
        //Khóa ngoại
        public int ID_ChucVu { get; set; }
        [ForeignKey("ID_ChucVu")]
        public Position position {set;get;}
        public int ID_Ban { get; set; }
        [ForeignKey("ID_Ban")]
        public Board board {set;get;}
        public string? ID_canbo{set; get;}
        [ForeignKey("ID_canbo")]
        public Cadre cadre {set;get;}
    }
}