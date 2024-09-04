using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.core.Interfaces;

namespace BackEnd.src.core.Entities
{
    [Table("DanToc")]
    public class Ethnicity
    {
        [Key]
        [DisplayName("Identify ethnicity")]
        public int? ID_DanToc{get;set;}
        [DisplayName("Ethnic name")]
        [StringLength(20)]
        public required string TenDanToc{get;set;}
        [DisplayName("Other names")]
        [StringLength(100)]
        public string? TenGoiKhac{get;set;} = "null";

        //Lấy ngược lại, theo ID_DanToc
        public virtual ICollection<User> users {get;set;}

        //Cho các lớp truy xuất ngược trả về mảng rỗng khi mảng rỗng
        public Ethnicity(){
            users = new List<User>();
        }
    }
}