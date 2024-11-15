
namespace BackEnd.src.web_api.DTOs
{
    public class CadreDto : UserDto
    {
        public string? ID_CanBo{set; get;}
        public DateTime? NgayCongTac{set; get;} = DateTime.Now;
        public string? GhiChu {set; get;}
        public int ID_TrinhDo {set; get;} = 12;
    }
}