using BackEnd.src.core.Models;
using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.web_api.DTOs;
using Isopoh.Cryptography.Argon2;
using MySql.Data.MySqlClient;
using log4net;
using log4net.Config;
using log4net.Util;
using BackEnd.src.core.Common;
using BackEnd.src.core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using BackEnd.src.core.Container;

namespace BackEnd.src.infrastructure.Services
{
    public class UserServices: IDisposable, IUserServices
    {        
        private readonly DatabaseContext _context;
        private static readonly ILog _log = LogManager.GetLogger(typeof(UserServices));
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _cache;
        private readonly IToken _token;


        //Khởi tạo
        public UserServices(
            DatabaseContext context,
            IEmailSender emailSender,
            IMemoryCache cache,
            IToken token
        ){
            _context = context;
            _emailSender = emailSender;
            _cache = cache;
            _token = token;
        }

        //Hủy
        public void Dispose() => _context.Dispose();

        //-1. Kiểm tra email có tồn tại không
        public async Task<bool> _CheckEmailExists(string email, MySqlConnection connection){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
            
            const string sql = "SELECT COUNT(Email) FROM nguoidung WHERE Email=@Email;";
            using (var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@Email",email);
                int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                if(count < 1)
                    return false;
            }
            return true;
        }

        //-2. Kiểm tra email có tồn tại không
        public async Task<bool> _CheckPhonNumberExists(string sdt, MySqlConnection connection){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            const string sql = "SELECT COUNT(SDT) FROM nguoidung WHERE SDT=@SDT;";
            using (var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@SDT",sdt);
                int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                if(count < 1)
                    return false;
            }
            return true;
        }

        //-2. Kiểm tra email có tồn tại không
        public async Task<bool> _CheckUserRegistered(string sdt, MySqlConnection connection){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
            
            const string sql = @"
            SELECT hs.TrangThaiDangKy
            FROM hosonguoidung hs 
            JOIN nguoidung nd ON hs.ID_user = nd.ID_user
            WHERE nd.SDT = @SDT;";
            using (var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@SDT",sdt);
                
                using var reader = await command.ExecuteReaderAsync();
                if(await reader.ReadAsync()){
                    string trangthai = reader.GetString(reader.GetOrdinal("TrangThaiDangKy"));
                    
                    if(trangthai == "1")    //Đã đăng ký
                        return true;
                }
            }
            return false;
        }

        //0.Đăng nhập
        public async Task<LoginModel> _Login(LoginModel loginDto){
            using var connection = await _context.Get_MySqlConnection();

            //Lấy số điện thoại, kiểm tra xem có không .Nếu không thì trả về false
            const string CheckSDT = @"
            SELECT tk.TaiKhoan, tk.MatKhau, tk.RoleID, tk.SuDung, nd.Email
            FROM taikhoan tk 
            INNER JOIN nguoidung nd ON nd.SDT = tk.TaiKhoan
            WHERE tk.TaiKhoan = @taikhoan 
            AND EXISTS ( 
            SELECT 1 FROM hosonguoidung hs 
            WHERE hs.ID_user = nd.ID_user 
            AND hs.TrangThaiDangKy = '1')
            AND tk.BiKhoa = '0'
            LIMIT 1;";

            using var command = new MySqlCommand(CheckSDT, connection);
            command.Parameters.AddWithValue("@TaiKhoan", loginDto.account);
            using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            
            if(!await reader.ReadAsync().ConfigureAwait(false)){
                //_log.WarnExt("Đăng nhập thất bại vì không tìm thấy số điện thoại");
                Console.WriteLine("Đăng nhập thất bại vì không tìm thấy số điện thoại");
                return null;
            }

            string HashedPwd = reader.GetString(reader.GetOrdinal("MatKhau")),
                    role = reader.GetInt32(reader.GetOrdinal("RoleID")).ToString(),
                    Email = reader.GetString(reader.GetOrdinal("Email"));

            int SuDung = reader.GetInt32(reader.GetOrdinal("SuDung"));

            //Thực hiện vệc xác thực mật khẩu. Nếu không đúng thì không đăng nhập được
                var isPasswordValid = await Task.Run(() => 
                    Argon2.Verify(HashedPwd, loginDto.password)).ConfigureAwait(false);

                if (!isPasswordValid)
                {
                    _log.WarnExt("Đăng nhập thất bại: Sai mật khẩu");
                    return null;
                }

            //Tạo đối tượng. Rồi trả về đối tượng này
            return new LoginModel{
                account = loginDto.account,
                Email = Email,
                password = HashedPwd,
                Role = role,
                SuDung = SuDung
            };
        }

