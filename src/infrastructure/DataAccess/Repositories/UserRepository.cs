using BackEnd.src.core.Entities;
using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.web_api.DTOs;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using MySql.Data.MySqlClient;
using BackEnd.src.core.Common;
using Isopoh.Cryptography.Argon2;
using BackEnd.src.infrastructure.Services;
using Microsoft.AspNetCore.Http;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class UserRepository : IDisposable, IUserRepository
    {
        private readonly DatabaseContext _context;
        private readonly CloudinaryService _cloudinaryService;

        //Khởi tạo
        public UserRepository(DatabaseContext context, CloudinaryService cloudinaryService){
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        //hủy
        public void Dispose() => _context.Dispose();

        //0.Kiểm tra email có trùng không
        public async Task<int> CheckEmailAlreadyExits(string email, MySqlConnection connection){
            try{
                //Nếu đầu vào rỗng thì trả về false
                if(string.IsNullOrEmpty(email)) return 1;

                const string sql_check = "SELECT COUNT(Email) FROM nguoidung WHERE Email = @Email;";
                using(var Command = new MySqlCommand(sql_check, connection)){
                    Command.Parameters.AddWithValue("@Email",email);
                    return Convert.ToInt32(await Command.ExecuteScalarAsync());
                }
            }catch(MySqlException ex){
                Console.WriteLine($"Lỗi tại kiểm tra email trùng trong MYSQL");
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

        //1.Kiểm tra xem số điện thoại có bị trùng không
        public async Task<int> CheckPhoneNumberAlreadyExits(string sdt, MySqlConnection connection){
            try{
                const string sql_check = "SELECT COUNT(sdt) FROM nguoidung WHERE sdt = @sdt;";
                using(var Command = new MySqlCommand(sql_check, connection)){
                    Command.Parameters.AddWithValue("@sdt",sdt);
                    return Convert.ToInt32(await Command.ExecuteScalarAsync());
                }
            }catch(MySqlException ex){
                Console.WriteLine($"Lỗi tại kiểm tra số điện thoại trùng trong MYSQL");
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

        //2. Kiểm tra xem số căn cước có bị trùng không
        public async Task<int> CheckCitizenIdentificationAlreadyExits(string cccd, MySqlConnection connection){
            try{
                const string sql_check = "SELECT COUNT(cccd) FROM nguoidung WHERE cccd = @cccd;";
                using(var Command = new MySqlCommand(sql_check, connection)){
                    Command.Parameters.AddWithValue("@cccd",cccd);
                    return Convert.ToInt32(await Command.ExecuteScalarAsync());
                }
            }catch(MySqlException ex){
                Console.WriteLine($"Lỗi tại kiểm tra số căn cước trùng trong MYSQL");
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

        //2.5 lấy thuộc tính cụ thể của người dùng thông qua ID người dùng
        public async Task<string> _GetUserProperties(string ID_user, string attribute, MySqlConnection connection){
            try{
                string result = null;
                string sql = $"SELECT {attribute} FROM nguoidung WHERE ID_user = @ID_user";
                using (var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ID_user", ID_user);

                    using var reader = await command.ExecuteReaderAsync();
                    if(await reader.ReadAsync())
                        result = reader.GetString(reader.GetOrdinal($"{attribute}"));
                }

                return result;
            }catch(MySqlException ex){
                Console.WriteLine($"Lỗi tại lấy thuộc tính cụ thể của người dùng trong MYSQL");
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

        //3. KIểm tra vai trò người dùng có tồn tại không
        public async Task<bool> CheckRoleAlreadyExits(int role, MySqlConnection connection){
            try{
                const string sql_check = "SELECT COUNT(RoleID) FROM vaitro WHERE RoleID = @RoleID;";
                using(var Command = new MySqlCommand(sql_check, connection)){
                    Command.Parameters.AddWithValue("@RoleID",role);
                    int count = Convert.ToInt32(await Command.ExecuteScalarAsync());
                    if(count < 1)
                        return false;       //Không tồn tại
                }
                return true;
            }catch(MySqlException ex){
                Console.WriteLine($"Lỗi tại kiểm tra vai trò người dùng trong MYSQL");
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

        //4. Kiểm tra thông tin trùng lặp
        public async Task<int> CheckForDuplicateUserInformation(int N,UserDto user, MySqlConnection connection){
            if(await CheckPhoneNumberAlreadyExits(user.SDT, connection) > N) return 0;
            if(await CheckEmailAlreadyExits(user.Email, connection) > N) return -1;
            //if(await CheckCitizenIdentificationAlreadyExits(user.CCCD, connection) > N) return -2;
            if(await CheckRoleAlreadyExits(user.RoleID, connection) == false) return -3;
            return 1;
        }

        //5. Kiểm tra xem có thông tin nào bỏ trống không, nếu có thì trả về false
        public bool CheckUserInformationIsNotEmpty(UserDto user){
            if(user.HoTen == null ||user.GioiTinh == null ||user.NgaySinh == null ||user.DiaChiLienLac == null ||
            user.Email == null ||user.SDT == null ||user.ID_DanToc == null || user.RoleID == null)
                return false;
            return true;
        }

        //6.Liệt kê all
        public async Task<List<Users>> _GetListOfUsers(){
            var list = new List<Users>();
            using var connection = await _context.Get_MySqlConnection();
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            using var command = new MySqlCommand("SELECT * FROM nguoidung", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new Users{
                    ID_user = reader.GetString(reader.GetOrdinal("ID_user")),
                    HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                    GioiTinh = reader.GetString(reader.GetOrdinal("GioiTinh")),
                    NgaySinh = reader.GetDateTime(reader.GetOrdinal("NgaySinh")),
                    DiaChiLienLac = reader.GetString(reader.GetOrdinal("DiaChiLienLac")),
                    CCCD = reader.GetString(reader.GetOrdinal("CCCD")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    SDT = reader.GetString(reader.GetOrdinal("SDT")),
                    HinhAnh = reader.GetString(reader.GetOrdinal("HinhAnh")),
                    ID_DanToc = reader.GetInt32(reader.GetOrdinal("ID_DanToc")),
                    RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                    PublicID = reader.GetString(reader.GetOrdinal("PublicID"))
                });
            }
            return list;
        }

        //7. Lấy thông tin người dùng theo vai trò
        public async Task<List<Users>> _GetListOfUsersWithRole(int roleID){
            var list = new List<Users>();
            using var connection = await _context.Get_MySqlConnection();
            const string sql = "SELECT * FROM nguoidung WHERE RoleID = @RoleID";
            using var command = new MySqlCommand(sql,connection);
            command.Parameters.AddWithValue("@RoleID",roleID);

            using var reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync()){
                list.Add(new Users{
                    ID_user = reader.GetString(reader.GetOrdinal("ID_user")),
                    HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                    GioiTinh = reader.GetString(reader.GetOrdinal("GioiTinh")),
                    NgaySinh = reader.GetDateTime(reader.GetOrdinal("NgaySinh")),
                    DiaChiLienLac = reader.GetString(reader.GetOrdinal("DiaChiLienLac")),
                    CCCD = reader.GetString(reader.GetOrdinal("CCCD")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    SDT = reader.GetString(reader.GetOrdinal("SDT")),
                    HinhAnh = reader.GetString(reader.GetOrdinal("HinhAnh")),
                    ID_DanToc = reader.GetInt32(reader.GetOrdinal("ID_DanToc")),
                    RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                    PublicID = reader.GetString(reader.GetOrdinal("PublicID"))
                });
            }
            return list;
        }

        //8.Thêm người dùng
        public async Task<int> _AddUser(UserDto user ,IFormFile fileAnh){
            using var connection = await  _context.Get_MySqlConnection();

            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
            
            //Sử dụng transaction để đảm bảo các thao tác đến CSDL được thực hiện trên cùng transaction
            using var transaction = await connection.BeginTransactionAsync();
            
            try{
                //Kiểm tra đầu vào là không được null và giá trị hợp lệ
                if(CheckUserInformationIsNotEmpty(user) == false)
                    return -4;

                //Kiểm tra điều kiện đầu vào sao cho SDT, email, cccd không được trùng nhau mới được phép thêm vào
                int KiemTraTrungNhau = await CheckForDuplicateUserInformation(0, user, connection);
                if(KiemTraTrungNhau <= 0)
                    return KiemTraTrungNhau;

                //Lấy ID tự động
                string ID_user = RandomString.CreateID_User();

                //Mã hóa thông tin quan trọng
                string hashedPwd = Argon2.Hash(user.MatKhau);

                //Kiểm tra xem nếu ảnh có tồn tại thì lưu thông tin
                if(fileAnh != null && fileAnh.Length > 0){
                    //Đưa file ảnh lên cloudinary
                    var uploadResult = await _cloudinaryService.UploadImage(fileAnh);
                    user.PublicID = uploadResult.PublicId;
                    user.HinhAnh = uploadResult.Url.ToString();
                }

                //Thêm thông tin người dùng - tài khoản
                string Input = @"INSERT INTO nguoidung(ID_user,HoTen,GioiTinh,NgaySinh,DiaChiLienLac,CCCD,SDT,Email,HinhAnh,PublicID,RoleID,ID_DanToc) 
                VALUES(@ID_user,@HoTen,@GioiTinh,@NgaySinh,@DiaChiLienLac,@CCCD,@SDT,@Email,@HinhAnh,@PublicID,@RoleID,@ID_DanToc);";
                string inputAccount = @"INSERT INTO taikhoan(TaiKhoan,MatKhau,BiKhoa,LyDoKhoa,NgayTao,SuDung,RoleID)
                VALUES(@TaiKhoan,@MatKhau,@BiKhoa,@LyDoKhoa,@NgayTao,@SuDung,@RoleID);";

                using(var command = new MySqlCommand($"{Input} {inputAccount}", connection, transaction)){
                    command.Parameters.AddWithValue("@ID_user", ID_user);
                    command.Parameters.AddWithValue("@HoTen", user.HoTen);
                    command.Parameters.AddWithValue("@GioiTinh", user.GioiTinh);
                    command.Parameters.AddWithValue("@NgaySinh", user.NgaySinh);
                    command.Parameters.AddWithValue("@DiaChiLienLac", user.DiaChiLienLac);
                    command.Parameters.AddWithValue("@CCCD", user.CCCD);
                    command.Parameters.AddWithValue("@SDT", user.SDT);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@HinhAnh", user.HinhAnh);
                    command.Parameters.AddWithValue("@PublicID", user.PublicID);
                    command.Parameters.AddWithValue("@RoleID", user.RoleID);
                    command.Parameters.AddWithValue("@ID_DanToc", user.ID_DanToc);
                    command.Parameters.AddWithValue("@TaiKhoan",user.SDT);
                    command.Parameters.AddWithValue("@MatKhau",hashedPwd);
                    command.Parameters.AddWithValue("@BiKhoa",0);
                    command.Parameters.AddWithValue("@LyDoKhoa","null");
                    command.Parameters.AddWithValue("@NgayTao",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@SuDung",1);
                
                    await command.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return 1;
            }catch(Exception){
                await transaction.RollbackAsync();
                throw;
            }
        }

        //9. Lấy thông tin người dùng theo ID
        public async Task<Users> _GetUserBy_ID(string id){
            using var connection = await _context.Get_MySqlConnection();

            const string sql = @"
                SELECT * FROM nguoidung
                WHERE ID_user = @ID_user";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID_user",id);

            using var reader = await command.ExecuteReaderAsync();
            if(await reader.ReadAsync()){
                return new Users{
                    ID_user = reader.GetString(reader.GetOrdinal("ID_user")),
                    HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                    GioiTinh = reader.GetString(reader.GetOrdinal("GioiTinh")),
                    NgaySinh = reader.GetDateTime(reader.GetOrdinal("NgaySinh")),
                    DiaChiLienLac = reader.GetString(reader.GetOrdinal("DiaChiLienLac")),
                    CCCD = reader.GetString(reader.GetOrdinal("CCCD")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    SDT = reader.GetString(reader.GetOrdinal("SDT")),
                    HinhAnh = reader.GetString(reader.GetOrdinal("HinhAnh")),
                    ID_DanToc = reader.GetInt32(reader.GetOrdinal("ID_DanToc")),
                    RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                    PublicID = reader.GetString(reader.GetOrdinal("PublicID"))
                };
            }

            return null;
        }

        //10. Sửa thông tin người dùng do Admin sửa
        public async Task<int> _EditUserBy_ID_ForAdmin(string ID_user, UserDto user){
            using var connection = await _context.Get_MySqlConnection();

            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync(); 

            //Kiểm tra thông tin đầu vào không được trùng
            int KiemTraThongTinTrungNhau = await CheckForDuplicateUserInformation(1, user, connection);
            if(KiemTraThongTinTrungNhau <= 0){
                Console.WriteLine("Thông tin đầu vào trùng nhau");
                return KiemTraThongTinTrungNhau;
            }

            //Kiểm tra đầu vào không được null
            if(CheckUserInformationIsNotEmpty(user) == false){
                Console.WriteLine("Thông tin đầu vào chưa điền đây đủ");
                return -4;
            }

            //Băm lại mật khẩu
            string hashedPwd = Argon2.Hash(user.MatKhau);

            const string sqlNguoiDung = @"
                UPDATE nguoidung 
                SET HoTen = @HoTen, GioiTinh=@GioiTinh, NgaySinh=@NgaySinh,
                    DiaChiLienLac=@DiaChiLienLac, SDT=@SDT, ID_DanToc=@ID_DanToc,
                    Email=@Email, HinhAnh=@HinhAnh, RoleID=@RoleID, PublicID=@PublicID
                WHERE ID_user = @ID_user;";
            
            //Cập nhật lại thông tin cá nhân người dùng
            using(var command = new MySqlCommand(sqlNguoiDung, connection)){
                command.Parameters.AddWithValue("@ID_user", ID_user);
                command.Parameters.AddWithValue("@HoTen", user.HoTen);
                command.Parameters.AddWithValue("@GioiTinh", user.GioiTinh);
                command.Parameters.AddWithValue("@NgaySinh", user.NgaySinh);
                command.Parameters.AddWithValue("@DiaChiLienLac", user.DiaChiLienLac);
                command.Parameters.AddWithValue("@SDT", user.SDT);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@HinhAnh", user.HinhAnh);
                command.Parameters.AddWithValue("@PublicID", user.PublicID);
                command.Parameters.AddWithValue("@RoleID", user.RoleID);
                command.Parameters.AddWithValue("@ID_DanToc", user.ID_DanToc);
            
                int rowAffected = await command.ExecuteNonQueryAsync();
                if(rowAffected < 0) return -5;

                Console.WriteLine("Cập nhật thông tin cá nhân thành công");
            }

            const string sqlTaiKhoan = @"
                UPDATE taikhoan
                SET TaiKhoan=@SDT, MatKhau=@MatKhau, BiKhoa=@BiKhoa,
                    LyDoKhoa=@LyDoKhoa, NgayTao=@NgayTao, SuDung=@SuDung,
                    RoleID=@RoleID
                WHERE TaiKhoan = @TaiKhoan;";
            //Cập nhật tài khoản
            using(var command1 = new MySqlCommand(sqlTaiKhoan, connection)){
                command1.Parameters.AddWithValue("@SDT",user.SDT);
                command1.Parameters.AddWithValue("@TaiKhoan",user.TaiKhoan);
                command1.Parameters.AddWithValue("@MatKhau",hashedPwd);
                command1.Parameters.AddWithValue("@BiKhoa",user.BiKhoa);
                command1.Parameters.AddWithValue("@LyDoKhoa",user.LyDoKhoa);
                command1.Parameters.AddWithValue("@NgayTao",user.NgayTao);
                command1.Parameters.AddWithValue("@RoleID",user.RoleID);
                command1.Parameters.AddWithValue("@SuDung",user.SuDung);
                
                //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
                int rowAffected = await command1.ExecuteNonQueryAsync();
                if(rowAffected < 0){
                    return -6;
                } 
                
                Console.WriteLine("Đã hoàn thành cập nhật thông tin tài khoản");
            }

            return 1;
        }

        //11. Xóa thông tin người dùng theo ID
        public async Task<bool> _DeleteUserBy_ID(string ID){
            using var connection =await _context.Get_MySqlConnection();

            //Tìm số điện thoại theo ID người dùng
            const string sql_SDT = "SELECT SDT,PublicID FROM nguoidung WHERE ID_user = @ID_user";
            string acc = null, publicID = null;
            using(var command0 = new MySqlCommand(sql_SDT,connection)){
                command0.Parameters.AddWithValue("@ID_user",ID);

                //Nếu tìm thấy người dùng
                using var reader = await command0.ExecuteReaderAsync();
                if(await reader.ReadAsync()){
                    acc = reader.GetString(reader.GetOrdinal("SDT"));
                    publicID = reader.GetString(reader.GetOrdinal("PublicID"));
                }
            }

            //Nếu khồn tìm thấy thì trả về false
            if(string.IsNullOrEmpty(acc) || string.IsNullOrEmpty(publicID) ) return false;

            //Xóa ảnh đã lưu trên cloudinary
            await _cloudinaryService.DeleteImage(publicID);

            //xóa tài khoản
            const string deleteAcc = "DELETE FROM taikhoan WHERE taikhoan = @taikhoan";
            using (var command1 = new MySqlCommand(deleteAcc, connection)){
                command1.Parameters.AddWithValue("@taikhoan",acc);

                int rowAffected = await command1.ExecuteNonQueryAsync();
                if(rowAffected < 1) return false;
            }

            //Xóa thông tin người dùng
            const string deleteInfor = "DELETE FROM nguoidung WHERE ID_user = @ID_user";
            using (var command2 = new MySqlCommand(deleteInfor, connection)){
                command2.Parameters.AddWithValue("@ID_user",ID);

                int rowAffected = await command2.ExecuteNonQueryAsync();
                if(rowAffected < 1) return false;
            }
            return true;
        }

        //11. Xóa thông tin người dùng theo ID
        public async Task<bool> _DeleteUserBy_ID(string ID, MySqlConnection connection){
            try{
                //Tìm số điện thoại theo ID người dùng
                const string sql_SDT = "SELECT SDT,PublicID FROM nguoidung WHERE ID_user = @ID_user";
                string acc = null, publicID = null;
                using(var command0 = new MySqlCommand(sql_SDT,connection)){
                    command0.Parameters.AddWithValue("@ID_user",ID);

                    //Nếu tìm thấy người dùng
                    using var reader = await command0.ExecuteReaderAsync();
                    if(await reader.ReadAsync()){
                        acc = reader.GetString(reader.GetOrdinal("SDT"));
                        publicID = reader.GetString(reader.GetOrdinal("PublicID"));
                    }
                }

                //Nếu khồn tìm thấy thì trả về false
                if(string.IsNullOrEmpty(acc) || string.IsNullOrEmpty(publicID) ) return false;

                //Xóa ảnh đã lưu trên cloudinary
                await _cloudinaryService.DeleteImage(publicID);

                //Xóa thông tin người dùng
                const string deleteInfor = "DELETE FROM nguoidung WHERE ID_user = @ID_user";
                using (var command2 = new MySqlCommand(deleteInfor, connection)){
                    command2.Parameters.AddWithValue("@ID_user",ID);

                    int rowAffected = await command2.ExecuteNonQueryAsync();
                    if(rowAffected < 1) return false;
                }
                return true;
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

        //12. Kiểm tra sự tồn tại của người dùng
        public async Task<bool> _CheckUserExists(string ID, MySqlConnection connection){

            //Kiểm tra xem ID người dùng có tồn tại không
            const string sql = "SELECT COUNT(ID_user) FROM nguoidung WHERE ID_user = @ID_user;";
            using(var command = new MySqlCommand(sql,connection )){
                command.Parameters.AddWithValue("@ID_user",ID);
                int  count = Convert.ToInt32(await command.ExecuteScalarAsync());
                
                if(count < 0) return false;
            }
            return true;
        }

        //13. Cập nhập ảnh người dùng thông qua ID người dùng
        public async Task<bool> _EditUserImageByID(string ID, IFormFile file){
            using var connection = await _context.Get_MySqlConnection();

            //Kiểm tra xem ID người dùng có tồn tại không
            bool check = await _CheckUserExists(ID,connection);
            if(!check) return false;

            string publicID = null;

            //Lấy PublicID để cập nhật trên nó
            const string sql_GetPublicID = "SELECT PublicID FROM nguoidung WHERE ID_user = @ID_user;";
            using(var command0 = new MySqlCommand(sql_GetPublicID, connection)){
                command0.Parameters.AddWithValue("@ID_user",ID);

                using var reader = await command0.ExecuteReaderAsync();
                if(await reader.ReadAsync()){
                    publicID = reader.GetString(reader.GetOrdinal("PublicID"));
                }
            }
            
            //Cập nhật lại ảnh trên cloudinary
            var EditImage = await _cloudinaryService.UpdateImage(publicID, file);
            string hinhAnh = EditImage.Url.ToString();
                publicID = EditImage.PublicId;

            //Cập nhật lại ảnh trong sql
            const string sqlUpdate = @"UPDATE nguoidung 
            SET HinhAnh = @HinhAnh, PublicID = @PublicID
            WHERE ID_user = @ID_user";
            using (var command2 = new MySqlCommand(sqlUpdate, connection)){
                command2.Parameters.AddWithValue("@ID_user",ID);
                command2.Parameters.AddWithValue("@HinhAnh",hinhAnh);
                command2.Parameters.AddWithValue("@PublicID",publicID);

                await command2.ExecuteNonQueryAsync();
            }

            return true;
        }

        //14. Sửa pwd theo ID người dùng - admin
        public async Task<bool> _SetUserPassword(string iduser, string newPassword){
            using var connect = await _context.Get_MySqlConnection();
            
            //Lấy SDT dựa trên ID người dùng
            string sdt = await _GetUserProperties(iduser, "SDT", connect);
            if(string.IsNullOrEmpty(sdt)) return false;

            //Băm mật khẩu
            newPassword = Argon2.Hash(newPassword);

            //Truy xuất theo ID người dùng và đổi
            const string sql= $"UPDATE taikhoan SET MatKhau = @MatKhau WHERE Taikhoan = @Taikhoan";
            using(var command = new MySqlCommand(sql, connect)){
                command.Parameters.AddWithValue("@MatKhau", newPassword);
                command.Parameters.AddWithValue("@Taikhoan", sdt);
                
                int count = Convert.ToInt32(await command.ExecuteNonQueryAsync());
                if(count < 0)
                    return false;
            }
            return true;
        }

        //15. Người dùng tự thay đổi mật khẩu
        public async Task<int> _ChangeUserPassword(string iduser, string oldPwd, string newPwd){
            using var connect = await _context.Get_MySqlConnection();
            
            //Lấy SDT dựa trên ID người dùng
            string sdt = await _GetUserProperties(iduser, "SDT", connect);
            if(string.IsNullOrEmpty(sdt)) return 0;

            //Lấy mật khẫu đã băm dựa trên tài khoản
            string HashedPwd = null;
            string sqlGetHashedPwd = "SELECT MatKhau FROM taikhoan WHERE TaiKhoan = @TaiKhoan;";
            using(var command0 = new MySqlCommand(sqlGetHashedPwd, connect)){
                command0.Parameters.AddWithValue("@TaiKhoan", sdt);

                using var reader = await command0.ExecuteReaderAsync();
                if(await reader.ReadAsync())
                    HashedPwd = reader.GetString(reader.GetOrdinal("MatKhau"));
            }

            if(string.IsNullOrEmpty(HashedPwd)) return -1;

            //So sánh mật khẩu cũ đã băm với mật khẩu mới ,nếu khớp thì có thể thay đổi được
            if(Argon2.Verify(HashedPwd, oldPwd) !=  true) return -2;

            //Băm mật khẩu
            newPwd = Argon2.Hash(newPwd);

            //Truy xuất theo ID người dùng và đổi
            const string sql= $"UPDATE taikhoan SET MatKhau = @MatKhau WHERE Taikhoan = @Taikhoan";
            using(var command = new MySqlCommand(sql, connect)){
                command.Parameters.AddWithValue("@MatKhau", newPwd);
                command.Parameters.AddWithValue("@Taikhoan", sdt);
                
                int count = Convert.ToInt32(await command.ExecuteNonQueryAsync());
                if(count < 0)
                    return -3;
            }
            return 1;
        }

        //16. Thêm người dùng có Connection
        public async Task<object> _AddUserWithConnect(UserDto user ,IFormFile fileAnh ,MySqlConnection connection, MySqlTransaction transaction){

            //Kiểm tra trạng thái kết nối trước khi mở 
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
            
            try{
                //Kiểm tra đầu vào là không được null và giá trị hợp lệ
                if(CheckUserInformationIsNotEmpty(user) == false)
                    return -4;

                //Kiểm tra điều kiện đầu vào sao cho SDT, email, cccd không được trùng nhau mới được phép thêm vào
                int KiemTraTrungNhau = await CheckForDuplicateUserInformation(0, user, connection);
                if(KiemTraTrungNhau <= 0)
                    return KiemTraTrungNhau;

                //Lấy ID tự động
                string ID_user = RandomString.CreateID_User();

                //Mã hóa thông tin quan trọng
                string hashedPwd = Argon2.Hash(user.MatKhau);

                //Kiểm tra xem nếu ảnh có tồn tại thì lưu thông tin
                if(fileAnh != null && fileAnh.Length > 0){
                    //Đưa file ảnh lên cloudinary
                    var uploadResult = await _cloudinaryService.UploadImage(fileAnh);
                    user.PublicID = uploadResult.PublicId;
                    user.HinhAnh = uploadResult.Url.ToString();
                }

                //Thêm thông tin người dùng - tài khoản
                string Input = @"INSERT INTO nguoidung(ID_user,HoTen,GioiTinh,NgaySinh,DiaChiLienLac,SDT,Email,HinhAnh,PublicID,RoleID,ID_DanToc) 
                VALUES(@ID_user,@HoTen,@GioiTinh,@NgaySinh,@DiaChiLienLac,@SDT,@Email,@HinhAnh,@PublicID,@RoleID,@ID_DanToc);";
                string inputAccount = @"INSERT INTO taikhoan(TaiKhoan,MatKhau,BiKhoa,LyDoKhoa,NgayTao,SuDung,RoleID)
                VALUES(@TaiKhoan,@MatKhau,@BiKhoa,@LyDoKhoa,@NgayTao,@SuDung,@RoleID);";

                using(var command = new MySqlCommand($"{Input} {inputAccount}", connection, transaction)){
                    command.Parameters.AddWithValue("@ID_user", ID_user);
                    command.Parameters.AddWithValue("@HoTen", user.HoTen);
                    command.Parameters.AddWithValue("@GioiTinh", user.GioiTinh);
                    command.Parameters.AddWithValue("@NgaySinh", user.NgaySinh);
                    command.Parameters.AddWithValue("@DiaChiLienLac", user.DiaChiLienLac);
                    command.Parameters.AddWithValue("@SDT", user.SDT);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@HinhAnh", user.HinhAnh);
                    command.Parameters.AddWithValue("@PublicID", user.PublicID);
                    command.Parameters.AddWithValue("@RoleID", user.RoleID);
                    command.Parameters.AddWithValue("@ID_DanToc", user.ID_DanToc);
                    command.Parameters.AddWithValue("@TaiKhoan",user.SDT);
                    command.Parameters.AddWithValue("@MatKhau",hashedPwd);
                    command.Parameters.AddWithValue("@BiKhoa",0);
                    command.Parameters.AddWithValue("@LyDoKhoa","null");
                    command.Parameters.AddWithValue("@NgayTao",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@SuDung",1);
                
                    await command.ExecuteNonQueryAsync();
                }

                return ID_user;
            }
            catch(MySqlException ex){ 
                Console.WriteLine("Error Mysql Message: " + ex.Message);
                Console.WriteLine("Error Mysql SqlState: " + ex.SqlState);
                Console.WriteLine("Error Mysql TargetSite: " + ex.TargetSite);
                Console.WriteLine("Error Mysql Code: " + ex.Code);
                Console.WriteLine("Error Mysql Source: " + ex.Source);
                Console.WriteLine("Error Mysql Data: " + ex.Data);
                if(transaction.Connection != null)
                    await transaction.RollbackAsync();

                //Xử lý để in lỗi cụ thể
                switch(ex.Number){
                    case 1062:
                        if(ex.Message.Contains("CCCD"))
                            return -2;
                        else if(ex.Message.Contains("SDT"))
                            return 0;
                        else if(ex.Message.Contains("Email"))
                            return -1;
                        else
                            return -100;
                    default:
                        return -100; 
                }
            }catch(Exception ex){
                Console.WriteLine("Error: " + ex.Message);
                await transaction.RollbackAsync();
                throw;
            }
        }

        //17. Thêm người cử tri có Connection
        public async Task<object> _AddVoterWithConnect(UserDto user ,IFormFile fileAnh ,MySqlConnection connection, MySqlTransaction transaction){

            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
            
            try{
                //Kiểm tra đầu vào là không được null và giá trị hợp lệ
                if(_CheckVoterInformationIsNotEmpty(user) == false)
                    return -4;

                //Kiểm tra điều kiện đầu vào sao cho SDT, email, cccd không được trùng nhau mới được phép thêm vào
                int KiemTraTrungNhau = await CheckForDuplicateUserInformation(0, user, connection);
                if(KiemTraTrungNhau <= 0)
                    return KiemTraTrungNhau;

                //Lấy ID tự động
                string ID_user = RandomString.CreateID_User();

                //Kiểm tra xem nếu ảnh có tồn tại thì lưu thông tin
                if(fileAnh != null && fileAnh.Length > 0){
                    //Đưa file ảnh lên cloudinary
                    var uploadResult = await _cloudinaryService.UploadImage(fileAnh);
                    user.PublicID = uploadResult.PublicId;
                    user.HinhAnh = uploadResult.Url.ToString();
                }

                //Đặt mật khẩu mặt định, có băm
                string hashedPwd = Argon2.Hash("88888888");

                //Thêm thông tin người dùng - tài khoản
                string Input = @"INSERT INTO nguoidung(ID_user,HoTen,GioiTinh,NgaySinh,DiaChiLienLac,SDT,HinhAnh,PublicID,RoleID,ID_DanToc,Email) 
                VALUES(@ID_user,@HoTen,@GioiTinh,@NgaySinh,@DiaChiLienLac,@SDT,@HinhAnh,@PublicID,@RoleID,@ID_DanToc,@Email);";
                string inputAccount = @"INSERT INTO taikhoan(TaiKhoan,MatKhau,BiKhoa,LyDoKhoa,NgayTao,SuDung,RoleID)
                VALUES(@TaiKhoan,@MatKhau,@BiKhoa,@LyDoKhoa,@NgayTao,@SuDung,@RoleID);";

                using(var command = new MySqlCommand($"{Input} {inputAccount}", connection, transaction)){
                    command.Parameters.AddWithValue("@ID_user", ID_user);
                    command.Parameters.AddWithValue("@HoTen", user.HoTen);
                    command.Parameters.AddWithValue("@GioiTinh", user.GioiTinh);
                    command.Parameters.AddWithValue("@NgaySinh", user.NgaySinh);
                    command.Parameters.AddWithValue("@DiaChiLienLac", user.DiaChiLienLac);
                    command.Parameters.AddWithValue("@SDT", user.SDT);
                    command.Parameters.AddWithValue("@HinhAnh", user.HinhAnh);
                    command.Parameters.AddWithValue("@PublicID", user.PublicID);
                    command.Parameters.AddWithValue("@RoleID", user.RoleID);
                    command.Parameters.AddWithValue("@ID_DanToc", user.ID_DanToc);
                    command.Parameters.AddWithValue("@TaiKhoan",user.SDT);
                    command.Parameters.AddWithValue("@MatKhau",hashedPwd);     //Để mặc định vậy.  Sau đó cử tri đăng nhập sẽ yêu cầu cử tri đặt lại mật khẩu
                    command.Parameters.AddWithValue("@BiKhoa",0);
                    command.Parameters.AddWithValue("@LyDoKhoa","null");
                    command.Parameters.AddWithValue("@NgayTao",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@SuDung",1);
                    command.Parameters.AddWithValue("@Email",user.Email);
                
                    await command.ExecuteNonQueryAsync();
                }

                return ID_user;
            }
            catch(MySqlException ex){
                
                if(transaction.Connection != null)
                    await transaction.RollbackAsync();

                //Xử lý để in lỗi cụ thể
                switch(ex.Number){
                    case 1062:
                        if(ex.Message.Contains("CCCD"))
                            return -2;
                        else if(ex.Message.Contains("SDT"))
                            return 0;
                        else if(ex.Message.Contains("Email"))
                            return -1;
                        else
                            return -100;
                    default:
                        return -100; 
                }
            }catch(Exception){
                await transaction.RollbackAsync();
                throw;
            }
        }

        //18. Xóa thông tin người dùng theo ID
        public async Task<bool> _DeleteUserBy_ID_withConnection(string ID, MySqlConnection connection){
            try{
                //Kiểm tra kết nối
                if(connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                //Tìm số điện thoại theo ID người dùng
                const string sql_SDT = "SELECT PublicID FROM nguoidung WHERE ID_user = @ID_user";
                string publicID = null;
                using(var command0 = new MySqlCommand(sql_SDT,connection)){
                    command0.Parameters.AddWithValue("@ID_user",ID);

                    //Nếu tìm thấy người dùng
                    using var reader = await command0.ExecuteReaderAsync();
                    if(await reader.ReadAsync()){
                        publicID = reader.GetString(reader.GetOrdinal("PublicID"));
                    }
                }

                //Nếu khồn tìm thấy thì trả về false
                if(string.IsNullOrEmpty(publicID) ) return false;

                //Xóa ảnh đã lưu trên cloudinary
                await _cloudinaryService.DeleteImage(publicID);

                //Xóa thông tin người dùng
                const string deleteInfor = "DELETE FROM nguoidung WHERE ID_user = @ID_user";
                using (var command2 = new MySqlCommand(deleteInfor, connection)){
                    command2.Parameters.AddWithValue("@ID_user",ID);

                    int rowAffected = await command2.ExecuteNonQueryAsync();
                    if(rowAffected < 1) return false;
                }
                return true;
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

        //19.Liệt kê all -account
        public async Task<List<UserDto>> _GetListOfUsersAndAccounts(){
            var list = new List<UserDto>();
            using var connection = await _context.Get_MySqlConnection();
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            using var command = new MySqlCommand("SELECT * FROM nguoidung", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new UserDto{
                    ID_user = reader.GetString(reader.GetOrdinal("ID_user")),
                    HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                    GioiTinh = reader.GetString(reader.GetOrdinal("GioiTinh")),
                    NgaySinh = reader.GetDateTime(reader.GetOrdinal("NgaySinh")).ToString("dd/MM/yyyy HH:mm:ss"),
                    DiaChiLienLac = reader.GetString(reader.GetOrdinal("DiaChiLienLac")),
                    CCCD = reader.GetString(reader.GetOrdinal("CCCD")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    SDT = reader.GetString(reader.GetOrdinal("SDT")),
                    HinhAnh = reader.GetString(reader.GetOrdinal("HinhAnh")),
                    ID_DanToc = reader.GetInt32(reader.GetOrdinal("ID_DanToc")),
                    RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                    PublicID = reader.GetString(reader.GetOrdinal("PublicID")),
                    TaiKhoan = reader.GetString(reader.GetOrdinal("TaiKhoan")),
                    MatKhau = reader.GetString(reader.GetOrdinal("MatKhau")),
                    BiKhoa = reader.GetString(reader.GetOrdinal("BiKhoa")),
                    LyDoKhoa = reader.GetString(reader.GetOrdinal("LyDoKhoa")),
                    NgayTao = reader.GetString(reader.GetOrdinal("NgayTao")),
                    SuDung = reader.GetInt32(reader.GetOrdinal("SuDung"))
                });
            }
            return list;
        }

        //20. Kiểm tra xem có thông tin nào bỏ trống của cử tri không, nếu có thì trả về false
        public bool _CheckVoterInformationIsNotEmpty(UserDto user){
            if(user.HoTen == null ||user.GioiTinh == null ||user.NgaySinh == null ||user.DiaChiLienLac == null ||
            user.Email == null ||user.SDT == null ||user.ID_DanToc == null || user.RoleID == null)
                return false;
            return true;
        }

        //21. Kiểm tra xem thông tin chức vụ có tồn tại không
        public async Task<bool> _CheckPositionExist(int ID_chucvu, MySqlConnection connection, MySqlTransaction transaction){
            try{
                const string sql = "SELECT * FROM chucvu WHERE ID_chucvu = @ID_chucvu";
                using(var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ID_chucvu", ID_chucvu);

                    int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                    if(count < 1)
                        return false;       //Không tồn tại
                }
                return true;
            }catch(Exception){
                await transaction.RollbackAsync();
                return false;
            } 
        }

        //21 - 2. Kiểm tra xem thông tin chức vụ có tồn tại không
        public async Task<bool> _CheckPositionExist(int ID_chucvu, MySqlConnection connection){

            const string sql = "SELECT * FROM chucvu WHERE ID_chucvu = @ID_chucvu";
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_chucvu", ID_chucvu);

                int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                if(count < 1)
                    return false;       //Không tồn tại
            }
            return true;
        }

        //22. kiểm tra xem người dù đã có tài khoản hay chưa
        public async Task<int> _CheckRegisteredUser(string ID){
            using var connect = await _context.Get_MySqlConnection();

            //Kiểm tra xem ID người dùng có tồn tại không
            bool checkUserExists = await _CheckUserExists(ID, connect);
            if(!checkUserExists) return -1;

            string trangThai = "0";

            //Kiểm tra xem người dùng đã đăng ký chưa
            const string sql = "SELECT TrangThaiDangKy FROM hosonguoidung WHERE ID_user =@ID_user;";
            using(var command = new MySqlCommand(sql, connect)){
                command.Parameters.AddWithValue("@ID_user", ID);

                using var reader = await command.ExecuteReaderAsync();
                if(await reader.ReadAsync())
                    trangThai =  reader.GetString(reader.GetOrdinal("TrangThaiDangKy"));
            }
            return Convert.ToInt32(trangThai);
        }

        //23. Người dùng thay đổi mật khẩu dựa trên email
        public async Task<int> _SetPwdBasedOnUserEmail(string Email, string password){
            using var connect = await _context.Get_MySqlConnection();

            //Kiểm tra xem email người dùng có tồn tại không
            int checkUserExists = await CheckEmailAlreadyExits(Email,connect);
                if(checkUserExists <= 0) return -1;

            //Bắm mật khẩu
            password = Argon2.Hash(password);
            
            //set pwd
            const string sql = @"
            UPDATE taikhoan tk 
            JOIN nguoidung nd ON tk.TaiKhoan = nd.SDT
            SET tk.MatKhau = @MatKhau
            WHERE nd.Email=@Email;";
            using (var command = new MySqlCommand(sql, connect)){
                command.Parameters.AddWithValue("@MatKhau", password);
                command.Parameters.AddWithValue("@Email", Email);

                int rowAffected = await command.ExecuteNonQueryAsync();
                return rowAffected;
            }
        }

        //Lấy  thông tin người dùng dựa trên email
        public async Task<PersonalInformationDTO> _GetPersonnalInfomationByEmail(string email){
            using var connection = await _context.Get_MySqlConnection();
            
            const string sql = @"         
            SELECT nd.HoTen, nd.GioiTinh, nd.NgaySinh, nd.ID_user ,
            nd.DiaChiLienLac, nd.Email, nd.SDT, nd.HinhAnh, dt.TenDanToc,
            CASE 
            WHEN tk.RoleID = '5' THEN ct.ID_CuTri
            WHEN tk.RoleID = '2' THEN ucv.ID_ucv
            WHEN tk.RoleID = '8' THEN cb.ID_CanBo
            ELSE NULL
            END AS ID_Object
            FROM nguoidung nd
            JOIN dantoc dt ON dt.ID_DanToc = nd.ID_DanToc
            JOIN taikhoan tk ON tk.TaiKhoan = nd.SDT
            LEFT JOIN cutri ct ON ct.ID_user = nd.ID_user AND tk.RoleID = '5'
            LEFT JOIN ungcuvien ucv ON ucv.ID_user = nd.ID_user AND tk.RoleID = '2'
            LEFT JOIN canbo cb ON cb.ID_user = nd.ID_user AND tk.RoleID = '8'
            WHERE nd.Email = @Email;";

            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@Email", email);
                
                using var reader = await command.ExecuteReaderAsync();
                if(await reader.ReadAsync()){
                    return new PersonalInformationDTO{
                        HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                        GioiTinh = reader.GetString(reader.GetOrdinal("GioiTinh")),
                        NgaySinh = reader.GetDateTime(reader.GetOrdinal("NgaySinh")),
                        DiaChiLienLac = reader.GetString(reader.GetOrdinal("DiaChiLienLac")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        SDT = reader.GetString(reader.GetOrdinal("SDT")),
                        HinhAnh = reader.GetString(reader.GetOrdinal("HinhAnh")),
                        TenDanToc = reader.GetString(reader.GetOrdinal("TenDanToc")),
                        ID_Object = reader.GetString(reader.GetOrdinal("ID_Object")),
                        ID_user = reader.GetString(reader.GetOrdinal("ID_user"))
                    };
                }
            }

            return null;
        }

        //Lấy danh sách kỳ bầu cử mà người dùng có thể tham dự dựa trên sđt người dùng
        public async Task<List<ElectionsDto>> _getListOfElectionsByUserPhone(string sdt){
            try{
               using var connection = await _context.Get_MySqlConnection();
                var list = new List<ElectionsDto>();

                //Kiểm tra xem số điện thoại có tồn tại 
                int checkVoterExists = await CheckPhoneNumberAlreadyExits(sdt, connection);
                if(checkVoterExists == 0) return null;

                //lấy danh sách kỳ bầu cử có mặc cử tri
                const string sql = @"
                SELECT kb.ngayBD, kb.ngayKT, kb.TenKyBauCu, kb.MoTa, dv.TenDonViBauCu,tt.GhiNhan,
                kb.SoLuongToiDaCuTri, kb.SoLuongToiDaUngCuVien, kb.SoLuotBinhChonToiDa,
                dv.ID_DonViBauCu, kb.ID_Cap
                FROM trangthaibaucu tt 
                JOIN cutri ct ON ct.ID_CuTri = tt.ID_CuTri
                JOIN nguoidung nd ON nd.ID_user = ct.ID_user
                JOIN kybaucu kb ON kb.ngayBD = tt.ngayBD
                JOIN donvibaucu dv ON dv.ID_DonViBauCu = tt.ID_DonViBauCu
                WHERE  nd.SDT =  @SDT";
                using (var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@SDT",sdt);

                    using var reader = await command.ExecuteReaderAsync();
                    while(await reader.ReadAsync()){
                        list.Add(new ElectionsDto{ 
                            ngayBD = reader.GetDateTime(reader.GetOrdinal("ngayBD")),
                            ngayKt =  reader.GetDateTime(reader.GetOrdinal("ngayBD")),
                            TenKyBauCu = reader.GetString(reader.GetOrdinal("TenKyBauCu")),
                            Mota = reader.GetString(reader.GetOrdinal("Mota")),
                            GhiNhan = reader.GetString(reader.GetOrdinal("GhiNhan")),
                            TenDonViBauCu = reader.GetString(reader.GetOrdinal("TenDonViBauCu")),
                            ID_Cap = reader.GetInt32(reader.GetOrdinal("ID_Cap")),
                            ID_DonViBauCu = reader.GetInt32(reader.GetOrdinal("ID_DonViBauCu")),
                            SoLuongToiDaCuTri = reader.GetInt32(reader.GetOrdinal("SoLuongToiDaCuTri")),
                            SoLuongToiDaUngCuVien = reader.GetInt32(reader.GetOrdinal("SoLuongToiDaUngCuVien")),
                            SoLuotBinhChonToiDa = reader.GetInt32(reader.GetOrdinal("SoLuotBinhChonToiDa"))
                        });
                    }        
                }
                return list; 
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

        //Người dùng tự cập nhật thong tin cá nhân
        public async Task<int> _UpdatePersonalInfomation(PersonalInformationDTO personalInfo){
            using var connection = await _context.Get_MySqlConnection();
            
            try{
                //Kiểm tra trạng thái kết nối trước khi mở
                if(connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync(); 
                
                //Tìm xem số điện thoại trước đó có tồn tại không
                bool CheckOld_phoneNum = await CheckPhoneNumberAlreadyExits(personalInfo.old_SDT,connection) > 0;
                if(!CheckOld_phoneNum) return 0;

                //Kiểm tra xem số điện thoại có bị trùng không
                if(await CheckPhoneNumberAlreadyExits(personalInfo.SDT, connection) > 1) return -1;

                //Kiểm tra xem email có bị trùng không
                if(await CheckEmailAlreadyExits(personalInfo.Email, connection) > 1) return -2;           

                //Thay đổi thông tin chính
                const string sqlUser = @"
                UPDATE nguoidung 
                SET HoTen = @HoTen, GioiTinh=@GioiTinh, NgaySinh=@NgaySinh,
                DiaChiLienLac=@DiaChiLienLac, SDT=@SDT, ID_DanToc=@ID_DanToc,
                Email=@Email
                WHERE ID_user = @ID_user;";

                //Thay đổi tải khoản dựa trên sdt mới
                const string sqlacc = @"UPDATE taikhoan 
                SET TaiKhoan = @SDT
                WHERE TaiKhoan = @OLD_SDT;";

                using(var command = new MySqlCommand(sqlUser + sqlacc, connection)){
                    command.Parameters.AddWithValue("@ID_user", personalInfo.ID_user);
                    command.Parameters.AddWithValue("@HoTen", personalInfo.HoTen);
                    command.Parameters.AddWithValue("@GioiTinh", personalInfo.GioiTinh);
                    command.Parameters.AddWithValue("@NgaySinh", personalInfo.NgaySinh);
                    command.Parameters.AddWithValue("@DiaChiLienLac", personalInfo.DiaChiLienLac);
                    command.Parameters.AddWithValue("@OLD_SDT", personalInfo.old_SDT);
                    command.Parameters.AddWithValue("@SDT", personalInfo.SDT);
                    command.Parameters.AddWithValue("@Email", personalInfo.Email);
                    command.Parameters.AddWithValue("@ID_DanToc", personalInfo.ID_DanToc);
                    
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