using System;

namespace BackEnd.core.Interfaces
{
    public interface Iuser
    {
        public string HoTen { set; get; }
        public string GioiTinh { set; get; }
        public DateTime NgaySinh { set; get; }
        public string DiaChiLienLac { set; get; }
        public string CCCD { set; get; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string HinhAnh { set; get; }
    }
} 