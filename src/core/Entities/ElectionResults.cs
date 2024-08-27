using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.core.Entities{
    [Table("KetQuaBauCu")]
    public class ElectionResults{
        [Key]
        public int ID_ketQua{set; get;}
        [DisplayName("Number of votes")]
        public int SoLuotBinhChon{set; get;}
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Time of registration")]
        public DateTime ThoiDiemDangKy { get; set;}
        [DisplayName("voting rate")]
        public int TyLeBinhChon{set; get;}
        [ForeignKey("Elections")]
        public DateTime ngayBD { get; set; }
        [ForeignKey("Candidate")]
        public string ID_ucv { set; get; }
        [ForeignKey("ListOfPositions")]
        public int ID_Cap { get; set; }
    }
}
