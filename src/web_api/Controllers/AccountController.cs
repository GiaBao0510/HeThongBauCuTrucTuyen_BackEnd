using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackEnd.src.core.Models;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.web_api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;
using log4net;
using BackEnd.src.infrastructure.DataAccess.Context;
using MySql.Data.MySqlClient;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController: ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly AppSetting _appSettings;
        private readonly DatabaseContext _context;
        private static readonly ILog _log = LogManager.GetLogger(typeof(AccountController));

        //Khởi tạo
        public AccountController(
            IUserServices userServices,
            IOptionsMonitor<AppSetting> optionsMonitor,
            DatabaseContext context
        ){
            _userServices = userServices;
            _context = context;    
            _appSettings = optionsMonitor.CurrentValue;     //Lấy giá trị đã định nghĩa trong file appsettings
        }

        //0.Chuyển số thực sang thời gian
        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate){
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();
        }

        //0.1.Hàm này tạo ra refresh token
        private string GenerateRefreshToken(){
            var random = new byte[32];

            //Tạo random cho mảng byte
            using(var x = RandomNumberGenerator.Create()){
                x.GetBytes(random);

                return Convert.ToBase64String(random);
            }
        }

        //0.2.Hàm này tạo ra token
        private async Task<TokenModel> GenerateToken(LoginModel loginModel){
            if(loginModel == null){
                Console.WriteLine("Lỗi vì loginModel null");
            }
            Console.WriteLine($"account: {loginModel.account}");
            Console.WriteLine($"Block: {loginModel.BiKhoa}");
            Console.WriteLine($"Role: {loginModel.Role}");
            Console.WriteLine($"SuDung: {loginModel.SuDung}");

            //Lấy mảng byte giống bên biến SecretKeyBytes trong file Starup.cs
            var secretKeyByte = new SymmetricSecurityKey( Encoding.UTF8.GetBytes(_appSettings.SecretKey));
            var creds = new SigningCredentials(secretKeyByte, SecurityAlgorithms.HmacSha512Signature);

            //Chỉ định các Claims cho nguoiwg dùng
            var Claims = new List<Claim>(){
                new Claim("SDT", loginModel.account),
                new Claim(ClaimTypes.Role, loginModel.Role),
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
        
        //0.3. Hàm này lưu trữ hoặc cập nhật lại refreshtoken(Nếu trong bảng refreshtoken đã có người dùng rồi)
        private async Task SaveOrUpdateRefreshToken(string jwtId, string refreshToken, string account){
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

        //0.4 Vô hiệu hóa refresh token khi người dùng đăng xuất
        private async Task InvalidateRefreshTokenForUser(string TaiKhoan){
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

        //1.Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginModel loginModel){
            try{
                //Nếu không điển cũng quăng ra lỗi luôn
                if(string.IsNullOrEmpty(loginModel.account) || string.IsNullOrEmpty(loginModel.password))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền đầy đủ thông tin đăng nhập."});

                var result = await _userServices._Login(loginModel);
                if(result == null)          //Đăng nhập thất bại
                    return BadRequest(
                        new ApiRespons{Success = false, Message = "Tài khoản hoặc mật khẩu không chính xác."}
                    );

                //Đăng nhập thành công
                var token = await GenerateToken(result);

                return Ok(new
                    ApiRespons{
                        Success = true, 
                        Message = "Đăng nhập thành công", 
                        Data = token
                    }
                );
            }
            catch(Exception ex){
                Console.WriteLine($"Error message:{ex.Message}");
                Console.WriteLine($"Error TargetSite:{ex.TargetSite}");
                Console.WriteLine($"Error StackTrace:{ex.StackTrace}");
                Console.WriteLine($"Error Data:{ex.Data}");
                return StatusCode(500, new ApiRespons{
                    Success = false, 
                    Message = $"Lỗi khi thực hiện đăng nhập tài khoản Người dùng: {ex.Message}"
                });
            }
        }

        //2.Giai hạn lại token
        [HttpPost("renewtoken")]
        public async Task<IActionResult> RenewToken([FromBody] TokenModel tokenModel){
            using var connection = await _context.Get_MySqlConnection();
            var jwtTokenHandler= new JwtSecurityTokenHandler();

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

            try{
                //Check1:  Kiểm tra valid format token
                var tokenInverification = jwtTokenHandler.ValidateToken(tokenModel.accessToken, 
                    tokenValidParameters, out var validatedToken);

                //Check2: kiểm tra thuật toán mã hóa. Nếu không đúng thuật toán mã hóa thì báo lỗi
                if(validatedToken is JwtSecurityToken jwtSecurityToken){
                    var result = jwtSecurityToken.Header.Alg.Equals //So sánh kiểm tra có đúng thuật toán này có dùng để mã hóa khồn
                        (SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                    
                    //Nếu không hợp lệ
                    if(!result)
                        return Ok(new ApiRespons{
                            Success = false, 
                            Message = "Invalid token"
                        });
                }

                //Check3: Kiểm tra xem accesstoken này còn hạn hay không
                var utcExpireDate = long.Parse(tokenInverification.Claims.FirstOrDefault(
                    x => x.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Exp).Value);
                
                    //Chuyển đổi giá trị thực sang
                var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                if(expireDate > DateTime.UtcNow){
                    return Ok(new ApiRespons{
                            Success = false, 
                            Message = "AccessToken has not yet expire"
                        });
                }

                //Check4: Kiểm tra ResfreshToken còn tồn tại trong DB không
                const string sqlCheckRefreshTokenExists = @"SELECT token,JwtId FROM refreshtoken WHERE token =@token;";
                string storedToken = null,
                        JwtId = null,
                        TaiKhoan = null;
                using (var command = new MySqlCommand(sqlCheckRefreshTokenExists, connection)){
                    command.Parameters.AddWithValue("@token", tokenModel.refreshToken);
                    
                    using var reader = await command.ExecuteReaderAsync();
                    if(await reader.ReadAsync()){
                        storedToken = reader.GetString(reader.GetOrdinal("token"));
                        JwtId = reader.GetString(reader.GetOrdinal("JwtId"));
                        TaiKhoan = reader.GetString(reader.GetOrdinal("TaiKhoan"));
                    }
                }
                    //Không có tồn tại thì trả về
                if(
                    string.IsNullOrEmpty(storedToken) || 
                    string.IsNullOrEmpty(JwtId) || string.IsNullOrEmpty(TaiKhoan)
                )
                    return Ok(new ApiRespons{
                            Success = false, 
                            Message = "RefreshToken dose not exists"
                        });

                //Check5: Kiểm tra xem RefreshToken có được sử dụng hay thu hồi hay chưa
                const string sqlCheckRefreshTokenIsUsed = @"
                SELECT COUNT(token) 
                FROM refreshtoken 
                WHERE token = @token 
                AND (IsUsed = 1 OR IsRevoked = 1);";
                using (var command = new MySqlCommand(sqlCheckRefreshTokenIsUsed, connection)){
                    command.Parameters.AddWithValue("@token", storedToken);
                    
                    int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                    if(count > 0)
                        return Ok(new ApiRespons{
                            Success = false, 
                            Message = "RefreshToken has been revoked or used"
                        });
                }

                //Check6: AccessToken id == JwtId in RefreshToken
                var jti = tokenInverification.Claims.FirstOrDefault(
                    x => x.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti).Value;
                if(!string.Equals(jti, JwtId))
                    return Ok(new ApiRespons{
                            Success = false, 
                            Message = "Token doesn't match"
                        });

                //>>>7. Cập nhật lại token này sẽ được sử dụng và thu hồi
                const string sqlUpdateUsed = "UPDATE refreshtoken SET IsUsed=1, IsRevoked=1 WHERE token=@token;";
                using (var command = new MySqlCommand(sqlUpdateUsed, connection)){
                    command.Parameters.AddWithValue("@token", storedToken);
                    
                    await command.ExecuteNonQueryAsync();
                }

                //8.Cấp lại token
                const string sqlReissuedToken = @"
                SELECT tk.TaiKhoan, tk.RoleID, tk.BiKhoa, tk.SuDung, tk.MatKhau
                FROM refreshtoken rf INNER JOIN taikhoan tk ON rf.TaiKhoan = tk.TaiKhoan
                WHERE rf.TaiKhoan = @TaiKhoan";
                var loginMedel1 = new LoginModel();
                using (var command = new MySqlCommand(sqlReissuedToken, connection)){
                    command.Parameters.AddWithValue("@TaiKhoan", TaiKhoan);
                    
                    using var reader = await command.ExecuteReaderAsync();
                    if(await reader.ReadAsync()){
                        loginMedel1.account = reader.GetString(reader.GetOrdinal("TaiKhoan"));
                        loginMedel1.Role = reader.GetString(reader.GetOrdinal("Role"));
                        loginMedel1.BiKhoa = reader.GetString(reader.GetOrdinal("BiKhoa"));
                        loginMedel1.SuDung = reader.GetInt32(reader.GetOrdinal("SuDung"));
                        loginMedel1.password = reader.GetString(reader.GetOrdinal("MatKhau"));
                    }
                }
            
                var token = await GenerateToken(loginMedel1);
                return Ok(new
                    ApiRespons{
                        Success = true, 
                        Message = "Làm mới token thành công", 
                        Data = token
                    }
                );

            }catch(Exception ex){
                Console.WriteLine($"Error message:{ex.Message}");
                Console.WriteLine($"Error TargetSite:{ex.TargetSite}");
                Console.WriteLine($"Error StackTrace:{ex.StackTrace}");
                Console.WriteLine($"Error Data:{ex.Data}");
                return BadRequest(new ApiRespons{
                    Success = false, 
                    Message = "Something went wrong"
                });
            }
        }

        //3. Đăng xuất
        [HttpPost("logout")]
        public async Task<IActionResult> LogOut([FromQuery] string sdt){
            //Đăng xuất khỏi xác thực cookie
            await HttpContext.SignOutAsync(scheme: "SecurityBauCuTrucTuyen");

            //Vô hiệu hóa refreshtoken
            await InvalidateRefreshTokenForUser(sdt);

            return Ok(new ApiRespons
            {
                Success = true,
                Message = "Đăng xuất thành công",
                Data = null
            });
        }

    }
}