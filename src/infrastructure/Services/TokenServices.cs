using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BackEnd.src.core.Interfaces;
using BackEnd.src.core.Models;
using BackEnd.src.infrastructure.DataAccess.Context;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.Services
{
    public class TokenServices : IToken
    {
        private readonly DatabaseContext _context;
        private readonly AppSetting _appSettings;

        //Khởi tạo
        public TokenServices(
            DatabaseContext context,
            IOptionsMonitor<AppSetting> optionsMonitor
        ){
            _context = context;    
            _appSettings = optionsMonitor.CurrentValue;     
        }

        //-1.Chuyển số thực sang thời gian
        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate){
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();
        } 

        //0. Tạo ra RefreshToken
        public string GenerateRefreshToken(){
            var random = new byte[32];

            //Tạo random cho mảng byte
            using(var x = RandomNumberGenerator.Create()){
                x.GetBytes(random);

                return Convert.ToBase64String(random);
            }
        }

        //1. Hàm này lưu trữ hoặc cập nhật lại refreshtoken(Nếu trong bảng refreshtoken đã có người dùng rồi)
        public async Task SaveOrUpdateRefreshToken(string jwtId, string refreshToken, string account){
            using(var connection = _context.CreateConnection()){
                await connection.OpenAsync();
                
                //Câu lệnh mysql sẽ kiểm tra nếu chưa có thì Tạo mới. Ngược lại nếu người dùng có refreshtoken thì cập nhật và lưu lại 
                const string sql = @"
                INSERT INTO refreshtoken(token, JwtId, IsUsed, IsRevoked, IssuedAt, ExpiredAt, TaiKhoan)
                VALUES(@token, @JwtId, @IsUsed, @IsRevoked, @IssuedAt, @ExpiredAt, @TaiKhoan)
                ON DUPLICATE KEY UPDATE 
                token = VALUES(token),
                JwtId = VALUES(JwtId),
                IsUsed = VALUES(IsUsed),
                IsRevoked = VALUES(IsRevoked),
                IssuedAt = VALUES(IssuedAt),
                ExpiredAt = VALUES(ExpiredAt);";    
                
                using (var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@token", refreshToken);
                    command.Parameters.AddWithValue("@JwtId", jwtId);
                    command.Parameters.AddWithValue("@IsUsed", 0);
                    command.Parameters.AddWithValue("@IsRevoked", 0);
                    command.Parameters.AddWithValue("@IssuedAt", DateTime.UtcNow);
                    command.Parameters.AddWithValue("@ExpiredAt", DateTime.UtcNow.AddHours(10));
                    command.Parameters.AddWithValue("@TaiKhoan", account);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        //2.Tạo ra token
        public async Task<TokenModel> GenerateToken(LoginModel loginModel)
        {
            if (loginModel == null)
            {
                Console.WriteLine("Lỗi vì loginModel null");
            }

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

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(5),   // Thời gian hết hạn
                signingCredentials: creds               // Cấu hình ký mã hóa token
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            // Lưu vào DB nếu người dùng chưa có refresh token, ngược lại thì cập nhật lại
            await SaveOrUpdateRefreshToken(token.Id, refreshToken, loginModel.account);

            return new TokenModel
            {
                accessToken = accessToken,
                refreshToken = refreshToken
            };
        }

        //4 Vô hiệu hóa refresh token khi người dùng đăng xuất
        public async Task InvalidateRefreshTokenForUser(string TaiKhoan){
            using(var connection = _context.CreateConnection()){
                await connection.OpenAsync();
                
                //Đánh dấu tài khoản đã được thu hồi và sử dụng
                const string sql = @"
                UPDATE refreshtoken
                SET IsUsed = 1, IsRevoked = 1
                WHERE TaiKhoan = @TaiKhoan;";
                
                using (var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@TaiKhoan", TaiKhoan);
                    await command.ExecuteNonQueryAsync();
                }
            }
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
    }
}