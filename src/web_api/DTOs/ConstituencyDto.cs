using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.src.web_api.DTOs
{
    public class ConstituencyDto
    {
        public int ID_DonViBauCu { get; set; }
        [Required]
        [DisplayName("Constituency name")]
        [StringLength(50)]
        public required string TenDonViBauCu { get; set; }
        [DisplayName("Constituency address")]
        [StringLength(255)]
        public required string DiaChi { get; set; }
        [Required]
        public int ID_QH { get; set; }
    }
}