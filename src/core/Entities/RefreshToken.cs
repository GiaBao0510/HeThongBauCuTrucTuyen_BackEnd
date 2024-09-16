

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.src.core.Entities 
{
    [Table("RefreshToken")]
    public class RefreshToken
    {
        [Key]
        public Guid IDrfToken{set; get;}        //ID Rtoken
        public string token{get; set;}          //Nội dung Token
        public string JwtId{set; get;}          //Một refreshtoken sẽ tương ứng với một accesstoken cụ thể nào đó 
        public int IsUsed{set; get;}           //Kiểm tra được sài hay chưa
        public int IsRevoked{set; get;}        //Đã được thu hồi hay chưa
        public DateTime IssuedAt{set; get;}     //Tạo ngày nào 
        public DateTime ExpiredAt{set; get;}    //Hết hạn ngày nào


        //Khóa ngoại
        public string? ID_user{set;get;}
        [ForeignKey(nameof(ID_user))]
        public Users users{set; get;}
    }
}