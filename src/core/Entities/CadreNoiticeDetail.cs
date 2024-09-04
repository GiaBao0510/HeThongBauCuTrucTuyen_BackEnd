using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.core.Entities;

namespace BackEnd.src.core.Entities
{
    [Table("chitietthongbaocanbo")]
    public class CadreNoiticeDetail
    {
        //Khóa ngoại
        public int? ID_ThongBao { set; get; }
        [ForeignKey("ID_ThongBao")]
        public Notifications notifications  { set; get; }
        public string? ID_CanBo { set; get; }
        [ForeignKey("ID_CanBo")]
        public Cadre Cadre  { set; get; }
    }
}