using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.core.Entities
{
    [Table("DonViBauCu")]
    public class Constituency
    {
        [Key]
        public int ID_DonViBauCu { get; set; }
        [Required]
        [DisplayName("Constituency name")]
        [StringLength(50)]
        public required string TenDonViBauCu { get; set; }
        [DisplayName("Constituency address")]
        [StringLength(255)]
        public required string DiaChi { get; set; }

        //Khóa ngoại
        public int? ID_QH { get; set; }
        [ForeignKey("ID_QH")]
        public District district {set;get;}

        //Truy xuất ngược
        public virtual ICollection<Board> board{set;get;}
        public virtual ICollection<ListOfPositions>listofpositions{set;get;}
        public virtual ICollection<ElectionStatus> electionStatus{set;get;}

        //Cho các đối tượng truy xuất ngược trả về mảng rỗng
        public Constituency(){
            board = new List<Board>();
            listofpositions = new List<ListOfPositions>();
            electionStatus = new List<ElectionStatus>();
        }
    }
}
