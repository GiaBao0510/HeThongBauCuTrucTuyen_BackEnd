using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.core.Interfaces;

namespace BackEnd.core.Entities
{
    [Table("ChiTietTrinhDoHocVanUngCuVien")]
    public class EducationLevelDetails
    {
        public int? ID_TrinhDo { set; get; }
        [ForeignKey("ID_TrinhDo")]
        public EducationLevel educationLevel{set;get;}

        public string? ID_ucv { set; get; }
        [ForeignKey("ID_ucv")]
        public Candidate candidate{set;get;}
    }
}
