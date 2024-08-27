
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.core.Entities
{
    [Table("LichSuDangNhap")]
    public class LoginHistory
    {
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Time")]
        public DateTime ThoiDiem { get; set; }
        [DisplayName("IP Address")]
        public string DiaChiIP { get; set; }
        [ForeignKey("Account")]
        public string TaiKhoan { get; set; }
    }
}
