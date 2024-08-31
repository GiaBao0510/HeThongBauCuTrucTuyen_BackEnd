using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.core.Entities
{
    [Table("QuanHuyen")]
    public class District
    {
        [Key]
        [DisplayName("ID_District")]
        public int ID_QH { get; set; }
        [Required]
        [StringLength(50)]
        [DisplayName("District name")]
        public required string TenQH { get; set; }
        [ForeignKey("Province")]
        public int STT { get; set; }
    }
}