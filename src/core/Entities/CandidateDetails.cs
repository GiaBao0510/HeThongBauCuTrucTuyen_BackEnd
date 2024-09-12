using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.core.Entities;

namespace BackEnd.src.core.Entities
{
    [Table("ChiTietUngCuVien")]

    public class CandidateDetails
    {
        public string? ID_ucv;
        [ForeignKey("ID_ucv")]
        public Candidate candidate { get; set; }
        public int ID_ChucVu{set;get;}
        [ForeignKey("ID_ChucVu")]
        public Position position { get; set; }
    }
}