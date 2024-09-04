using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.core.Interfaces;
using BackEnd.src.core.Entities;

namespace BackEnd.core.Entities
{
    [Table("TrinhDoHocVan")]
	public class EducationLevel
	{
		[Key]
        [DisplayName("Determine educational level")]
        public int? ID_TrinhDo { set; get; }
        [StringLength(50)]
        [DisplayName("Name of educational level")]
        public required string TenTrinhDoHocVan { set; get; }

        //Truy xuất ngược
        public virtual ICollection<CadreEducationLevelDetail> cadreEducationLevelDetail{set;get;}
        public virtual ICollection<EducationLevelDetails> educationLevelDetails{set;get;}

        //Cho các lớp truy xuất ngược trả về mảng rỗng
        public EducationLevel(){
            cadreEducationLevelDetail = new List<CadreEducationLevelDetail>();
            educationLevelDetails = new List<EducationLevelDetails>();

        }
	}
}