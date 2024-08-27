using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.core.Entities
{
    [Table("Ban")]
    public class Board
    {
        [Key]
        public int ID_Ban { get; set; }
        [Required]
        [DisplayName("Board name")]
        [StringLength(50)]
        public string TenBan { get; set; }
        [ForeignKey("Constituency")]
        public int ID_DonViBauCu { get; set; }
    }
}
