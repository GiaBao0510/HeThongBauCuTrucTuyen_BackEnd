using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.core.Interfaces;

namespace BackEnd.core.Entities
{
    [Table("CuTri")]
    public class Vouter : Iuser
    {
        [Key]
        [DisplayName("Identify candidates")]
        public required string ID_CuTri { set; get; }
        [Required]
        [DisplayName("Name")]
        [StringLength(50)]
        public required string HoTen { set; get; }
        [Required]
        [DisplayName("Sex")]
        [StringLength(1)]
        public required string GioiTinh { set; get; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date of birth")]
        public DateTime NgaySinh { set; get; }
        [Required]
        [DisplayName("Address")]
        [StringLength(100)]
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
        public string? Email { get; set; }
        [DisplayName("avatar")]
        [StringLength(12)]
        public string? HinhAnh { set; get; }
        [ForeignKey("Roles")]
        public int RoleID { get; set; }
    }
}