        //1. Gửi lại mã opt
        public async Task<bool> _ResendOtpAsync(EmailDTO emailDTO){
            
            // --- Kiểm tra xem email có tồn tại không
            using var connection = await _context.Get_MySqlConnection();
            const string sql = "SELECT COUNT(Email) FROM nguoidung WHERE Email=@Email;";
            using (var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@Email",emailDTO.Email);
                int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                if(count < 1)
                    return false;
            }

            // --- Thực hiển gửi mã
            string title = "Mã xác thực đã gửi lại";
            string opt = RandomString.DaySoNgauNhien(6);        //Chuỗi ngẫu nhiên
            EmailOTP emailopt = new EmailOTP();
            var emailBody = emailopt.GenerateOtpEmail(opt);     //Nội dung email
            var cacheKey = $"OTP_{emailDTO.Email}";                      //Đặt Key lưu vào bộ nhớ đệm
            _cache.Set(cacheKey, opt,TimeSpan.FromMinutes(5));  //Key, value và thời gian hết hạn
            await _emailSender.SendEmailAsync(emailDTO.Email, title, emailBody); //Gửi

            return true;
        }

        //2. Gửi mã otp khi đăng nhập 
        public async Task _SendVerificationOTPcodeAfterLogin(string email){
            // --- Thực hiển gửi mã
            string opt = await Task.Run(() => RandomString.DaySoNgauNhien(6)).ConfigureAwait(false);        //Chuỗi ngẫu nhiên
            EmailOTP emailopt = new EmailOTP();
            var emailBody = emailopt.GenerateOtpEmail(opt);     //Nội dung email
            
            //Thiết lập lưu mã otp vào bộ nhớ đệm
            var cacheKey = $"OTP_{email}";                      //Đặt Key lưu vào bộ nhớ đệm
            _cache.Set(cacheKey, opt,TimeSpan.FromMinutes(5));  //Key, value và thời gian hết hạn
            
            //Gửi
            await _emailSender.SendEmailAsync(
                email, 
                "Xác thực mã otp"+":"+opt, 
                emailBody
            ); 
        }

        //3. Xác thực mã otp sau khi đăng nhập
        public async Task<TokenModel> _VerifyOtpCodeAfterLogin(VerifyOtpDto verifyOtpDto){
            var cacheKey = $"OTP_{verifyOtpDto.Email}";

            //Kiểm tra trong bộ nhớ cache có chứa OTP không
            if(_cache.TryGetValue(cacheKey, out string cacheOTP)){
                //Nếu mã otp hợp lệ thì xóa khỏi bộ nhớ đệm
                if(cacheOTP == verifyOtpDto.Otp){
                    _cache.Remove(cacheKey);

                    //Tạo token rồi trả về cho người dùng
                    LoginModel loginModel = new LoginModel();

                    const string sql = @"
                    SELECT tk.TaiKhoan,tk.MatKhau,tk.RoleID,tk.BiKhoa,tk.SuDung, nd.Email 
                    FROM taikhoan tk INNER JOIN nguoidung nd ON nd.SDT = tk.TaiKhoan
                    WHERE nd.Email = @EMAIL";
                    using var connection = await _context.Get_MySqlConnection();
                    using(var command = new MySqlCommand(sql, connection)){
                        command.Parameters.AddWithValue("@EMAIL", verifyOtpDto.Email);
                        using var reader = await command.ExecuteReaderAsync();

                        if(await reader.ReadAsync()){       //Lưu thông tin này lại. Vì thông tin này để tạo ra token
                            loginModel.account = reader.GetString(reader.GetOrdinal("TaiKhoan"));
                            loginModel.Email = reader.GetString(reader.GetOrdinal("Email"));
                            loginModel.password = reader.GetString(reader.GetOrdinal("MatKhau"));
                            loginModel.Role = reader.GetInt32(reader.GetOrdinal("RoleID")).ToString();
                            loginModel.BiKhoa = reader.GetString(reader.GetOrdinal("BiKhoa"));
                            loginModel.SuDung = reader.GetInt32(reader.GetOrdinal("SuDung"));
                        }
                    }
                    var token = await _token.GenerateToken(loginModel);

                    return token;
                }
            }
            return null; //xác nhận mã otp này không hợp lệ
        }

