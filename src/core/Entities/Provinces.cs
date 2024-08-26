using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.core.Entities
{
    class Province
    {
        [Key]
        [DisplayName("ID_Provice")]
        public int STT { get; set; }
        [Required]
        [StringLength(50)]
        [DisplayName("Provice name")]
        public string TenTinhThanh { set; get; }
    }
}