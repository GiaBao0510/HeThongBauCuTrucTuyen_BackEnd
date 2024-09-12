using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.src.core.Entities;


namespace BackEnd.src.core.Entities
{
    [Table("CanBo")]
    public class Cadre
    {
        [Key]
        [DisplayName("Identify cadre")]
       public required string ID_canbo{set; get;}
       [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Start date")]
       public DateTime NgayCongTac{set; get;}
       [DisplayName("Note")]
       [MaxLength(255)]
       public string GhiChu {set; get;}

       //Khóa ngoại
       public string ID_user;
       [ForeignKey("ID_user")]  
       public Users user{set;get;}

       //Truy xuất ngược
       public virtual ICollection<ResponseCadre> responseCadre{set;get;}
       public virtual ICollection<CadreNoiticeDetail> cadreNoiticeDetail{set;get;}
       public virtual ICollection<WorkPlace> workPlace{set;get;}
       public virtual ICollection<CadreEducationLevelDetail> cadreEducationLevelDetail{set;get;}

       //Các lớp truy xuất ngược trả về mảng rỗng
       public Cadre(){
        responseCadre = new List<ResponseCadre>();
        cadreNoiticeDetail = new List<CadreNoiticeDetail>();
        workPlace = new List<WorkPlace>();
        cadreEducationLevelDetail = new List<CadreEducationLevelDetail>();
       }
    }
}