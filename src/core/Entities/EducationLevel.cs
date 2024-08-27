using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.core.Interfaces;

namespace BackEnd.core.Entities
{
    [Table("TrinhDoHocVan")]
	public class EducationLevel
	{
		[Key]
        [DisplayName("Determine educational level")]
        public int ID_TrinhDo { set; get; }
        [StringLength(50)]
        [DisplayName("Name of educational level")]
        public string TenTrinhDoHocVan { set; get; }
	}
}