

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.src.core.Entities
{
    [Table("HoSoNguoiDung")]
    public class Profiles
    {
        [Key]
        [DisplayName("Profile code")]
        public int MaSo{get; set;}

        [DisplayName("Registration status")]
        public string TrangThaiDangKy{set; get;} = "0";

        //Khóa ngoại
        public string? ID_user { get; set; }
        [ForeignKey("ID_user")]
        public Users users{set; get;}
    }
}