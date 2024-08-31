using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.core.Entities
{
    [Table("DonViBauCu")]
    public class Constituency
    {
        [Key]
        public int ID_DonViBauCu { get; set; }
        [Required]
        [DisplayName("Constituency name")]
        [StringLength(50)]
        public required string TenDonViBauCu { get; set; }
        [DisplayName("Constituency address")]
        [StringLength(255)]
        public required string DiaChi { get; set; }
        [ForeignKey("District")]
        public int ID_QH { get; set; }
    }
}
