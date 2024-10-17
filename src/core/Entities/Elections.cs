using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.src.core.Entities;

namespace BackEnd.core.Entities
{ 
    [Table("KyBauCu")]
	public class Elections
	{
		[Key]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Start date")]
        public DateTime ngayBD { get; set; } = DateTime.Now.ToLocalTime();
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("End date")]
        public DateTime ngayKT { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Closing date of nomination")]
        public DateTime NgayKT_UngCu { get; set; }
        [StringLength(50)]
        [DisplayName("Election name")]
        public required string TenKyBauCu {get; set;}
        [StringLength(255)]
        [DisplayName("Description")]
        public string MoTa { get; set; } = "";
        public int SoLuongToiDaCuTri { get; set; } = 0;
        public int SoLuongToiDaUngCuVien { get; set; } = 0;
        public int SoLuotBinhChonToiDa { get; set; } = 0;
        public int ID_Cap { get; set; } = 0;

        //Truy xuất ngược
        public virtual ICollection<Vote> vote{set;get;}
        public virtual ICollection<ElectionResults> electionResults{set;get;}
        public virtual ICollection<ElectionStatus> electionStatus{set;get;}
        public virtual ICollection<Lock> _lock{set;get;}
        //Trả về mảng rỗng cho các lớp truy xuất ngược
        public Elections(){
            vote = new List<Vote>();
            electionResults = new List<ElectionResults>();
            electionStatus = new List<ElectionStatus>();
            _lock = new List<Lock>();
        }
	}
}