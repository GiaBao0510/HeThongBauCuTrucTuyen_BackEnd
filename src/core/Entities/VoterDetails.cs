using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.core.Entities;

namespace BackEnd.src.core.Entities
{
    [Table("ChiTietCuTri")]
    public class VoterDetails
    {
        public string? ID_CuTri;
        [ForeignKey("ID_CuTri")]
        public Voter voter { get; set; }
        public int ID_ChucVu{set;get;}
        [ForeignKey("ID_ChucVu")]
        public Position position { get; set; }
    }
}