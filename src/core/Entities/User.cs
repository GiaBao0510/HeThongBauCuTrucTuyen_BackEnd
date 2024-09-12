using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.core.Entities;

namespace BackEnd.src.core.Entities
{
    [Table("NguoiDung")]
    public class Users
    {
        [Key]
        [DisplayName("Identify user")]
        public required string ID_user{set;get;}
        [DisplayName("Name")]
        [StringLength(50)]
        public required string HoTen { set; get; }
        [Required]
        [DisplayName("Sex")]
        [StringLength(1)]
        public required string GioiTinh { set; get; } = "1";
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date of birth")]
        public required DateTime NgaySinh { set; get; }
        [Required]
        [DisplayName("Address")]
        [StringLength(150)]
        public required string DiaChiLienLac { set; get; }
        [Required]
        [DisplayName("Citizen identication")]
        [StringLength(12)]
        public required string CCCD { set; get; }
        [Phone]
        [DataType(DataType.PhoneNumber)]
        public required string SDT { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public required string Email { get; set; }
        [DisplayName("PathImage")]
        [StringLength(255)]
        public string? HinhAnh { set; get; }
        [DisplayName("PublicID")]
        [StringLength(50)]
        public string? PublicID { set; get; }

        //Khóa ngoại
        public int? ID_DanToc{set; get;}
        [ForeignKey("ID_DanToc")]
        public Ethnicity ethnicity{set; get;}
        public int? RoleID{set; get;}
        [ForeignKey("Roles")]
        public  Roles roles {set;get;}

        //Truy xuất ngược lại
        public virtual ICollection<Voter> voter {get; set;}
        public virtual ICollection<Candidate> candidates{get;set;}
        public virtual ICollection<Cadre> cadre{set; get;}
        public virtual ICollection<TemporaryAddress> temporaryAddress{set; get;}
        public virtual ICollection<PermanentAddress> permanentAddress{set; get;}
        public virtual ICollection<Profiles> profile {set; get;}

        //Cho các lớp truy xuất ngược trả về mảng rỗng khi mảng rỗng
        public Users(){
            voter = new List<Voter>();
            candidates = new List<Candidate>();
            cadre = new List<Cadre>();
            temporaryAddress = new List<TemporaryAddress>();
            permanentAddress = new List<PermanentAddress>();
            profile = new List<Profiles>();
        }
    }
}