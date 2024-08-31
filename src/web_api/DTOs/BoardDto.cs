using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.src.web_api.DTOs
{
    public class BoardDto
    {
        public int? ID_Ban { get; set; }
        [Required]
        [StringLength(50)]
        public required string TenBan { get; set; }
        [Required]
        public int ID_DonViBauCu { get; set; }
    }
}