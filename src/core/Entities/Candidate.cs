using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.core.Interfaces;

namespace BackEnd.core.Entities
{
    [Table("UngCuVien")]
    public class Candidate: Iuser
    {
        [Key]
        [DisplayName("Identify candidates")]
        public string ID_ucv { set; get; }
        [Required]
        [DisplayName("Name")]
        [StringLength(50)]
        public string HoTen {  set; get; }
        [Required]
        [DisplayName("Sex")]
        [StringLength(1)]
        public string GioiTinh { set; get; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date of birth")]
        public DateTime NgaySinh { set; get; }
        [Required]
        [DisplayName("Address")]
        [StringLength(100)]
        public string DiaChiLienLac {  set; get; }
        [Required]
        [DisplayName("Citizen identication")]
        [StringLength(12)]
        public string CCCD { set; get; }
        [Phone]
        [DataType(DataType.PhoneNumber)]
        public string SDT { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        [DisplayName("avatar")]
        [StringLength(12)]
        public string HinhAnh { set; get; }
        [DisplayName("Status")]
        public string TrangThai { set; get; } = "Hoat dong";
        [ForeignKey("Roles")]
        public int RoleID { get; set; }
    }
}

