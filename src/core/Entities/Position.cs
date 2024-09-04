using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.src.core.Entities
{
    [Table("ChucVu")]
    public class Position
    {
        [Key]
        public int? ID_ChucVu { get; set; }
        [Required]
        [DisplayName("Board name")]
        [MaxLength(50)]
        public required string TenChucVu { get; set; }
        
        //Truy xuất ngược
       public virtual ICollection<WorkPlace>? workPlace{set;get;}

       //Hàm khởi tạo này chủ yếu dùng để cho các đối tượng được truy xuất ngược là trả về một mảng rỗng
       public Position(){
        workPlace = new List<WorkPlace>(); //Một danh sách rỗng
       }
    }
}