using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BackEnd.src.core.Interfaces;
using BackEnd.src.core.Models;
using BackEnd.src.infrastructure.DataAccess.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.Services
{
    public class TokenServices : IToken
    {
        private readonly DatabaseContext _context;
        private readonly AppSetting _appSettings;
        private readonly IConfiguration _configuration;

        //Khởi tạo
        public TokenServices(
            DatabaseContext context,
            IOptionsMonitor<AppSetting> optionsMonitor,
            IConfiguration configuration
        ){
            _context = context;    
            _appSettings = optionsMonitor.CurrentValue;
            _configuration = configuration;     
        }

        //1.Chuyển số thực sang thời gian
        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate){
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();
        } 

        //2.Tạo ra token
        public async Task<TokenModel> GenerateToken(LoginModel loginModel)
        {
            // Lấy mảng byte giống bên biến SecretKeyBytes trong file Startup.cs
            var secretKeyByte = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.SecretKey));
            var creds = new SigningCredentials(secretKeyByte, SecurityAlgorithms.HmacSha256Signature);

            // Chỉ định các Claims cho người dùng
            var claims = new List<Claim>()
            {
                new Claim("SDT", loginModel.account),
                new Claim(ClaimTypes.Role, loginModel.Role),
                new Claim(ClaimTypes.Email, loginModel.Email),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("SuDung", loginModel.SuDung.ToString()),
                new Claim("BiKhoa", loginModel.BiKhoa)
            };

            // Tạo Accesstoken
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),   // Thời gian hết hạn
                signingCredentials: creds               // Cấu hình ký mã hóa token
            );

            //Tạo RefreshToken
            var token2 = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(14),   // Thời gian hết hạn
                signingCredentials: creds               // Cấu hình ký mã hóa token
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = new JwtSecurityTokenHandler().WriteToken(token2);

            return new TokenModel
            {
                accessToken = accessToken,
                refreshToken = refreshToken
            };
        }

        //5.kiểm tra xem token có hợp lệ không
        public int _CheckLogined(string token)
        {
            var jwtTokenHandler= new JwtSecurityTokenHandler();

            //Đọc thông tin từ token
            var tokenData = jwtTokenHandler.ReadJwtToken(token);
            
            //Lấy mảng byte giống bên biến SecretKeyBytes trong file Starup.cs
            var SecretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);
            var tokenValidParameters =  new TokenValidationParameters{
                    //Tự cấp token
                    ValidateIssuer = false,     //Không kiểm tra nguoiwg phát hành token
                    ValidateAudience = false,   //Không kiểm tra đối tượng nhận token
                    ValidateLifetime = false,   //Không kiểm tra hết hạn của token. Vì đây là giai hạn lại toekn. Nếu không khai thuộc tính này có giá trị false thì nó sẽ báo lỗi

                    //Ký vào token
                    ValidateIssuerSigningKey = true,                                //kiểm tra khóa bí mật đã được sử dụng ký vào token, đảm bảo việc token không bị thay đổi
                    IssuerSigningKey = new SymmetricSecurityKey(SecretKeyBytes),    //Khóa bí mật được sử dụng ở dạng byte sẽ được sử dụng để xác thực token, đây cũng là khóa sử dụng để ký xác thực
                    ClockSkew = TimeSpan.Zero                                       //Đặt độ lệch thời gian xác thực token là 0. 
            };
            //Check1:  Kiểm tra valid format token
            var tokenInverification = jwtTokenHandler.ValidateToken(token, 
                tokenValidParameters, out var validatedToken);

            //Check2: kiểm tra thuật toán mã hóa. Nếu không đúng thuật toán mã hóa thì báo lỗi
            if(validatedToken is JwtSecurityToken jwtSecurityToken){
                var result = jwtSecurityToken.Header.Alg.Equals //So sánh kiểm tra có đúng thuật toán này có dùng để mã hóa khồn
                    ("http://www.w3.org/2001/04/xmldsig-more#hmac-sha256", StringComparison.InvariantCultureIgnoreCase);

                if(!result)
                    return 0;
            }

            //Check3: Kiểm tra xem accesstoken này còn hạn hay không
            var utcExpireDate = long.Parse(tokenInverification.Claims.FirstOrDefault(
                x => x.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Exp).Value);
            
                //Chuyển đổi giá trị thực sang
            var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
            if(expireDate < DateTime.UtcNow){
                return -1;
            }

            return 1;
        }

        //Làm mới tại accesstoken cho người dùng
        public async Task<TokenModel> _RenewToken(LoginModel loginModel){
            // Lấy mảng byte giống bên biến SecretKeyBytes trong file Startup.cs
            var secretKeyByte = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.SecretKey));
            var creds = new SigningCredentials(secretKeyByte, SecurityAlgorithms.HmacSha256Signature);

            // Chỉ định các Claims cho người dùng
            var claims = new List<Claim>()
            {
                new Claim("SDT", loginModel.account),
                new Claim(ClaimTypes.Role, loginModel.Role),
                new Claim(ClaimTypes.Email, loginModel.Email),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("SuDung", loginModel.SuDung.ToString()),
                new Claim("BiKhoa", loginModel.BiKhoa)
            };

            // Tạo Accesstoken
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),   // Thời gian hết hạn
                signingCredentials: creds               // Cấu hình ký mã hóa token
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            
            return new TokenModel
            {
                accessToken = accessToken
            };
        }
    



    }
}