        //4. Gửi mã otp cũng với tiêu đề
        public async Task<bool> _SendOtpCodeWithTitle(string email, string title){
            
            //Check email có tồn tại không
            using var connection = await _context.Get_MySqlConnection();
            bool checkEmailExists = await _CheckEmailExists(email, connection);
            if(!checkEmailExists) return false;

            string opt = RandomString.DaySoNgauNhien(6);        //Chuỗi ngẫu nhiên
            EmailOTP_Verify emailopt = new EmailOTP_Verify();
            var emailBody = emailopt.GenerateOtpEmailVerify(opt);     //Nội dung email
            var cacheKey = $"OTP_{email}";                      //Đặt Key lưu vào bộ nhớ đệm
            _cache.Set(cacheKey, opt,TimeSpan.FromMinutes(5));  //Key, value và thời gian hết hạn
            await _emailSender.SendEmailAsync(email, title+":"+opt, emailBody); //Gửi
            return true;
        }

        //5. Xác thực mã otp
        public async Task<int> _VerifyOtp(VerifyOtpDto verifyOtpDto){
            using var connection = await _context.Get_MySqlConnection();
            
            //Kiểm tra Email có tồn tại không
            bool checkEmailExists = await _CheckEmailExists(verifyOtpDto.Email, connection);
            if(!checkEmailExists)
                return -1; 

            var cacheKey = $"OTP_{verifyOtpDto.Email}";
            
            //Lấy giá trị từ CacheKey
            if(_cache.TryGetValue(cacheKey, out string cacheOTP)){
                if(cacheOTP == verifyOtpDto.Otp){
                    _cache.Remove(cacheKey);
                    return 1;
                }
            }
            return 0;   //Mã xác minh không hợp lệ
        }
        
        //6. Xác thực mã otp khi quên mật khẩu
        public async Task<bool> _SendOtpCodeWhenForgotPwd(EmailDTO emailDTO){
            using var connection = await _context.Get_MySqlConnection();

            //Kiểm tra xem email có tồn tại không
            bool CheckExistsEmail = await _CheckEmailExists(emailDTO.Email, connection);
            if(!CheckExistsEmail)
                return false;

            //Gửi mã opt cùng với tiêu đề đặt lại mật khẩu
            string title = "Đặt lại mật khẩu";
            string opt = RandomString.DaySoNgauNhien(6);        //Chuỗi ngẫu nhiên
            EmailOTP_password emailopt = new EmailOTP_password();
            var emailBody = emailopt.GenerateOtpEmail(opt);     //Nội dung email
            var cacheKey = $"OTP_{emailDTO.Email}";                      //Đặt Key lưu vào bộ nhớ đệm
            _cache.Set(cacheKey, opt,TimeSpan.FromMinutes(5));  //Key, value và thời gian hết hạn
            await _emailSender.SendEmailAsync(emailDTO.Email, title+":"+opt, emailBody); //Gửi
            return true;
        }

        //7.Đặt lại mật khẩu dựa trên email người dùng
        public async Task<bool> _ResetUserPassword(string email, string newPwd){
            using var connection = await _context.Get_MySqlConnection();
            const string sql = @"
            UPDATE taikhoan tk
            SET tk.MatKhau = @newPwd
            WHERE EXISTS(
            SELECT 1
            FROM nguoidung nd
            WHERE tk.TaiKhoan = nd.SDT
            AND nd.Email = @email);";

            //Băm mật khẩu
            newPwd = Argon2.Hash(newPwd);
            using (var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@newPwd", newPwd);
                command.Parameters.AddWithValue("@email", email);
                
                int rowAffected = await command.ExecuteNonQueryAsync();
                if(rowAffected < 0) return false;
            }
            return true;
        }

