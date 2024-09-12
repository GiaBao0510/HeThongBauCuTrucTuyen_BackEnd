using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.core.Entities;

namespace BackEnd.src.core.Entities
{
    [Table("ChiTietTrinhDoHocVanCanBo")] 
    public class CadreEducationLevelDetail
    {
       public string? ID_canbo{set;get;} 
       [ForeignKey("ID_canbo")] 
       public Cadre cadre{set;get;}
       public string? ID_TrinhDo{set;get;} 
       [ForeignKey("ID_TrinhDo")] 
       public EducationLevel educationLevel{set;get;}  
    }
}