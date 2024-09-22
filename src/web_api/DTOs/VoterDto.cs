
namespace BackEnd.src.web_api.DTOs
{
    public class VoterDto: UserDto
    {
        public string? ID_CuTri{set;get;}
        public string? TenDanToc {set; get;} ="null";
    }
}