
namespace BackEnd.src.web_api.DTOs
{
    public class ListOfPositionDTO
    {
        public string? TenCapUngCu { get; set; }= "null";
        
        //Khóa ngoại
        public int ID_DonViBauCu { get; set; } = -1;
    }
}