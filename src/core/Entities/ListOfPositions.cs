using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.core.Entities
{
    class ListOfPositions
    {
        [Key]
        public int ID_Cap { get; set; }
        [Required]
        [DisplayName("Position name")]
        [StringLength(50)]
        public string TenCapUngCu { get; set; }
        [ForeignKey("Constituency")]
        public int ID_DonViBauCu { get; set; }
    }
}
 
