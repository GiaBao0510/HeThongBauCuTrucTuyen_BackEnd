using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.core.Interfaces;

namespace BackEnd.core.Entities
{
    [Table("ChiTietTrinhDoHocVan")]
    public class EducationLevelDetails
    {
        [ForeignKey("EducationLevel")]
        public int ID_TrinhDo { set; get; }
        [ForeignKey("Candidate")]
        public  required string ID_ucv { set; get; }
    }
}
