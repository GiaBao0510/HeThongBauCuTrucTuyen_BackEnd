using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.core.Entities
{
    [Table("KyBauCu")]
	public class Elections
	{
		[Key]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Start date")]
        public DateTime ngayBD { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("End date")]
        public DateTime ngayKT { get; set; }
        [StringLength(50)]
        [DisplayName("Election name")]
        public required string TenKyBauCu {get; set;}
        [StringLength(50)]
        [DisplayName("Description")]
        public string MoTa { get; set; } = "";
	}
}