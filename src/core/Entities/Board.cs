using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.src.core.Entities;

namespace BackEnd.core.Entities
{
    [Table("Ban")]
    public class Board
    {
        [Key]
        public int ID_Ban { get; set; }
        [Required]
        [DisplayName("Board name")]
        [StringLength(50)]
        public required string TenBan { get; set; }
        
        //Khoa ngoại
        public int ID_DonViBauCu { get; set; }
        [ForeignKey("ID_DonViBauCu")]
        public Constituency constituency{get; set;}

        //Truy xuất ngược
       public virtual ICollection<WorkPlace> workPlace{set;get;}

       //Tạo danh sách rỗng cho đối tượng bị truy xuất ngược
       public Board(){
        workPlace = new List<WorkPlace>();
       }

    }
}
