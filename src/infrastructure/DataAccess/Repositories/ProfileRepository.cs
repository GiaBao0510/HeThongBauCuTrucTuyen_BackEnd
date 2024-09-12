

using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class ProfileRepository: IDisposable, IProfileRepository
    {
         private readonly DatabaseContext _context;

        //Khởi tạo
        public ProfileRepository(DatabaseContext context){
            _context = context;
        }

        //hủy
        public void Dispose() => _context.Dispose();

        //1.Lấy ID_user dựa trên hồ sơ người dùng
        public async Task<string> _GetIDUserByProfile(string MaSo){
            using var connection = await _context.Get_MySqlConnection();

            string IdUser = null;
            const string sql = "SELECT ID_user FROM hosonguoidung WHERE MaSo =@MaSo ";
            
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@MaSo", MaSo);
                
                using var reader = await command.ExecuteReaderAsync();
                if(await reader.ReadAsync())
                    IdUser = reader.GetString(reader.GetOrdinal("ID_user"));
            }
            return IdUser;
        }
        //2.kiểm tra xem hồ sơ của người dùng đã đăng ký chưa
        public async Task<bool> _CheckRegistedUserProfile(string ID_user){
            using var connection = await _context.Get_MySqlConnection();
            const string sql = "SELECT COUNT(ID_user) FROM hosonguoidung WHERE ID_user =@ID_user && TrangThaiDangKy =@TrangThaiDangKy ";

            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_user", ID_user);
                command.Parameters.AddWithValue("@TrangThaiDangKy", "1");
                
                return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
            } 
        }
        //Kiểm tra xem hồ sơ người dùng có tồn tại không
        public async Task<bool> _CheckProfileExists(string ID_user, MySqlConnection connection){
            const string sql = "SELECT COUNT(ID_user) FROM hosonguoidung WHERE ID_user = @ID_user";

            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_user", ID_user);
                
                return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
            }  
        }
        
        //4.Thêm hồ sơ người dùng
        public async Task<bool> _AddProfile(string ID_user,  string Status,MySqlConnection connection){
            
            //Kiểm tra xem người dùng có tồn tại không. Nếu có thì không thể thêm
            bool CheckUserProfileExists = await _CheckProfileExists(ID_user, connection);
            if(CheckUserProfileExists) return false;
            
            //SQL thêm
            const string sql = "INSERT INTO hosonguoidung(TrangThaiDangKy,ID_user) VALUES(@TrangThaiDangKy,@ID_user);";

            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_user", ID_user);
                command.Parameters.AddWithValue("@TrangThaiDangKy", Status);
                
                await command.ExecuteNonQueryAsync();
            }
            return true; 
        }

        //4.Cập nhật hố sơ người dùng - mã người dùng
        public async Task<bool> _UpdateProfileBy_IdUser(ProfileDto profileDto, string userID){
            
            using var connection = await _context.Get_MySqlConnection();
            const string sqlUpdate = "UPDATE hosonguoidung SET TrangThaiDangKy =@TrangThaiDangKy WHERE ID_user =@ID_user; ";
            using(var command = new MySqlCommand(sqlUpdate, connection)){
                command.Parameters.AddWithValue("@ID_user", userID);
                command.Parameters.AddWithValue("@TrangThaiDangKy", profileDto.TrangThaiDangKy);
                
                return Convert.ToInt32(await command.ExecuteNonQueryAsync()) > 0;
            }
        }
        //5.Cập nhật hố sơ người dùng - mã hồ sơ
        public async Task<bool> _UpdateProfileBy_ProfileCode(ProfileDto profileDto, string ProfileCode){
            using var connection = await _context.Get_MySqlConnection();
            const string sqlUpdate = "UPDATE hosonguoidung SET TrangThaiDangKy =@TrangThaiDangKy WHERE MaSo =@MaSo; ";
            using(var command = new MySqlCommand(sqlUpdate, connection)){
                command.Parameters.AddWithValue("@MaSo", ProfileCode);
                command.Parameters.AddWithValue("@TrangThaiDangKy", profileDto.TrangThaiDangKy);
                
                return Convert.ToInt32(await command.ExecuteNonQueryAsync()) > 0;
            }
        }
        //6.Lấy danh sách người dùng đã đăng ký
        public async Task<List<ProfileDto>> _GetListRegisteredProfiles(){
            var list = new List<ProfileDto>();
            using var connection = await _context.Get_MySqlConnection();
            const string sql = @"
            SELECT hs.MaSo, hs.TrangThaiDangKy, hs.ID_user, nd.HoTen 
            FROM hosonguoidung hs
            INNER JOIN nguoidung nd ON hs.ID_user = nd.ID_user
            WHERE hs.TrangThaiDangKy =@TrangThaiDangKy;";
            
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@TrangThaiDangKy", "1");

                using var reader = await command.ExecuteReaderAsync();
                while(await reader.ReadAsync()){
                    list.Add(new ProfileDto{
                        MaSo = reader.GetInt32(reader.GetOrdinal("MaSo")),
                        TrangThaiDangKy = reader.GetString(reader.GetOrdinal("TrangThaiDangKy")),
                        ID_user = reader.GetString(reader.GetOrdinal("ID_user")),
                        HoTen = reader.GetString(reader.GetOrdinal("HoTen"))
                    });
                }
            }
            return list;
        }
        //7.Lấy danh sách người dùng chưa đăng ký
        public async Task<List<ProfileDto>> _GetListUnregisteredProfiles(){
            var list = new List<ProfileDto>();
            using var connection = await _context.Get_MySqlConnection();
            const string sql = @"
            SELECT hs.MaSo, hs.TrangThaiDangKy, hs.ID_user, nd.HoTen 
            FROM hosonguoidung hs
            INNER JOIN nguoidung nd ON hs.ID_user = nd.ID_user
            WHERE hs.TrangThaiDangKy =@TrangThaiDangKy;";
            
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@TrangThaiDangKy", "0");

                using var reader = await command.ExecuteReaderAsync();
                while(await reader.ReadAsync()){
                    list.Add(new ProfileDto{
                        MaSo = reader.GetInt32(reader.GetOrdinal("MaSo")),
                        TrangThaiDangKy = reader.GetString(reader.GetOrdinal("TrangThaiDangKy")),
                        ID_user = reader.GetString(reader.GetOrdinal("ID_user")),
                        HoTen = reader.GetString(reader.GetOrdinal("HoTen"))
                    });
                }
            }
            return list;
        }
        //8.Lấy danh sách hồ sơ
        public async Task<List<ProfileDto>> _GetListProfiles(){
            var list = new List<ProfileDto>();
            using var connection = await _context.Get_MySqlConnection();
            const string sql = "SELECT * FROM hosonguoidung;";
            
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@TrangThaiDangKy", "0");

                using var reader = await command.ExecuteReaderAsync();
                while(await reader.ReadAsync()){
                    list.Add(new ProfileDto{
                        ID_user = reader.GetString(reader.GetOrdinal("ID_user")),
                        MaSo = reader.GetInt32(reader.GetOrdinal("MaSo")),
                        TrangThaiDangKy = reader.GetString(reader.GetOrdinal("TrangThaiDangKy"))
                    });
                }
            }
            return list;
        }
        //9. Lấy danh sách cử tri đã đăng ký tài khoản
        public async Task<List<ProfileDto>> _GetListRegisteredVoter(){
            var list = new List<ProfileDto>();
            using var connection = await _context.Get_MySqlConnection();
            const string sql = @"
            SELECT hs.MaSo, hs.TrangThaiDangKy, hs.ID_user, nd.HoTen 
            FROM hosonguoidung hs 
            INNER JOIN nguoidung nd ON hs.ID_user = nd.ID_user
            JOIN cutri ct ON ct.ID_user = nd.ID_user
            WHERE hs.TrangThaiDangKy =@TrangThaiDangKy ;";
            
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@TrangThaiDangKy", "1");

                using var reader = await command.ExecuteReaderAsync();
                while(await reader.ReadAsync()){
                    list.Add(new ProfileDto{
                        MaSo = reader.GetInt32(reader.GetOrdinal("MaSo")),
                        TrangThaiDangKy = reader.GetString(reader.GetOrdinal("TrangThaiDangKy")),
                        ID_user = reader.GetString(reader.GetOrdinal("ID_user")),
                        HoTen = reader.GetString(reader.GetOrdinal("HoTen"))
                    });
                }
            }
            return list;
        }
        //10. Lấy danh sách cử tri chưa đăng ký tài khoản
        public async Task<List<ProfileDto>> _GetListUnregisteredVoter(){
            var list = new List<ProfileDto>();
            using var connection = await _context.Get_MySqlConnection();
            const string sql = @"
            SELECT hs.MaSo, hs.TrangThaiDangKy, hs.ID_user, nd.HoTen 
            FROM hosonguoidung hs 
            INNER JOIN nguoidung nd ON hs.ID_user = nd.ID_user
            JOIN cutri ct ON ct.ID_user = nd.ID_user
            WHERE hs.TrangThaiDangKy =@TrangThaiDangKy ;";
            
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@TrangThaiDangKy", "0");

                using var reader = await command.ExecuteReaderAsync();
                while(await reader.ReadAsync()){
                    list.Add(new ProfileDto{
                        MaSo = reader.GetInt32(reader.GetOrdinal("MaSo")),
                        TrangThaiDangKy = reader.GetString(reader.GetOrdinal("TrangThaiDangKy")),
                        ID_user = reader.GetString(reader.GetOrdinal("ID_user")),
                        HoTen = reader.GetString(reader.GetOrdinal("HoTen"))
                    });
                }
            }
            return list;
        }

        //11.Nếu cử tri quét mã thì sẽ tự động cập nhật trạng thái cử tri là đã đăng ký
        public async Task<bool> _AutomaticallyUpdateUserStatusIfRegistered(string ID_cutri){
            using var connection = await _context.Get_MySqlConnection();
            const string sqlUpdate = @"UPDATE hosonguoidung hs
            INNER JOIN nguoidung nd ON nd.ID_user = hs.ID_user
            INNER JOIN cutri ct ON ct.ID_user = nd.ID_user
            SET hs.TrangThaiDangKy = @TrangThaiDangKy
            WHERE ct.ID_CuTri = @ID_CuTri;";
            
            using(var command = new MySqlCommand(sqlUpdate, connection)){
                command.Parameters.AddWithValue("@ID_CuTri", ID_cutri);
                command.Parameters.AddWithValue("@TrangThaiDangKy","1");
                
                return Convert.ToInt32(await command.ExecuteNonQueryAsync()) > 0;
            }
        }
        //12. Xóa hồ sơ dựa trên ID_người dùng
        public async Task<bool> _DeleteProfileBy_Id_user(string ID_user){
            using var connection = await _context.Get_MySqlConnection();
            const string sqlDelete = "DELETE FROM hosonguoidung WHERE ID_user = @ID_user;";
            
            using(var command = new MySqlCommand(sqlDelete, connection)){
                command.Parameters.AddWithValue("@ID_user", ID_user);
                
                return Convert.ToInt32(await command.ExecuteNonQueryAsync()) > 0;
            }
        }
        //13. Xóa hồ sơ dựa trên Mã hồ sơ
        public async Task<bool> _DeleteProfileBy_ProfileCode(string ProfileCode){
            using var connection = await _context.Get_MySqlConnection();
            const string sqlDelete = "DELETE FROM hosonguoidung WHERE MaSo = @MaSo;";
            
            using(var command = new MySqlCommand(sqlDelete, connection)){
                command.Parameters.AddWithValue("@MaSo", ProfileCode);
                
                return Convert.ToInt32(await command.ExecuteNonQueryAsync()) > 0;
            }
        }
        //14. Xóa hồ sơ dựa trên ID_cử tri
        public async Task<bool> _DeleteProfileBy_ID_cutri(string ID_cutri){
            using var connection = await _context.Get_MySqlConnection();
            const string sqlDelete = @"
            DELETE hs
            FROM hosonguoidung hs
            JOIN nguoidung nd ON hs.ID_user = nd.ID_user
            JOIN cutri ct ON ct.ID_user = nd.ID_user
            WHERE ct.ID_CuTri = @ID_CuTri;";
            
            using(var command = new MySqlCommand(sqlDelete, connection)){
                command.Parameters.AddWithValue("@ID_CuTri", ID_cutri);
                
                return Convert.ToInt32(await command.ExecuteNonQueryAsync()) > 0;
            }
        }
        //15. Xóa hồ sơ dựa trên ID_Cán bộ
        public async Task<bool> _DeleteProfileBy_ID_canbo(string ID_canbo){
            using var connection = await _context.Get_MySqlConnection();
            const string sqlDelete = @"
            DELETE hs 
            FROM hosonguoidung hs
            JOIN nguoidung nd ON hs.ID_user = nd.ID_user 
            JOIN canbo cb ON cb.ID_user = nd.ID_user
            WHERE cb.ID_CanBo = @ID_CanBo;";
            
            using(var command = new MySqlCommand(sqlDelete, connection)){
                command.Parameters.AddWithValue("@ID_CanBo", ID_canbo);
                
                return Convert.ToInt32(await command.ExecuteNonQueryAsync()) > 0;
            }
        }
        //16. Xóa hồ sơ dựa trên ID_ứng cử viên
        public async Task<bool> _DeleteProfileBy_ID_ungcuvien(string ID_ungcuvien){
            using var connection = await _context.Get_MySqlConnection();
            const string sqlDelete = @"
            DELETE hs 
            FROM hosonguoidung hs 
            JOIN nguoidung nd ON hs.ID_user = nd.ID_user
            JOIN ungcuvien ucv ON ucv.ID_user = nd.ID_user
            WHERE ucv.ID_ucv = @ID_ucv;";
            
            using(var command = new MySqlCommand(sqlDelete, connection)){
                command.Parameters.AddWithValue("@ID_ucv", ID_ungcuvien);
                
                return Convert.ToInt32(await command.ExecuteNonQueryAsync()) > 0;
            }
        }

    }
}