using BackEnd.src.core.Models;
using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.web_api.DTOs;
using Isopoh.Cryptography.Argon2;
using MySql.Data.MySqlClient;
using log4net;
using log4net.Config;
using log4net.Util;

namespace BackEnd.src.infrastructure.Services
{
    public class UserServices: IDisposable, IUserServices
    {        
        private readonly DatabaseContext _context;
        private static readonly ILog _log = LogManager.GetLogger(typeof(UserServices));
        
        //Khởi tạo
        public UserServices(DatabaseContext context){
            _context = context;
        }

        //Hủy
        public void Dispose() => _context.Dispose();

        //0.Đăng nhập
        public async Task<LoginModel> _Login(LoginModel loginDto){
            using var connection = await _context.Get_MySqlConnection();

            //Lấy số điện thoại, kiểm tra xem có không .Nếu không thì trả về false
            const string CheckSDT = @"
            SELECT TaiKhoan,MatKhau,RoleID,BiKhoa,SuDung 
            FROM taikhoan 
            WHERE TaiKhoan = @SoDienThoai;";

            using var command = new MySqlCommand(CheckSDT, connection);
            command.Parameters.AddWithValue("@SoDienThoai", loginDto.account);
            using var reader = await command.ExecuteReaderAsync();
            
            if(!await reader.ReadAsync()){
                _log.WarnExt("Đăng nhập thất bại vì không tìm thấy số điện thoại");
                return null;
            }

            string HashedPwd = reader.GetString(reader.GetOrdinal("MatKhau")),
                    role = reader.GetInt32(reader.GetOrdinal("RoleID")).ToString(),
                    BiKhoa = reader.GetString(reader.GetOrdinal("BiKhoa"));
            int SuDung = reader.GetInt32(reader.GetOrdinal("SuDung"));

            if(string.IsNullOrEmpty(role) || string.IsNullOrEmpty(HashedPwd)){
                _log.WarnExt("Đăng nhập thất bại vì không tìm thấy số điện thoại");
                return null;
            }

            //Thực hiện vệc xác thực mật khẩu. Nếu không đúng thì không đăng nhập được
            if(!Argon2.Verify(HashedPwd, loginDto.password)){
                _log.WarnExt("Đăng nhập thất bại: Sai mật khẩu");
                return null;
            }

            //Tạo đối tượng. Rồi trả về đối tượng này
            return new LoginModel{
                account = loginDto.account,
                password = HashedPwd,
                Role = role,
                BiKhoa = BiKhoa,
                SuDung = SuDung
            };
        }
    }
    
}