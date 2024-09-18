using BackEnd.src.infrastructure.DataAccess.Context;
using MySql.Data.MySqlClient;
using BackEnd.src.web_api.DTOs;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.AspNetCore.Http;
using BackEnd.src.infrastructure.Services;
using BackEnd.src.core.Common;
using Isopoh.Cryptography.Argon2;
using System.Data;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class VoterReposistory : UserRepository, IDisposable ,IVoterRepository
    {
        private readonly DatabaseContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IProfileRepository _profileRepository;

        //Khởi tạo
        public VoterReposistory(
            DatabaseContext context,CloudinaryService cloudinaryService,
            IProfileRepository profileRepository
        ): base(context, cloudinaryService){
            _context=context;
            _cloudinaryService = cloudinaryService;
            _profileRepository = profileRepository;
        }
        //Hủy
        public new void Dispose() => _context.Dispose();

        //-1 .Kiểm tra ID cử tri có tồn tại không
        public async Task<bool> _CheckVoterExists(string ID, MySqlConnection connection){
            const string sqlCount = "SELECT COUNT(ID_CuTri) FROM cutri WHERE ID_CuTri = @ID_CuTri";
            using(var command = new MySqlCommand(sqlCount, connection)){
                command.Parameters.AddWithValue("@ID_CuTri",ID);
                
                int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                if(count < 1) return false;
            }
            return true;
        }

        //0. Lấy ID người dùng dựa trên ID cử tri
        public async Task<string> GetIDUserBaseOnIDCuTri(string id, MySqlConnection connection){
            string ID_user = null;
            const string sql = "SELECT ID_user FROM cutri WHERE ID_CuTri = @ID_CuTri;";
            
            using (var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_CuTri", id);

                using var reader = await command.ExecuteReaderAsync();
                if(await reader.ReadAsync())
                    ID_user = reader.GetString(reader.GetOrdinal("ID_user"));
            }
            return ID_user;
        }

        //1.Thêm
        public async Task<int> _AddVoter(VoterDto voter, IFormFile fileAnh){
            using var connect = await _context.Get_MySqlConnection();

            //Kiểm tra kết nối
            if(connect.State != System.Data.ConnectionState.Open )
                await connect.OpenAsync();
            
            using var transaction =await connect.BeginTransactionAsync();

            try{
                voter.RoleID = 5;   //Gán vai trò mặc định khi tạo cử tri

                //Thêm thông tin cơ sở
                var FillInBasicInfo = await _AddVoterWithConnect(voter, fileAnh, connect, transaction);
                
                //Nếu có lỗi thì in ra
                if(FillInBasicInfo is int result && result <=0){
                    await transaction.RollbackAsync();
                    return result;
                }
                
                //Lấy ID người dùng vừa tạo và tạo ID cử tri
                string ID_cutri = RandomString.CreateID(),
                        ID_user = FillInBasicInfo.ToString();

                //Ngược lại thêm cử tri
                const string sql = "INSERT INTO cutri(ID_CuTri,ID_user) VALUES(@ID_CuTri,@ID_user);";
                using(var command = new MySqlCommand(sql, connect)){
                    command.Parameters.AddWithValue("@ID_CuTri", ID_cutri);
                    command.Parameters.AddWithValue("@ID_user", ID_user);

                    await command.ExecuteNonQueryAsync();
                }

                //Tạo hồ sơ
                bool AddProfile = await _profileRepository._AddProfile(ID_user, "0", connect);
                if(!AddProfile){
                    Console.WriteLine("Lỗi khi tạo hồ sơ cho cử tri");
                    await transaction.RollbackAsync();
                    return -100;
                }

                await transaction.CommitAsync();
                return 1;
            }catch(Exception ex){
                try{
                    await transaction.RollbackAsync();
                }catch(Exception rollbackEx){
                    // Log lỗi rollback nếu cần thiết
                    Console.WriteLine($"Rollback Exception Message: {rollbackEx.Message}");
                    Console.WriteLine($"Rollback Stack Trace: {rollbackEx.StackTrace}");
                }
                // Log lỗi và ném lại exception để controller xử lý
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        //2. Lấy thông tin người dùng theo ID
        public async Task<VoterDto> _GetVoterBy_ID(string IDvoter){
            using var connection = await _context.Get_MySqlConnection();
            
            const string sql = @"
            SELECT * 
            FROM cutri ct 
            INNER JOIN nguoidung nd ON nd.ID_user = ct.ID_user 
            WHERE ct.ID_CuTri = @ID_CuTri;";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID_CuTri", IDvoter);

            using var reader = await command.ExecuteReaderAsync();
            if(await reader.ReadAsync()){
                return new VoterDto{
                    ID_CuTri = reader.GetString(reader.GetOrdinal("ID_CuTri")),
                    ID_user = reader.GetString(reader.GetOrdinal("ID_user")),
                    HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                    GioiTinh = reader.GetString(reader.GetOrdinal("GioiTinh")),
                    NgaySinh = reader.GetDateTime(reader.GetOrdinal("NgaySinh")).ToString("dd-MM-yyyy HH:mm:ss"),
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

        //3. Sửa thông tin cử tri dựa trên ID cử tri
        public async Task<int> _EditVoterBy_ID(string IDvoter ,VoterDto voter){
            using var connection = await _context.Get_MySqlConnection();

            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync(); 

            using var transaction = await connection.BeginTransactionAsync();

            try{
                //Lấy ID_user từ cử tri
                string ID_user = await GetIDUserBaseOnIDCuTri(IDvoter, connection);
                if(string.IsNullOrEmpty(ID_user))
                    return -5;
                
                //Kiểm tra thông tin đầu vào không được trùng
                int KiemTraThongTinTrungNhau = await CheckForDuplicateUserInformation(1, voter, connection);
                if(KiemTraThongTinTrungNhau <= 0){
                    Console.WriteLine("Thông tin đầu vào trùng nhau");
                    return KiemTraThongTinTrungNhau;
                }

                //Kiểm tra đầu vào không được null
                if(CheckUserInformationIsNotEmpty(voter) == false){
                    Console.WriteLine("Thông tin đầu vào chưa điền đây đủ");
                    return -4;
                }

                //Cập nhật thông tin cử tri - Và cập nhật phần tài khoảng cử tri, nếu SDT thay đổi
                const string sqlNguoiDung = @"
                    UPDATE nguoidung 
                    SET HoTen = @HoTen, GioiTinh=@GioiTinh, NgaySinh=@NgaySinh,
                        DiaChiLienLac=@DiaChiLienLac, CCCD=@CCCD, SDT=@SDT, ID_DanToc=@ID_DanToc,
                        Email=@Email, RoleID=@RoleID
                    WHERE ID_user = @ID_user;";
                const string sqlVoterAccount = @"
                UPDATE taikhoan
                SET taikhoan = @SDT
                WHERE Taikhoan = @Taikhoan;";

                using(var command1 = new MySqlCommand($"{sqlNguoiDung} {sqlVoterAccount}", connection)){
                    command1.Parameters.AddWithValue("@ID_user", ID_user);
                    command1.Parameters.AddWithValue("@HoTen", voter.HoTen);
                    command1.Parameters.AddWithValue("@GioiTinh", voter.GioiTinh);
                    command1.Parameters.AddWithValue("@NgaySinh", voter.NgaySinh);
                    command1.Parameters.AddWithValue("@DiaChiLienLac", voter.DiaChiLienLac);
                    command1.Parameters.AddWithValue("@CCCD", voter.CCCD);
                    command1.Parameters.AddWithValue("@SDT", voter.SDT);
                    command1.Parameters.AddWithValue("@Taikhoan", voter.TaiKhoan);
                    command1.Parameters.AddWithValue("@Email", voter.Email);
                    command1.Parameters.AddWithValue("@RoleID", voter.RoleID);
                    command1.Parameters.AddWithValue("@ID_DanToc", voter.ID_DanToc);
                
                    int rowAffected = await command1.ExecuteNonQueryAsync();
                    if(rowAffected < 0) return -6;

                    Console.WriteLine("Cập nhật thông tin cá nhân thành công");
                }
                await transaction.CommitAsync();
                return 1;
            }catch(MySqlException ex){
                await transaction.RollbackAsync();

                //Xử lý lỗi MySQL cụ thể
                switch(ex.Number){
                    case 1062:
                        if(ex.Message.Contains("CCCD"))
                            return -2;
                        else if(ex.Message.Contains("SDT"))
                            return 0;
                        else if(ex.Message.Contains("Email"))
                            return -1;
                        else
                            return -7;
                    default:
                        return -8;
                }

            }catch(Exception){
                await transaction.RollbackAsync();
                return -9;
            }
            
        }

        //4. Lấy tất cả thông tin cử tri
        public async Task<List<VoterDto>> _GetListOfVoter(){
            var list = new List<VoterDto>();
            using var connection = await _context.Get_MySqlConnection();
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            const string sql = @"
            SELECT * FROM nguoidung nd 
            INNER JOIN cutri ct ON ct.ID_user = nd.ID_user";
            using var command = new MySqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new VoterDto{
                    ID_CuTri =reader.GetString(reader.GetOrdinal("ID_CuTri")), 
                    ID_user = reader.GetString(reader.GetOrdinal("ID_user")),
                    HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                    GioiTinh = reader.GetString(reader.GetOrdinal("GioiTinh")),
                    NgaySinh = reader.GetDateTime(reader.GetOrdinal("NgaySinh")).ToString("dd-MM-yyyy"),
                    DiaChiLienLac = reader.GetString(reader.GetOrdinal("DiaChiLienLac")),
                    CCCD = reader.GetString(reader.GetOrdinal("CCCD")),
                    Email = reader.IsDBNull(reader.GetOrdinal("Email"))? null: reader.GetString(reader.GetOrdinal("Email")),
                    SDT = reader.GetString(reader.GetOrdinal("SDT")),
                    HinhAnh = reader.IsDBNull(reader.GetOrdinal("HinhAnh"))? null:reader.GetString(reader.GetOrdinal("HinhAnh")),
                    ID_DanToc = reader.GetInt32(reader.GetOrdinal("ID_DanToc")),
                    RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                    PublicID = reader.IsDBNull(reader.GetOrdinal("PublicID"))? null :reader.GetString(reader.GetOrdinal("PublicID"))
                });
            }
            return list;
        }

        //5. Đặt mật khẩu cử tri
        public async Task<bool> _SetVoterPassword(string id,string newPwd){
            using var connection = await _context.Get_MySqlConnection();

            //Băm mật khẩu
            newPwd = Argon2.Hash(newPwd);

            //Cập nhật mật khẩu dựa trên ID
            const string sql = @"
            UPDATE taikhoan tk 
            JOIN nguoidung nd ON nd.sdt = tk.TaiKhoan
            JOIN cutri ct ON nd.ID_user = ct.ID_user
            SET tk.MatKhau = @MatKhau
            WHERE ct.ID_CuTri = @ID_CuTri;";
            
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_CuTri", id);
                command.Parameters.AddWithValue("@MatKhau", newPwd);
                
                int rowAffect = await command.ExecuteNonQueryAsync();
                if(rowAffect < 0)
                    return false;
            }
            return true;
        }

        //6. Cử tri thay đổi mật khẩu
        public async Task<int> _ChangeVoterPassword(string id, string oldPwd, string newPwd){
            using var connect = await _context.Get_MySqlConnection();

            //Lấy mật khẩu đã băm của cử tri
            string hashedPwd = null;
            const string sqlGetHashedPwd = @"
            SELECT tk.MatKhau 
            FROM taikhoan tk
            INNER JOIN nguoidung nd ON tk.TaiKhoan = nd.SDT
            INNER JOIN cutri ct ON ct.ID_user = nd.ID_user
            WHERE ct.ID_CuTri = @ID_CuTri;";

            using(var command0 = new MySqlCommand(sqlGetHashedPwd,connect)){
                command0.Parameters.AddWithValue("@ID_CuTri", id);
                
                using var reader = await command0.ExecuteReaderAsync();
                if(await reader.ReadAsync())
                    hashedPwd = reader.GetString(reader.GetOrdinal("MatKhau"));
            }

            //Nếu tìm không thấy ID cử tri thì trả về 0
            if(string.IsNullOrEmpty(hashedPwd)) return 0;

            //So sánh 2 mật khẩu đã băm và mật khẩu cũ
            if(Argon2.Verify(hashedPwd, oldPwd) != true) return -1;

            //Băm mật khẩu
            newPwd = Argon2.Hash(newPwd);

            //Truy xuất ID cử tri rồi cập nhật lại
            const string sqlUpdatePwd = @"
            UPDATE taikhoan tk 
            JOIN nguoidung nd ON nd.sdt = tk.TaiKhoan
            JOIN cutri ct ON nd.ID_user = ct.ID_user
            SET tk.MatKhau = @MatKhau
            WHERE ct.ID_CuTri = @ID_CuTri;";

            using(var command1 = new MySqlCommand(sqlUpdatePwd, connect)){
                command1.Parameters.AddWithValue("@ID_CuTri", id);
                command1.Parameters.AddWithValue("@MatKhau", newPwd);
                
                int rowAffect = await command1.ExecuteNonQueryAsync();
                if(rowAffect < 0)
                    return -2;
            }

            return 1;
        }

        //7. Xóa tài khoảng người dùng dựa trên ID cư tri
        public async Task<bool> _DeleteVoterBy_ID(string IDvoter){
            using var connect = await _context.Get_MySqlConnection();

            //Truy xuất lấy ID cử tri
            string ID_user = await GetIDUserBaseOnIDCuTri(IDvoter, connect);
            if(string.IsNullOrEmpty(ID_user)) return false;

            //Xóa tài khoản cử tri trước rồi xóa tài khoản người dùng sau
            const string sqlDeleteVoter = @"DELETE FROM cutri WHERE ID_CuTri = @ID_CuTri;";
            using(var command1 = new MySqlCommand(sqlDeleteVoter, connect)){
                command1.Parameters.AddWithValue("@ID_CuTri", IDvoter);
                
                int rowAffect = await command1.ExecuteNonQueryAsync();
                if(rowAffect < 0)
                    return false;
            }

            if(await _DeleteUserBy_ID_withConnection(ID_user, connect) == false)
                return false;

            return true;
        }

        //8. Lấy thông tin cử tri và kèm theo tài khoản
        public async Task<List<VoterDto>> _GetListOfVotersAndAccounts(){
            var list = new List<VoterDto>();
            using var connection = await _context.Get_MySqlConnection();
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            const string sql = @"
            SELECT nd.ID_user,nd.HoTen,nd.GioiTinh,nd.NgaySinh,nd.DiaChiLienLac,nd.CCCD,nd.Email,nd.SDT,
            nd.HinhAnh,nd.PublicID,nd.ID_DanToc,nd.RoleID, ct.ID_CuTri 
            ,tk.TaiKhoan,tk.MatKhau,tk.BiKhoa,tk.LyDoKhoa,tk.NgayTao,tk.SuDung,tk.RoleID 
            FROM nguoidung nd 
            INNER JOIN taikhoan tk ON tk.TaiKhoan = nd.SDT 
            JOIN cutri ct ON ct.ID_user = nd.ID_user";
            using var command = new MySqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new VoterDto{
                    ID_CuTri = reader.GetString(reader.GetOrdinal("ID_CuTri")),
                    ID_user = reader.GetString(reader.GetOrdinal("ID_user")),
                    HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                    GioiTinh = reader.GetString(reader.GetOrdinal("GioiTinh")),
                    NgaySinh = reader.GetDateTime(reader.GetOrdinal("NgaySinh")).ToString("dd-MM-yyyy"),
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
                    BiKhoa = reader.GetBoolean(reader.GetOrdinal("BiKhoa")).ToString(),
                    LyDoKhoa = reader.IsDBNull(reader.GetOrdinal("LyDoKhoa")) ? null : reader.GetString(reader.GetOrdinal("LyDoKhoa")),
                    NgayTao = reader.GetDateTime(reader.GetOrdinal("NgayTao")).ToString("dd-MM-yyyy HH:mm:ss"),
                    SuDung = reader.GetInt32(reader.GetOrdinal("SuDung"))
                });
            }
            return list;
        }

        //9. Gửi báo cáo
        public async Task<bool> _VoterSubmitReport(SendReportDto reportDto){
            using var connect = await _context.Get_MySqlConnection();
            using var transaction =await connect.BeginTransactionAsync();

            try{
                //Kiểm tra cử tri có tồn tại hay không
                bool CheckExists = await _CheckVoterExists(reportDto.IDSender, connect);
                if(!CheckExists) return false;

                //Gửi báo cáo
                const string sqlSend = @"INSERT INTO phanhoicutri(ID_CuTri,ThoiDiem,Ykien) 
                VALUES(@ID_CuTri,@ThoiDiem,@Ykien);";

                using(var command = new MySqlCommand(sqlSend, connect)){
                    command.Parameters.AddWithValue("@ID_CuTri", reportDto.IDSender);
                    command.Parameters.AddWithValue("@ThoiDiem", reportDto.ThoiDiem);
                    command.Parameters.AddWithValue("@Ykien", reportDto.YKien);
                    
                    await command.ExecuteNonQueryAsync();
                }
                await transaction.CommitAsync();
                return true;

            }catch(MySqlException ex){
                Console.WriteLine($"Error in mySQL Message: {ex.Message}");
                Console.WriteLine($"Error in mySQL StackTrace: {ex.StackTrace}");
                Console.WriteLine($"Error in mySQL Code: {ex.Code}");

                await transaction.RollbackAsync();
                return false;
            }catch(Exception ex){
                Console.WriteLine($"Error in mySQL Message: {ex.Message}");
                Console.WriteLine($"Error in mySQL StackTrace: {ex.StackTrace}");
                Console.WriteLine($"Error in mySQL TargetSite: {ex.TargetSite}");

                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}