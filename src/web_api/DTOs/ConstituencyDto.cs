namespace BackEnd.src.web_api.DTOs
{
    public class ConstituencyDto
    {
        public int ID_DonViBauCu { get; set; } = -1;
        public string? TenDonViBauCu { get; set; } = "null";
        public string?  DiaChi { get; set; } = "null";
        public int ID_QH { get; set; }  = -1;
    }
}