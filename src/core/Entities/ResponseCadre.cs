using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.core.Entities;

namespace BackEnd.src.core.Entities
{
    [Table("PhanHoiCanBo")]
    public class ResponseCadre
    {
        [DisplayName("Opinion")]
        [StringLength(255)]
        public required string YKien {set; get;} 
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Time")]
        public DateTime ThoiDiem { get; set; } = DateTime.Now;
        
        //Khóa ngoại
        public string? ID_canbo { set; get; }
        [ForeignKey("ID_canbo")]
        public Cadre Cadre  { set; get; }
    }
}