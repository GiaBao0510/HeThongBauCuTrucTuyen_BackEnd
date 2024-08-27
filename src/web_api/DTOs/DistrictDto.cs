using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.src.web_api.DTOs
{
    public class DistrictDto
    {
        [Required]
        [StringLength(50)]
        public string TenQH { get; set; }

        [Required]
        public int STT { get; set; }
    }
}