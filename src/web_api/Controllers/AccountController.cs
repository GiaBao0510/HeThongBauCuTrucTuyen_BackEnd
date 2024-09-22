using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackEnd.src.core.Models;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.web_api.DTOs;
using BackEnd.src.core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;
using log4net;
using BackEnd.src.infrastructure.DataAccess.Context;
using MySql.Data.MySqlClient;
using System.Web;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController: ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly AppSetting _appSettings;
        private readonly DatabaseContext _context;
        private readonly IMemoryCache _cache;
        private readonly IEmailSender _emailSender;
        private readonly IToken _token;
        private readonly ILoginHistoryRepository _loginHistory;
        private static readonly ILog _log = LogManager.GetLogger(typeof(AccountController));

        //Khởi tạo
        public AccountController(
            IUserServices userServices,
            IOptionsMonitor<AppSetting> optionsMonitor,
            DatabaseContext context,
            IMemoryCache cache,
            IEmailSender emailSender,
            IToken token,
            ILoginHistoryRepository loginHistory
        ){
            _userServices = userServices;
            _context = context;    
            _appSettings = optionsMonitor.CurrentValue;     //Lấy giá trị đã định nghĩa trong file appsettings
            _cache = cache;
            _emailSender = emailSender;
            _token = token;
            _loginHistory = loginHistory;
        }

        //0.Chuyển số thực sang thời gian
        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate){
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();
        }

        //1.Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginModel loginModel){
            try{
                //1.Nếu không điển cũng quăng ra lỗi luôn
                if(string.IsNullOrEmpty(loginModel.account) || string.IsNullOrEmpty(loginModel.password))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền đầy đủ thông tin đăng nhập."});

                var result = await _userServices._Login(loginModel);
                if(result == null)          //Đăng nhập thất bại
                    return BadRequest(
                        new ApiRespons{Success = false, Message = "Tài khoản hoặc mật khẩu không chính xác."}
                    );

                //2.Gửi mã otp xác nhận
                await _userServices._SendVerificationOTPcodeAfterLogin(result.Email);

                return Ok(new {
                        Success = true,
                        VaiTro =  result.Role,
                        Message = "Tài khoản hợp lệ. Vui lòng xác nhận mã otp chúng tôi gửi trên mail để hoàn thành bước đăng nhập."
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
                        (SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    
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
            
                var token = await _token.GenerateToken(loginMedel1);
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
            await _token.InvalidateRefreshTokenForUser(sdt);

            return Ok(new ApiRespons
            {
                Success = true,
                Message = "Đăng xuất thành công",
                Data = null
            });
        }

        //4.Xác thực mã otp sau khi đăng nhập
        [HttpPost("verify-otp-after-login")]
        public async Task<IActionResult> VerifyOtpCodeAfterLogin([FromBody]VerifyOtpDto verify){
            try{
                if(string.IsNullOrEmpty(verify.Email) || string.IsNullOrEmpty(verify.Otp) || string.IsNullOrEmpty(verify.Phone))
                    return BadRequest(new ApiRespons{ Success = false, Message = "Email, số điện thoại và mã otp không được để trống"});
                
                var result = await _userServices._VerifyOtpCodeAfterLogin(verify);
                if(result == null)
                    return BadRequest(new ApiRespons{ Success = false, Message = "Mã otp xác nhận không chính xác"});
                
                //Lưu lịch sử đăng nhập
                var clientIpAddress = HttpContext.Connection.RemoteIpAddress;
                string ipv4 = clientIpAddress!=null? clientIpAddress.MapToIPv4().ToString():"null";     //Lấy địan chỉ IPv4
                bool checkSaveLogin = await _loginHistory._SaveLoginHistory(ipv4,verify.Phone);
                if(checkSaveLogin == false)
                return StatusCode(500, new ApiRespons{
                    Success = false, 
                    Message = "Lưu lịch sử đăng nhập thất bại"
                });
                
                return Ok(new ApiRespons{
                    Success = true, 
                    Message = "Mã OTP xác nhận thành công",
                    Data = result
                });
            }catch(Exception ex){
                Console.WriteLine($"Error message:{ex.Message}");
                Console.WriteLine($"Error TargetSite:{ex.TargetSite}");
                Console.WriteLine($"Error StackTrace:{ex.StackTrace}");
                Console.WriteLine($"Error Data:{ex.Data}");
                return StatusCode(500,new ApiRespons{
                    Success = false, 
                    Message = "Lỗi khi xác nhận mã otp. Sau khi đăng  nhập"
                });
            }
        }

        //5. Gửi lại mã OTP
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtpAsync([FromBody] EmailDTO emailDTO){
            try{
                if(string.IsNullOrEmpty(emailDTO.Email))
                    return StatusCode(400, new ApiRespons{ Success = false, Message = "Vui lòng điền Email để xác thực"});
                var result = await _userServices._ResendOtpAsync(emailDTO);
                if(result == false){
                    return StatusCode(400, new ApiRespons{ Success = false, Message = "Email này không tồn tại"});
                }

                return Ok(new ApiRespons{
                    Success = true, 
                    Message = "Gửi mã OTP thành công"
                });
            }catch(Exception ex){
                Console.WriteLine($"Error message:{ex.Message}");
                Console.WriteLine($"Error TargetSite:{ex.TargetSite}");
                Console.WriteLine($"Error StackTrace:{ex.StackTrace}");
                Console.WriteLine($"Error Data:{ex.Data}");
                return StatusCode(500,new ApiRespons{
                    Success = false, 
                    Message = "Lỗi khi gửi mã otp"
                });
            }
        }

        //6. Xác nhận mã OTP
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody]VerifyOtpDto verifyOtpDto){
            try{
                if(string.IsNullOrEmpty(verifyOtpDto.Email) || string.IsNullOrEmpty(verifyOtpDto.Otp))
                    return BadRequest(new ApiRespons{ Success = false, Message = "Email và mã otp không được để trống"});
                
                int result = await _userServices._VerifyOtp(verifyOtpDto);
                if(result <= 0){
                    string errorMessage = result switch{
                        0 => "Mã xác minh Otp không hợp lệ",
                        -1 => "Email không tồn tại",
                        _ => "Lỗi không xác định"
                    };

                    return BadRequest(new ApiRespons{
                        Success = false, 
                        Message = errorMessage
                    });
                }

                 return Ok(new ApiRespons{
                    Success = true, 
                    Message = "Mã OTP xác nhận thành công",
                    Data = result
                });
            }catch(Exception ex){
                Console.WriteLine($"Error message:{ex.Message}");
                Console.WriteLine($"Error TargetSite:{ex.TargetSite}");
                Console.WriteLine($"Error StackTrace:{ex.StackTrace}");
                Console.WriteLine($"Error Data:{ex.Data}");
                return StatusCode(500,new ApiRespons{
                    Success = false, 
                    Message = "Lỗi khi xác nhận mã otp"
                });
            }
        }

        //7. Gửi mã otp khi muốn đặt lại mậu khẩu
        [HttpPost("send-otp-forgotpassword")]
        public async Task<IActionResult> SendOtpCodeWhenForgotPwd([FromBody]EmailDTO emailDTO){
            try{
                if(string.IsNullOrEmpty(emailDTO.Email))
                    return BadRequest(new ApiRespons{ Success = false, Message = "Vui lòng nhập Email để thay đổi mật khẩu"});
                
                bool result = await _userServices._SendOtpCodeWhenForgotPwd(emailDTO);
                if(result == false)
                    return BadRequest(new ApiRespons{ Success = false, Message = "Email người dùng không tồn tại"});
                
                return Ok(new ApiRespons{
                    Success = true, 
                    Message = "Xác thực hợp lệ. Đến phần thay đổi mật khẩu",
                    Data = result
                });
            }catch(Exception ex){
                Console.WriteLine($"Error message:{ex.Message}");
                Console.WriteLine($"Error TargetSite:{ex.TargetSite}");
                Console.WriteLine($"Error StackTrace:{ex.StackTrace}");
                Console.WriteLine($"Error Data:{ex.Data}");
                return StatusCode(500,new ApiRespons{
                    Success = false, 
                    Message = "Lỗi khi gửi mã otp"
                });
            }
        }

        //8. Đặt lại mật khẩu
        [HttpPost("reset-pwd")]
        public async Task<IActionResult> ResetUserPassword([FromBody] VerifyOtpDto verifyOtpDto){
            try{
                if(string.IsNullOrEmpty(verifyOtpDto.Email) || string.IsNullOrEmpty(verifyOtpDto.NewPwd))
                    return BadRequest(new ApiRespons{ Success = false, Message = "Email và mật khẩu mới không được để trống"});
                
                var result = await _userServices._ResetUserPassword(verifyOtpDto.Email, verifyOtpDto.NewPwd);
                if(result == false)
                    return BadRequest(new ApiRespons{ Success = false, Message = "Không tìm thấy Email của người dùng để đặt lại mật khẩu"});
                
                return Ok(new ApiRespons{
                    Success = true, 
                    Message = "Đặt lại mật khẩu thành công",
                    Data = result
                });

            }catch(Exception ex){
                Console.WriteLine($"Error message:{ex.Message}");
                Console.WriteLine($"Error TargetSite:{ex.TargetSite}");
                Console.WriteLine($"Error StackTrace:{ex.StackTrace}");
                Console.WriteLine($"Error Data:{ex.Data}");
                return StatusCode(500,new ApiRespons{
                    Success = false, 
                    Message = "Lỗi khi đặt lại mã cho người dùng"
                });
            }
        }

        //9.Gửi mã Otp trước khi người dùng gửi phiếu bầu
        [HttpPost("send-otp-before-sending")]
        public async Task<IActionResult> SendOtpBeforeApply([FromBody] EmailDTO emailDTO){
            try{
                if(string.IsNullOrEmpty(emailDTO.Email))
                    return StatusCode(400, new ApiRespons{ Success = false, Message = "Vui lòng điền Email gửi thông tin xác thực"});
                
                var result = await _userServices._SendOtpCodeWithTitle(emailDTO.Email, "Xác thực bỏ phiếu");
                if(result == false)
                    return StatusCode(400, new ApiRespons{ Success = false, Message = "Email người dùng không tồn tại"});

                 return Ok(new ApiRespons{
                    Success = true, 
                    Message = "Xác thực thông tin người dùng thành công",
                    Data = result
                });
            }catch(Exception ex){
                Console.WriteLine($"Error message:{ex.Message}");
                Console.WriteLine($"Error TargetSite:{ex.TargetSite}");
                Console.WriteLine($"Error StackTrace:{ex.StackTrace}");
                Console.WriteLine($"Error Data:{ex.Data}");
                return StatusCode(500,new ApiRespons{
                    Success = false, 
                    Message = "Lỗi khi gửi mã otp"
                });
            }
        }

        //10. Gửi mã Otp sau khi người dùng cử tri đăng ký tài khoản
        [HttpPost("send-otp-after-registration")]
        public async Task<IActionResult> SendOtpÀterRegistration([FromBody] EmailDTO emailDTO){
            try{
                if(string.IsNullOrEmpty(emailDTO.Email))
                    return StatusCode(400, new ApiRespons{ Success = false, Message = "Vui lòng điền Email gửi thông tin xác thực"});
                
                var result = await _userServices._SendOtpCodeWithTitle(emailDTO.Email, "Xác thực đăng ký");
                if(result == false)
                    return StatusCode(400, new ApiRespons{ Success = false, Message = "Email người dùng không tồn tại"});

                 return Ok(new ApiRespons{
                    Success = true, 
                    Message = "Xác thực thông tin người dùng thành công",
                    Data = result
                });
            }catch(Exception ex){
                Console.WriteLine($"Error message:{ex.Message}");
                Console.WriteLine($"Error TargetSite:{ex.TargetSite}");
                Console.WriteLine($"Error StackTrace:{ex.StackTrace}");
                Console.WriteLine($"Error Data:{ex.Data}");
                return StatusCode(500,new ApiRespons{
                    Success = false, 
                    Message = "Lỗi khi gửi mã otp"
                });
            }
        }

        //11. Kiểm tra xem token hợp lệ không
        [HttpGet("check-logined")]
        public async Task<IActionResult> CheckLogined([FromHeader] string token){
            try{
                if(string.IsNullOrEmpty(token))
                    return StatusCode(400, new ApiRespons{ Success = false, Message = "Vui lòng điền token để kiểm tra"});
                
                var result = _token._CheckLogined(token);
                if(result <= 0){
                    string errorMessage = result switch{
                        0 => "Lỗi. Thuật toán mã hóa token không hợp lệ ",
                        -1 => "Token đã hết hạn hoặc không tồn tại",
                        _ => "Lỗi không xác định"
                    };
                    int status = result switch{
                        0 => 400,
                        -1 => 400,
                        _ => 500
                    };
                    return StatusCode(status, new ApiRespons{
                        Success = false, 
                        Message = errorMessage
                    });
                }

                return Ok(new ApiRespons{
                    Success = true, 
                    Message = "Token hợp lệ",
                    Data = result
                });
            }catch(Exception ex){
                Console.WriteLine($"Error message:{ex.Message}");
                Console.WriteLine($"Error TargetSite:{ex.TargetSite}");
                Console.WriteLine($"Error StackTrace:{ex.StackTrace}");
                Console.WriteLine($"Error Data:{ex.Data}");
                return StatusCode(500,new ApiRespons{
                    Success = false, 
                    Message = "Lỗi khi xác thực token"
                });
            }
        }


    }
}