        //8.Gửi mã OTP khi người dùng gửi bỏ phiếu
        public async Task<bool> _SendOtpCodeAfterUserVoted(EmailDTO emailDTO, MySqlConnection connection){
            //Kiểm tra xem email có tồn tại không
            bool CheckExistsEmail = await _CheckEmailExists(emailDTO.Email, connection);
            if(!CheckExistsEmail)
                return false;

            //Gửi mã opt cùng với tiêu đề đặt lại mật khẩu
            await _SendOtpCodeWithTitle(emailDTO.Email,"Xác thực bỏ phiếu");
            return true;
        }

        //9. Kiểm tra xem email có tồn tại không
        public async Task<bool> _CheckUserEmail(string email){
            using var connection = await _context.Get_MySqlConnection();
            //Kiểm tra xem email có tồn tại không
            bool CheckExistsEmail = await _CheckEmailExists(email, connection);
            if(!CheckExistsEmail)
                return false;

            //Gửi mã otp đến email trên
            string title = "Đặt lại mật khẩu";
            string opt = RandomString.DaySoNgauNhien(6);        //Chuỗi ngẫu nhiên
            EmailOTP_Verify emailopt = new EmailOTP_Verify();
            var emailBody = emailopt.GenerateOtpEmailVerify(opt);     //Nội dung email
            var cacheKey = $"OTP_{email}";                      //Đặt Key lưu vào bộ nhớ đệm
            _cache.Set(cacheKey, opt,TimeSpan.FromMinutes(5));  //Key, value và thời gian hết hạn
            await _emailSender.SendEmailAsync(email, title+":"+opt, emailBody); //Gửi

            return true;
        }

        //Xử lý ngươi dùng đăng ký. Gồm: Đặt mật khẩu người dung dựa trên sdt và  đặt lại hồ sơ người dùng là cử tri này đã đăng ký
        public async Task<int> _handleUserRegister(string sdt, string newPwd) {
            try{
                using var connection = await _context.Get_MySqlConnection();

                //Kiểm tra xem sdt người dùng có tồn tại không
                bool CheckExistsPhoneNumber= await _CheckPhonNumberExists(sdt, connection);
                if(!CheckExistsPhoneNumber)
                    return 0;

                //Kiểm tra người dùng đăng ký chưa. Nếu rồi thì báo lỗi
                bool checkUserRegistered = await _CheckUserRegistered(sdt, connection);
                if(checkUserRegistered == true)
                    return -1;

                //Băm mật khẩu
                newPwd = Argon2.Hash(newPwd);

                //Cập nhật lại mật khẩu
                const string setPwd = @"
                UPDATE taikhoan 
                SET MatKhau = @MatKhau
                WHERE TaiKhoan = @SDT;";

                //Cập nhật lại hò sơ người dùng dựa trên sdt
                const string updateStatus = @"
                UPDATE hosonguoidung hs
                JOIN nguoidung nd ON hs.ID_user = nd.ID_user
                SET hs.TrangThaiDangKy = '1'
                WHERE nd.SDT = @SDT;";

                using (var command = new MySqlCommand(setPwd+updateStatus ,connection)){
                    command.Parameters.AddWithValue("@MatKhau", newPwd);
                    command.Parameters.AddWithValue("@SDT", sdt);
                    await command.ExecuteNonQueryAsync();
                    
                    return 1;
                }
            }catch(MySqlException ex){
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Error Code: {ex.Code}");
                Console.WriteLine($"Error Source: {ex.Source}");
                Console.WriteLine($"Error HResult: {ex.HResult}");
                throw;
            }
            catch(Exception ex){
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Error Source: {ex.Source}");
                Console.WriteLine($"Error StackTrace: {ex.StackTrace}");
                Console.WriteLine($"Error TargetSite: {ex.TargetSite}");
                Console.WriteLine($"Error HResult: {ex.HResult}");
                Console.WriteLine($"Error InnerException: {ex.InnerException}");
                throw;
            }
        }
    }
    
}