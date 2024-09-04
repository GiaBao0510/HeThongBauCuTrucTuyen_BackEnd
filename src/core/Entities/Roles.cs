using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.core.Interfaces;
using BackEnd.src.core.Entities;

namespace BackEnd.core.Entities
{
    [Table("VaiTro")]
    public class Roles : IRole
    {
        [Key]
        [DisplayName("Role definition")]
        public int RoleID { set; get; }
        [Required]
        [DisplayName("Role name")]
        [StringLength(30)]
        public required string TenVaiTro { set; get; }

        //Truy xuất ngược lại
        public virtual ICollection<Account> account {set;get;}
        public virtual ICollection<User> user {set;get;}

        //Cho các lớp truy xuất ngược trả về mảng rỗng khi mảng rỗng
        public Roles(){
            account = new List<Account>();
            user = new List<User>();
        }
    }
}