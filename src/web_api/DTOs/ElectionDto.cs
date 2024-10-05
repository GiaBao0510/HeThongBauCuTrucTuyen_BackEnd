using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.src.web_api.DTOs
{
    public class ElectionDto
    {
        public string? ngayBD { get; set; }
        public string? ngayKT { get; set; }
        public string? NgayKT_UngCu { get; set; }
        public string? TenKyBauCu {get; set;}
        public string? MoTa { get; set; }
        public int SoLuongToiDaCuTri { get; set; }
        public int SoLuongToiDaUngCuVien { get; set; }
        public int SoLuotBinhChonToiDa { get; set; }
    }
}