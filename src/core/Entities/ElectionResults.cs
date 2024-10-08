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
        public int SoLuotBinhChon{set; get;} = 0;
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Time of registration")]
        public DateTime ThoiDiemDangKy { get; set;}
        [DisplayName("voting rate")]
        public int TyLeBinhChon{set; get;}

        //Khóa ngoại
        public required string ID_ucv { set; get; }
        [ForeignKey("ID_ucv")]
        public Candidate candidate {set;get;}
        public int ID_Cap { get; set; }
        [ForeignKey("ID_Cap")]
        public ListOfPositions listOfPositions {set;get;} 
        public DateTime ngayBD { get; set; }
        [ForeignKey("ngayBD")]
        public Elections elections{set;get;}

    } 
}
