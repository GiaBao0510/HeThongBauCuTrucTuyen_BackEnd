using BackEnd.src.infrastructure.DataAccess.Context;
using MySql.Data.MySqlClient;
using BackEnd.src.web_api.DTOs;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.AspNetCore.Http;
using BackEnd.src.infrastructure.Services;
using BackEnd.src.core.Common;
using Isopoh.Cryptography.Argon2;
using BackEnd.src.core.Interfaces;
using System.Numerics;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class VoterReposistory : UserRepository, IDisposable ,IVoterRepository
    {
        private readonly DatabaseContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IProfileRepository _profileRepository;
        private readonly IElectionsRepository _electionsRepository;
        private readonly IConstituencyRepository _constituencyRepository;
        //Khởi tạo
        public VoterReposistory(
            DatabaseContext context,CloudinaryService cloudinaryService,
            IProfileRepository profileRepository, 
            IElectionsRepository electionsRepository,
            IConstituencyRepository constituencyRepository,
            IPaillierServices paillierServices,
            IVoteRepository voteRepository,
            IListOfPositionRepository listOfPositionRepository
        ): base(context, cloudinaryService){
            _context=context;
            _cloudinaryService = cloudinaryService;
            _profileRepository = profileRepository;
            _electionsRepository = electionsRepository;
            _constituencyRepository = constituencyRepository;
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
                //Kiểm tra mã chức vụ có tồn tại không
                bool checkPosition = await _CheckPositionExist(voter.ID_ChucVu, connect, transaction);
                if(!checkPosition){
                    await transaction.RollbackAsync();
                    return -6;
                }

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
                    return -5;
                }

                //Tạo chức vụ cho cử tri
                int AddPosition = await _VoterTakePosition(ID_cutri, voter.ID_ChucVu, connect,transaction);

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
                        DiaChiLienLac=@DiaChiLienLac, SDT=@SDT, ID_DanToc=@ID_DanToc,
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
            SELECT ct.ID_CuTri, nd.ID_user, nd.HoTen, nd.GioiTinh, nd.NgaySinh, 
            nd.DiaChiLienLac, nd.Email, nd.SDT, nd.HinhAnh, nd.PublicID, nd.ID_DanToc 
            FROM nguoidung nd 
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
                    Email = reader.IsDBNull(reader.GetOrdinal("Email"))? null: reader.GetString(reader.GetOrdinal("Email")),
                    SDT = reader.GetString(reader.GetOrdinal("SDT")),
                    HinhAnh = reader.IsDBNull(reader.GetOrdinal("HinhAnh"))? null:reader.GetString(reader.GetOrdinal("HinhAnh")),
                    ID_DanToc = reader.GetInt32(reader.GetOrdinal("ID_DanToc")),
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

        //10.Hiển thị mã cử tri sau khi người dùng quét mã
        public async Task<VoterDto> _DisplayUserInformationAfterScanningTheCode(string ID){
            using var connect = await _context.Get_MySqlConnection();
            
            //Kiểm tra xem thông tin cử tri đã đăng ký chưa .Nếu chưa thì hiển thị
            int checkRegisteredVoter = await _CheckRegisteredVoter( ID, connect);
            if(checkRegisteredVoter == -1 || checkRegisteredVoter == 1) return null;
            
            var voter = new VoterDto();

            const string sql = @"
            SELECT nd.HoTen, nd.GioiTinh, nd.NgaySinh, nd.DiaChiLienLac, 
            nd.Email, nd.SDT, nd.HinhAnh,dt.TenDanToc 
            FROM nguoidung nd 
            JOIN cutri ct ON nd.ID_user = ct.ID_user
            JOIN dantoc dt ON dt.ID_DanToc = nd.ID_DanToc
            WHERE ct.ID_CuTri = @ID_CuTri;";

            using (var command = new MySqlCommand(sql, connect)){
                command.Parameters.AddWithValue("@ID_CuTri", ID);

                using var reader = await command.ExecuteReaderAsync();
                if(await reader.ReadAsync()){
                    voter.HoTen = reader.GetString(reader.GetOrdinal("HoTen"));
                    voter.GioiTinh = reader.GetString(reader.GetOrdinal("GioiTinh"));
                    voter.NgaySinh = reader.GetDateTime(reader.GetOrdinal("NgaySinh")).ToString("dd-MM-yyyy");
                    voter.DiaChiLienLac = reader.GetString(reader.GetOrdinal("DiaChiLienLac"));
                    voter.Email = reader.GetString(reader.GetOrdinal("Email"));
                    voter.SDT = reader.GetString(reader.GetOrdinal("SDT"));
                    voter.HinhAnh = reader.GetString(reader.GetOrdinal("HinhAnh"));
                    voter.TenDanToc = reader.GetString(reader.GetOrdinal("TenDanToc"));
                }
            }
            return voter;
        }

        //11.Kiểm tra xem thông tin cử tri đã đăng ký hay chưa
        public async Task<int> _CheckRegisteredVoter(string ID_cutri,MySqlConnection connect){

            //Kiểm tra xem ID người dùng có tồn tại không
            bool checkUserExists = await _CheckVoterExists(ID_cutri, connect);
            if(!checkUserExists) return -1;

            string trangThai = "0";

            //Kiểm tra xem người dùng đã đăng ký chưa
            const string sql = @"SELECT TrangThaiDangKy 
            FROM hosonguoidung hs JOIN cutri ct ON ct.ID_user = hs.ID_user
            WHERE ct.ID_CuTri = @ID_CuTri";
            using(var command = new MySqlCommand(sql, connect)){
                command.Parameters.AddWithValue("@ID_CuTri", ID_cutri);

                using var reader = await command.ExecuteReaderAsync();
                if(await reader.ReadAsync())
                    trangThai =  reader.GetString(reader.GetOrdinal("TrangThaiDangKy"));
            }
            return Convert.ToInt32(trangThai);
        }

        //12. Cử tri đảm nhận vị trí
        public async Task<int> _VoterTakePosition(string ID_voter, int ID_ChucVu, MySqlConnection connect ,MySqlTransaction? transaction){
            
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connect.State != System.Data.ConnectionState.Open)
                await connect.OpenAsync();

            try{
                //Kiểm tra xem mã chức vụ có tồn tại không
                bool checkPositionExists = await _CheckPositionExist(ID_ChucVu, connect, transaction);
                if(!checkPositionExists) return -1;

                //thêm chức vụ cho cử tri
                const string sql = @"INSERT INTO chitietcutri(ID_CuTri,ID_ChucVu) VALUES(@ID_CuTri,@ID_ChucVu);";
                using(var command = new MySqlCommand(sql, connect)){
                    command.Parameters.AddWithValue("@ID_CuTri", ID_voter);
                    command.Parameters.AddWithValue("@ID_ChucVu", ID_ChucVu);
                    
                    await  command.ExecuteNonQueryAsync(); 
                }
                return 1;
            }catch(Exception ex){
                Console.WriteLine($"Error in mySQL Message: {ex.Message}");
                Console.WriteLine($"Error in mySQL StackTrace: {ex.StackTrace}");
                Console.WriteLine($"Error in mySQL TargetSite: {ex.TargetSite}");

                await transaction.RollbackAsync();
                return -2;
            }
        }

        //13. Thay đổi chức vụ của cử tri
        public async Task<bool> _ChangeOfVoterPosition(string ID_voter, int ID_ChucVu){
            using var connect = await _context.Get_MySqlConnection();

            //Kiểm tra xem ID chức vụ có tồn tại không
            bool checkPositionExists = await _CheckPositionExist(ID_ChucVu, connect);
            if(!checkPositionExists) return false;

            const string sql = @"UPDATE chitietcutri SET ID_ChucVu =@ID_ChucVu WHERE ID_CuTri =@ID_CuTri; ";
            using (var command = new MySqlCommand(sql, connect)){
                command.Parameters.AddWithValue("@ID_CuTri", ID_voter);
                command.Parameters.AddWithValue("@ID_ChucVu", ID_ChucVu);
                
                int affect = await command.ExecuteNonQueryAsync();
                if(affect < 1 ) return false;
            }

            return true;
        }

        //14. Cử tri đặt mật khẩu, CCCD khi đăng ký
        public async Task<bool> _SetVoterCCCD_SetVoterPwd(string id, string newCCCD, string pwd){
            using var connect = await _context.Get_MySqlConnection();
            
            //Băm mật khẩu
            pwd = Argon2.Hash(pwd);
            
            const string sql = @"
            UPDATE nguoidung nd
            JOIN cutri ct ON ct.ID_user = nd.ID_user
            JOIN taikhoan tk ON tk.TaiKhoan = nd.SDT
            SET nd.CCCD = @CCCD, tk.MatKhau = @MatKhau
            WHERE ct.ID_CuTri = @ID_CuTri;";

            using(var command = new MySqlCommand(sql, connect)){
                command.Parameters.AddWithValue("@ID_CuTri", id);
                command.Parameters.AddWithValue("@CCCD", newCCCD);
                command.Parameters.AddWithValue("@MatKhau", pwd);
                
                int affect = await command.ExecuteNonQueryAsync();
                if(affect < 1 ) return false;
            }
            return true;
        }

        //15.Thêm danh sách cử tri vào cuộc bầu cử
        public async Task<int> _AddListVotersToTheElection(VoterListInElectionDto voterListInElectionDto){
            using var connect = await _context.Get_MySqlConnection();
            
            //Kiểm tra xem ngày bầu cử có tồn tại không
            bool checkElectionExist = await _electionsRepository._CheckIfElectionTimeExists(voterListInElectionDto.ngayBD, connect);
            if(!checkElectionExist) return 0;

            //Kiểm tra xem đơn vị bầu cử có hợp lệ không
            bool checkConstituencyExist = await _constituencyRepository._CheckIfConstituencyExists(voterListInElectionDto.ID_DonViBauCu, connect);
            if(!checkConstituencyExist) return -1;

            //Nếu danh sách cử tri vượt quá số lượt bình chọn tối đa đặt cho nó thì báo lỗi
            int sl_cuTriToiDa = await  _electionsRepository._MaximumNumberOfVoters(voterListInElectionDto.ngayBD, connect);
            if(sl_cuTriToiDa < 0) return -2;

            //Lấy số lượng cử tri trong kỳ bầu cử này ở hiện tại
            int sl_cuTriHienTai = await  _electionsRepository._GetCurrentVoterCountByElection(voterListInElectionDto.ngayBD, connect);
            if(sl_cuTriToiDa < 0) return -4;

            if((sl_cuTriHienTai + voterListInElectionDto.listIDVoter.Count) > sl_cuTriToiDa) return -3;

            //thêm từng cử tri trong danh sách vào cuộc bầu cử
            const string sql = @"
                INSERT INTO trangthaibaucu(ID_CuTri,ID_DonViBauCu,ngayBD,GhiNhan)
                VALUES(@ID_CuTri,@ID_DonViBauCu,@ngayBD,@GhiNhan);";

            foreach(string voter in voterListInElectionDto.listIDVoter){
                
                //Kiểm tra xem nếu cử tri đã tồn tại trong cuộc bầu cử này thì bỏ qua hoặc cử tri này không tồn tại trong danh sách thì bỏ qua
                bool checkVoterJoined = await _VoterCheckInElection(voter,voterListInElectionDto.ngayBD,connect);
                bool checVoterExists = await _CheckVoterExists(voter, connect);
                if(!checkVoterJoined && checVoterExists){
                    using(var command = new MySqlCommand(sql, connect)){
                        command.Parameters.AddWithValue("@ID_CuTri", voter);
                        command.Parameters.AddWithValue("@ID_DonViBauCu", voterListInElectionDto.ID_DonViBauCu);
                        command.Parameters.AddWithValue("@ngayBD", voterListInElectionDto.ngayBD);
                        command.Parameters.AddWithValue("@GhiNhan", "0");
                        
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            return 1;
        }

        //16. Kiểm tra xem cử tri có tồn tại trong kỳ bầu cử không
        public async Task<bool> _VoterCheckInElection(string ID_cutri, DateTime ngayBD, MySqlConnection connection){
            
            const string sql = @"
            SELECT COUNT(ct.ID_CuTri)
            FROM trangthaibaucu tt 
            JOIN cutri ct ON ct.ID_CuTri = tt.ID_CuTri
            WHERE tt.ngayBD = @ngayBD AND ct.ID_CuTri =@ID_CuTri;";

            using (var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ngayBD", ngayBD);
                command.Parameters.AddWithValue("@ID_CuTri", ID_cutri);
                int count = Convert.ToInt32(await command.ExecuteScalarAsync());

                return count > 0;
            }
        }           

        //18. Danh sách các kỳ bầu cử mà cử tri có thể tham gia
        public async Task<List<ElectionsDto>> _ListElectionsVotersHavePaticipated(string ID_cutri){
            
            using var connection = await _context.Get_MySqlConnection();
            var list = new List<ElectionsDto>();

            //Kiểm tra xem cử tri có tồn tại không
            bool checkVoterExists = await _CheckVoterExists(ID_cutri, connection);
            if(!checkVoterExists) return null;

            //lấy danh sách kỳ bầu cử có mặc cử tri
            const string sql = @" 
            SELECT kbc.ngayBD, kbc.ngayKT, kbc.TenKyBauCu, kbc.MoTa, kbc.SoLuongToiDaCuTri, kbc.SoLuongToiDaUngCuVien, kbc.SoLuotBinhChonToiDa
            FROM trangthaibaucu tt
            JOIN kybaucu kbc ON kbc.ngayBD = tt.ngayBD
            JOIN cutri ct ON tt.ID_CuTri = ct.ID_CuTri
            WHERE ct.ID_CuTri =@ID_CuTri; ";
            using (var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_CuTri",ID_cutri);

                using var reader = await command.ExecuteReaderAsync();
                while(await reader.ReadAsync()){
                    list.Add(new ElectionsDto{
                        ngayBD = reader.GetDateTime(reader.GetOrdinal("ngayBD")),
                        ngayKt =  reader.GetDateTime(reader.GetOrdinal("ngayBD")),
                        TenKyBauCu = reader.GetString(reader.GetOrdinal("TenKyBauCu")),
                        Mota = reader.GetString(reader.GetOrdinal("Mota")),
                        SoLuongToiDaCuTri = reader.GetInt32(reader.GetOrdinal("SoLuongToiDaCuTri")),
                        SoLuongToiDaUngCuVien = reader.GetInt32(reader.GetOrdinal("SoLuongToiDaUngCuVien")),
                        SoLuotBinhChonToiDa = reader.GetInt32(reader.GetOrdinal("SoLuotBinhChonToiDa"))
                    });
                }        
            }
            return list;
        }

        //19. Kiểm tra xem cử tri đã bỏ phiếu trong kỳ bầu cử chưa
        public async Task<bool> _CheckVoterHasVoted(string ID_cutri, DateTime ngayBD, MySqlConnection connect){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connect.State != System.Data.ConnectionState.Open)
                await connect.OpenAsync();
            try{
                const string sql = @"
                SELECT GhiNhan
                FROM trangthaibaucu
                WHERE ngayBD = @ngayBD AND ID_CuTri =@ID_CuTri;";

                using (var command = new MySqlCommand(sql, connect)){
                    command.Parameters.AddWithValue("@ngayBD", ngayBD);
                    command.Parameters.AddWithValue("@ID_CuTri", ID_cutri);

                    using var reader = await command.ExecuteReaderAsync();
                    if(await reader.ReadAsync()){
                        string ghinhan = reader.GetString(reader.GetOrdinal("GhiNhan"));
                        Console.WriteLine($">>Ghi nhận bỏ phiếu: {ghinhan}");
                        if(ghinhan.Equals("1")) return true; //Đã bỏ phiếu rồi
                    }
                    return  false;  //Chưa bỏ phiếu
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

        //Lấy mã đơn vị dựa trên mã cử tri
        public async Task<string> _getMaDonVi(string ID_CuTri,  MySqlConnection connect){
             //Kiểm tra trạng thái kết nối trước khi mở
            if(connect.State != System.Data.ConnectionState.Open)
                await connect.OpenAsync();
            
            try{
                const string sql = @"
                SELECT ID_DonViBauCu 
                FROM trangthaibaucu 
                WHERE ID_CuTri = @ID_CuTri;";
                string ID_DonViBauCu = null;

                using (var command = new MySqlCommand(sql, connect)){
                    command.Parameters.AddWithValue("@ID_CuTri", ID_CuTri);

                    using var reader = await command.ExecuteReaderAsync();
                    if(await reader.ReadAsync()){
                        ID_DonViBauCu = reader.GetString(reader.GetOrdinal("ID_DonViBauCu"));
                    }
                }
                return ID_DonViBauCu;

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

        //Lấy ma danh muc ứng cử dựa trên mã cử tri
        public async Task<string> _getMaDanhMucUngCu(string ID_CuTri,  MySqlConnection connect){
             //Kiểm tra trạng thái kết nối trước khi mở
            if(connect.State != System.Data.ConnectionState.Open)
                await connect.OpenAsync();
            
            try{
                const string sql = @"
                SELECT dm.ID_Cap
                FROM trangthaibaucu tt 
                JOIN donvibaucu dv ON tt.ID_DonViBauCu = dv.ID_DonViBauCu
                JOIN danhmucungcu dm ON dm.ID_DonViBauCu = dv.ID_DonViBauCu
                WHERE tt.ID_CuTri =  @ID_CuTri;";
                string ID_Cap = null;

                using (var command = new MySqlCommand(sql, connect)){
                    command.Parameters.AddWithValue("@ID_CuTri", ID_CuTri);

                    using var reader = await command.ExecuteReaderAsync();
                    if(await reader.ReadAsync()){
                        ID_Cap = reader.GetString(reader.GetOrdinal("ID_Cap"));
                    }
                }
                return ID_Cap;

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

        //Lấy danh sách ID cử tri theo ngày bầu cử
        public async Task<List<VoterID_DTO>> _getVoterID_ListBasedOnElection(string ngayBD, MySqlConnection connect){
            try{
                //Kiểm tra trạng thái kết nối trước khi mở
                if(connect.State != System.Data.ConnectionState.Open)
                    await connect.OpenAsync();

                var list = new List<VoterID_DTO>();
                const string sql = @"
                SELECT ID_CuTri
                FROM trangthaibaucu
                WHERE ngayBD =@ngayBD;";

                using(var command = new MySqlCommand(sql, connect)){
                    command.Parameters.AddWithValue("@ngayBD", ngayBD);
                    
                    using var reader = await command.ExecuteReaderAsync();
                    while(await reader.ReadAsync()){
                        list.Add(new VoterID_DTO{
                            ID_CuTri = reader.GetString(reader.GetOrdinal("ID_CuTri"))
                        });
                    }
                    return list;
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