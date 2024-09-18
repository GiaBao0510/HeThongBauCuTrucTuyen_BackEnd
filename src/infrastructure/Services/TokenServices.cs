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
        public async Task<TokenModel> GenerateToken(LoginModel loginModel){
            if(loginModel == null){
                Console.WriteLine("Lỗi vì loginModel null");
            }

            //Lấy mảng byte giống bên biến SecretKeyBytes trong file Starup.cs
            var secretKeyByte = new SymmetricSecurityKey( Encoding.UTF8.GetBytes(_appSettings.SecretKey));
            var creds = new SigningCredentials(secretKeyByte, SecurityAlgorithms.HmacSha512Signature);

            //Chỉ định các Claims cho nguoiwg dùng
            var Claims = new List<Claim>(){
                new Claim("SDT", loginModel.account),
                new Claim(ClaimTypes.Role, loginModel.Role),
                new Claim(ClaimTypes.Email, loginModel.Email),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("SuDung", loginModel.SuDung.ToString()),
                new Claim("BiKhoa", loginModel.BiKhoa)
            };

            var Token = new JwtSecurityToken(
                claims: Claims,
                expires: DateTime.UtcNow.AddHours(5),   //Thời gian hết hạn
                signingCredentials: creds               //Cấu hình ký mã hóa token

            );

            var AccessToken = new JwtSecurityTokenHandler().WriteToken(Token);
            var RefreshToken = GenerateRefreshToken();

            //Lưu vào DB nếu người dùng chưa có refreshtoken, ngược lại thì cập nhật lại
            await SaveOrUpdateRefreshToken(Token.Id, RefreshToken, loginModel.account);
            
            return new TokenModel{
                accessToken = AccessToken,
                refreshToken = RefreshToken
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
    }
}