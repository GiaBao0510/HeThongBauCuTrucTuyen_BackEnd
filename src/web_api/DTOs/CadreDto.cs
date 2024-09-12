
namespace BackEnd.src.web_api.DTOs
{
    public class CadreDto : UserDto
    {
       public string? ID_CanBo{set; get;}
       public DateTime? NgayCongTac{set; get;}
       public string? GhiChu {set; get;}
    }
}