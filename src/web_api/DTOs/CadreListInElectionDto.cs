namespace BackEnd.src.web_api.DTOs
{
    public class CadreListInElectionDto
    {
        public List<int>  ListID_ChucVu { get; set; }
        public int ID_Ban { get; set; }
        public List<string> ListID_canbo{set; get;}
        public DateTime ngayBD{set; get;}
    }
}