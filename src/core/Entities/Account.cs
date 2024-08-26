using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.core.Entities
{
    class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TaiKhoan { get; set; }
        [Required]
        [StringLength(100)]
        [DisplayName("PassWord")]
        public string MatKhau { get; set; }
        [StringLength(1)]
        [DisplayName("Looked")]
        public string BiKhoa { get; set; }
        [StringLength(100)]
        [DisplayName("Reason for lock")]
        public string LyDoKhoa { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date created")]
        public DateTime NgayTao { get; set; } = DateTime.Now;
        [DisplayName("Use")]
        public int SuDung { get; set; } = 1;
        [ForeignKey("Roles")]
        public int RoleID { set; get; }
    }
}
