using log4net;
using BackEnd.src.core.Common;
using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.infrastructure.Services;
using BackEnd.src.web_api.DTOs;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class CadreRepository: UserRepository, IDisposable, ICadreRepository
    {
        private readonly DatabaseContext _context;
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program)); 
        private readonly IElectionsRepository _ElectionsRepository; //kỳ bầu cử
        private readonly IProfileRepository _profileRepository;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IBoardRepository _boardRepository;      //Ban
        private readonly IPositionsRepository _positionsRepository;//Chức vụ
        private readonly IWorkPlaceRepository _workPlaceRepository;//Nơi làm việc

        //Khởi tạo
        public CadreRepository(
            DatabaseContext context,
            CloudinaryService cloudinaryService,
            IProfileRepository profileRepository, 
            IElectionsRepository ElectionsRepository,
            IPositionsRepository positionsRepository,
            IWorkPlaceRepository workPlaceRepository,
            IBoardRepository boardRepository
        ) : base(context, cloudinaryService){
            _context=context;
            _cloudinaryService = cloudinaryService;
            _profileRepository = profileRepository;
            _ElectionsRepository = ElectionsRepository;
            _positionsRepository = positionsRepository;
            _workPlaceRepository = workPlaceRepository;
            _boardRepository = boardRepository;
        }
        //Hủy
        public new void Dispose() => _context.Dispose();

        //-1 .Kiểm tra ID cử tri có tồn tại không
        public async Task<bool> _CheckCadreExists(string ID, MySqlConnection connection){
            const string sqlCount = "SELECT COUNT(ID_CanBo) FROM canbo WHERE ID_CanBo = @ID_CanBo";
            using(var command = new MySqlCommand(sqlCount, connection)){
                command.Parameters.AddWithValue("@ID_CanBo",ID);
                
                int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                if(count < 1) return false;
            }
            return true;
        }

        //0. Lấy ID người dùng dựa trên ID cán bộ
        public async Task<string> GetIDUserBaseOnIDCadre(string id, MySqlConnection connection){
            string ID_user = null;
            const string sql = "SELECT ID_user FROM canbo WHERE ID_CanBo = @ID_CanBo;";
            
            using (var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_CanBo", id);

                using var reader = await command.ExecuteReaderAsync();
                if(await reader.ReadAsync())
                    ID_user = reader.GetString(reader.GetOrdinal("ID_user"));
            }
            return ID_user;
        }

        //1.Thêm .Sẵn cập nhật luôn hồ sơ người dùng
        public async Task<int> _AddCadre(CadreDto Cadre, IFormFile fileAnh){
            using var connect = await _context.Get_MySqlConnection();

            //Kiểm tra kết nối
            if(connect.State != System.Data.ConnectionState.Open )
                await connect.OpenAsync();
            using var transaction =await connect.BeginTransactionAsync();

            try{
                //Chỉnh lại vai trò cho cán bộ
                Cadre.RoleID = 8;

                //Thêm thông tin cơ sở
                var FillInBasicInfo = await _AddUserWithConnect(Cadre, fileAnh, connect, transaction);
                
                //Nếu có lỗi thì in ra
                if(FillInBasicInfo is int result && result <=0){
                    Console.WriteLine($">>>> FillInBasicInfo: {FillInBasicInfo}");
                    await transaction.RollbackAsync();
                    return result;
                }
                
                //Lấy ID người dùng vừa tạo và tạo ID cán bộ
                string ID_CanBo = RandomString.CreateID(),
                        ID_user = FillInBasicInfo.ToString();

                //Ngược lại thêm cán bộ  
                const string sql = "INSERT INTO canbo(ID_CanBo,GhiChu,NgayCongTac,ID_user) VALUES(@ID_CanBo,@GhiChu,@NgayCongTac,@ID_user);";
                
                const string sqlCadreDegree = @"
                INSERT INTO chitiettrinhdohocvancanbo(ID_TrinhDo, ID_CanBo)
                VALUES (@ID_TrinhDo, @ID_CanBo);";

                using(var command = new MySqlCommand(sql + sqlCadreDegree, connect)){
                    command.Parameters.AddWithValue("@ID_CanBo", ID_CanBo);
                    command.Parameters.AddWithValue("@ID_user", ID_user);
                    command.Parameters.AddWithValue("@GhiChu", Cadre.GhiChu);
                    command.Parameters.AddWithValue("@NgayCongTac", Cadre.NgayCongTac);
                    command.Parameters.AddWithValue("@ID_TrinhDo", Cadre.ID_TrinhDo);

                    await command.ExecuteNonQueryAsync();
                }

                //Tạo hồ sơ
                bool AddProfile = await _profileRepository._AddProfile(ID_user, "1", connect);
                if(!AddProfile){
                    Console.WriteLine("Lỗi khi tạo hồ sơ cho cán bộ");
                    await transaction.RollbackAsync();
                    return -100;
                }

                await transaction.CommitAsync();
                return 1;
            }catch(MySqlException ex){
                Console.WriteLine($"Error in mySQL Message: {ex.Message}");
                Console.WriteLine($"Error in mySQL StackTrace: {ex.StackTrace}");
                Console.WriteLine($"Error in mySQL Code: {ex.Code}");

                await transaction.RollbackAsync();
                return -5;
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

        //2.1 Lấy thông tin người dùng theo ID
        public async Task<CadreDto> _GetCadreBy_ID(string IDCadre){
            using var connection = await _context.Get_MySqlConnection();
            
            const string sql = @"
            SELECT * 
            FROM canbo ct 
            INNER JOIN nguoidung nd ON nd.ID_user = ct.ID_user 
            WHERE ct.ID_CanBo = @ID_CanBo;";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID_CanBo", IDCadre);

            using var reader = await command.ExecuteReaderAsync();
            if(await reader.ReadAsync()){
                return new CadreDto{
                    ID_CanBo = reader.GetString(reader.GetOrdinal("ID_CanBo")),
                    NgayCongTac = reader.GetDateTime(reader.GetOrdinal("NgayCongTac")),
                    GhiChu = reader.GetString(reader.GetOrdinal("GhiChu")),
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

        //2.2 Kiểm tra xem ID cán bộ có tồn tại không
        public async Task<bool> _CheckIfTheCadreCodeExists(string ID_CanBo, MySqlConnection connection){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            const string sql = "SELECT COUNT(ID_CanBo) FROM canbo WHERE ID_CanBo=@ID_CanBo;";
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_CanBo",ID_CanBo);
                
                int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                return count > 0;
            } 
        }

        //3. Sửa thông tin cán bộ dựa trên ID cán bộ
        public async Task<int> _EditCadreBy_ID(string IDCadre ,CadreDto Cadre){
            using var connection = await _context.Get_MySqlConnection();

            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync(); 

            using var transaction = await connection.BeginTransactionAsync();

            try{
                //Lấy ID_user từ cán bộ
                string ID_user = await GetIDUserBaseOnIDCadre(IDCadre, connection);
                if(string.IsNullOrEmpty(ID_user))
                    return -5;
                
                //Kiểm tra thông tin đầu vào không được trùng
                int KiemTraThongTinTrungNhau = await CheckForDuplicateUserInformation(1, Cadre, connection);
                if(KiemTraThongTinTrungNhau <= 0){
                    Console.WriteLine("Thông tin đầu vào trùng nhau");
                    return KiemTraThongTinTrungNhau;
                }

                //Kiểm tra đầu vào không được null
                if(CheckUserInformationIsNotEmpty(Cadre) == false){
                    Console.WriteLine("Thông tin đầu vào chưa điền đây đủ");
                    return -4;
                }

                //Cập nhật thông tin cán bộ - Và cập nhật phần tài khoảng cán bộ, nếu SDT thay đổi
                const string sqlNguoiDung = @"
                    UPDATE nguoidung 
                    SET HoTen = @HoTen, GioiTinh=@GioiTinh, NgaySinh=@NgaySinh,
                        DiaChiLienLac=@DiaChiLienLac, SDT=@SDT, ID_DanToc=@ID_DanToc,
                        Email=@Email, RoleID=@RoleID
                    WHERE ID_user = @ID_user;";

                const string sqlCadre = @"UPDATE canbo SET GhiChu = @GhiChu ,NgayCongTac=@NgayCongTac WHERE ID_CanBo = @ID_CanBo;";

                using(var command1 = new MySqlCommand($"{sqlNguoiDung} {sqlCadre}", connection)){
                    command1.Parameters.AddWithValue("@ID_user", ID_user);
                    command1.Parameters.AddWithValue("@NgayCongTac", Cadre.NgayCongTac);
                    command1.Parameters.AddWithValue("@ID_CanBo", IDCadre);
                    command1.Parameters.AddWithValue("@GhiChu", Cadre.GhiChu);
                    command1.Parameters.AddWithValue("@HoTen", Cadre.HoTen);
                    command1.Parameters.AddWithValue("@GioiTinh", Cadre.GioiTinh);
                    command1.Parameters.AddWithValue("@NgaySinh", Cadre.NgaySinh);
                    command1.Parameters.AddWithValue("@DiaChiLienLac", Cadre.DiaChiLienLac);
                    command1.Parameters.AddWithValue("@SDT", Cadre.SDT);
                    command1.Parameters.AddWithValue("@Email", Cadre.Email);
                    command1.Parameters.AddWithValue("@RoleID", Cadre.RoleID);
                    command1.Parameters.AddWithValue("@ID_DanToc", Cadre.ID_DanToc);
                
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

        //4. Lấy tất cả thông tin cán bộ
        public async Task<List<CadreInfoDTO>> _GetListOfCadre(){
            try{
                var list = new List<CadreInfoDTO>();
                using var connection = await _context.Get_MySqlConnection();
                if(connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                const string sql = @"
                SELECT nd.ID_user, nd.HoTen, nd.GioiTinh, nd.NgaySinh, nd.DiaChiLienLac, 
                    nd.Email, nd.Email, nd.SDT, nd.HinhAnh, nd.PublicID, nd.ID_DanToc, nd.RoleID,
                    cb.ID_CanBo, cb.NgayCongTac, cb.GhiChu
                FROM nguoidung nd 
                INNER JOIN canbo cb ON cb.ID_user = nd.ID_user;";
                using var command = new MySqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();
                
                while(await reader.ReadAsync()){
                    list.Add(new CadreInfoDTO{
                        ID_CanBo =reader.GetString(reader.GetOrdinal("ID_CanBo")),
                        GhiChu =reader.IsDBNull(reader.GetOrdinal("GhiChu")) ? null:reader.GetString(reader.GetOrdinal("GhiChu")),
                        NgayCongTac =reader.IsDBNull(reader.GetOrdinal("NgayCongTac")) ? null:reader.GetDateTime(reader.GetOrdinal("NgayCongTac")),  
                        ID_user = reader.GetString(reader.GetOrdinal("ID_user")),
                        HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                        GioiTinh = reader.GetString(reader.GetOrdinal("GioiTinh")),
                        NgaySinh = reader.GetDateTime(reader.GetOrdinal("NgaySinh")).ToString("dd-MM-yyyy"),
                        DiaChiLienLac = reader.GetString(reader.GetOrdinal("DiaChiLienLac")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        SDT = reader.GetString(reader.GetOrdinal("SDT")),
                        HinhAnh = reader.GetString(reader.GetOrdinal("HinhAnh")),
                        ID_DanToc = reader.GetInt32(reader.GetOrdinal("ID_DanToc")),
                        RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                        PublicID = reader.GetString(reader.GetOrdinal("PublicID"))
                    });
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

        //5. Đặt mật khẩu cán bộ
        public async Task<bool> _SetCadrePassword(string id,string newPwd){
            using var connection = await _context.Get_MySqlConnection();

            //Băm mật khẩu
            newPwd = Argon2.Hash(newPwd);

            //Cập nhật mật khẩu dựa trên ID
            const string sql = @"
            UPDATE taikhoan tk 
            JOIN nguoidung nd ON nd.sdt = tk.TaiKhoan
            JOIN canbo ct ON nd.ID_user = ct.ID_user
            SET tk.MatKhau = @MatKhau
            WHERE ct.ID_CanBo = @ID_CanBo;";
            
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_CanBo", id);
                command.Parameters.AddWithValue("@MatKhau", newPwd);
                
                int rowAffect = await command.ExecuteNonQueryAsync();
                if(rowAffect < 0)
                    return false;
            }
            return true;
        }

        //6. cán bộ thay đổi mật khẩu
        public async Task<int> _ChangeCadrePassword(string id, string oldPwd, string newPwd){
            using var connect = await _context.Get_MySqlConnection();

            //Lấy mật khẩu đã băm của cán bộ
            string hashedPwd = null;
            const string sqlGetHashedPwd = @"
            SELECT tk.MatKhau 
            FROM taikhoan tk
            INNER JOIN nguoidung nd ON tk.TaiKhoan = nd.SDT
            INNER JOIN canbo ct ON ct.ID_user = nd.ID_user
            WHERE ct.ID_CanBo = @ID_CanBo;";

            using(var command0 = new MySqlCommand(sqlGetHashedPwd,connect)){
                command0.Parameters.AddWithValue("@ID_CanBo", id);
                
                using var reader = await command0.ExecuteReaderAsync();
                if(await reader.ReadAsync())
                    hashedPwd = reader.GetString(reader.GetOrdinal("MatKhau"));
            }

            //Nếu tìm không thấy ID cán bộ thì trả về 0
            if(string.IsNullOrEmpty(hashedPwd)) return 0;

            //So sánh 2 mật khẩu đã băm và mật khẩu cũ
            if(Argon2.Verify(hashedPwd, oldPwd) != true) return -1;

            //Băm mật khẩu
            newPwd = Argon2.Hash(newPwd);

            //Truy xuất ID cán bộ rồi cập nhật lại
            const string sqlUpdatePwd = @"
            UPDATE taikhoan tk 
            JOIN nguoidung nd ON nd.sdt = tk.TaiKhoan
            JOIN canbo ct ON nd.ID_user = ct.ID_user
            SET tk.MatKhau = @MatKhau
            WHERE ct.ID_CanBo = @ID_CanBo;";

            using(var command1 = new MySqlCommand(sqlUpdatePwd, connect)){
                command1.Parameters.AddWithValue("@ID_CanBo", id);
                command1.Parameters.AddWithValue("@MatKhau", newPwd);
                
                int rowAffect = await command1.ExecuteNonQueryAsync();
                if(rowAffect < 0)
                    return -2;
            }

            return 1;
        }

        //7. Xóa tài khoảng người dùng dựa trên ID cư tri
        public async Task<bool> _DeleteCadreBy_ID(string IDCadre){
            using var connect = await _context.Get_MySqlConnection();
            using var transaction =await connect.BeginTransactionAsync();

            try{
                //Truy xuất lấy ID cán bộ
                string ID_user = await GetIDUserBaseOnIDCadre(IDCadre, connect);
                Console.WriteLine($"ID_user: {ID_user}");
                if(string.IsNullOrEmpty(ID_user)) return false;

                //Xóa tài khoản dựa trên ID người dùng
                const string sqlDeleteCadre = @"
                DELETE FROM nguoidung WHERE ID_user = @ID_user;";
                using(var command1 = new MySqlCommand(sqlDeleteCadre, connect)){
                    command1.Parameters.AddWithValue("@ID_user", ID_user);
                    
                    int rowAffect = await command1.ExecuteNonQueryAsync();
                    if(rowAffect < 0){
                        await transaction.RollbackAsync();
                        return false;
                    }
                }

                await transaction.CommitAsync();
                return true;
            }catch(MySqlException ex){
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Error Code: {ex.Code}");
                Console.WriteLine($"Error Source: {ex.Source}");
                Console.WriteLine($"Error HResult: {ex.HResult}");
                await transaction.RollbackAsync();
                throw;
            }
            catch(Exception ex){
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Error Source: {ex.Source}");
                Console.WriteLine($"Error StackTrace: {ex.StackTrace}");
                Console.WriteLine($"Error TargetSite: {ex.TargetSite}");
                Console.WriteLine($"Error HResult: {ex.HResult}");
                Console.WriteLine($"Error InnerException: {ex.InnerException}");
                await transaction.RollbackAsync();
                throw;
            }
        }

        //8. Lấy thông tin cán bộ và kèm theo tài khoản
        public async Task<List<CadreDto>> _GetListOfCadresAndAccounts(){
            var list = new List<CadreDto>();
            using var connection = await _context.Get_MySqlConnection();
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            const string sql = @"
            SELECT nd.ID_user,nd.HoTen,nd.GioiTinh,nd.NgaySinh,nd.DiaChiLienLac,nd.CCCD,nd.Email,nd.SDT,
            nd.HinhAnh,nd.PublicID,nd.ID_DanToc,nd.RoleID, ct.ID_CanBo, ct.GhiChu, ct.NgayCongTac
            ,tk.TaiKhoan,tk.MatKhau,tk.BiKhoa,tk.LyDoKhoa,tk.NgayTao,tk.SuDung,tk.RoleID 
            FROM nguoidung nd 
            INNER JOIN taikhoan tk ON tk.TaiKhoan = nd.SDT 
            JOIN canbo ct ON ct.ID_user = nd.ID_user";
            using var command = new MySqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new CadreDto{
                    ID_CanBo = reader.GetString(reader.GetOrdinal("ID_CanBo")),
                    GhiChu =reader.IsDBNull(reader.GetOrdinal("GhiChu")) ? null:reader.GetString(reader.GetOrdinal("GhiChu")),
                    NgayCongTac =reader.IsDBNull(reader.GetOrdinal("NgayCongTac")) ? null:reader.GetDateTime(reader.GetOrdinal("NgayCongTac")),  
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

        //9. cán bộ phản hồi
        public async Task<bool> _CadreSubmitReport(SendReportDto reportDto){
            using var connect = await _context.Get_MySqlConnection();
            using var transaction =await connect.BeginTransactionAsync();

            try{
                //Kiểm tra cử tri có tồn tại hay không
                bool CheckExists = await _CheckCadreExists(reportDto.IDSender, connect);
                if(!CheckExists) return false;

                //Gửi báo cáo
                const string sqlSend = @"INSERT INTO phanhoicanbo(ID_CanBo,ThoiDiem,Ykien) 
                VALUES(@ID_CanBo,@ThoiDiem,@Ykien);";

                using(var command = new MySqlCommand(sqlSend, connect)){
                    command.Parameters.AddWithValue("@ID_CanBo", reportDto.IDSender);
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

        //10.thêm danh sách id cán bộ vào kỳ bầu cử
        public async Task<int>  _AddListCadresToTheElection(CadreListInElectionDto cadreListInElectionDto){
            using var connection = await _context.Get_MySqlConnection();
            try{
                int dem  = 0;
                
                //kiểm tra xem kỳ bầu cử có tồn tại không
                bool checkExitsElection = await _ElectionsRepository._CheckIfElectionTimeExists(cadreListInElectionDto.ngayBD, connection);
                if(!checkExitsElection)
                    return -2;

                foreach(var cadre in cadreListInElectionDto.ListID_canbo){
                    //Kiểm tra ID cán bộ đã tồn tại chưa
                    bool checkExistsCadre = await _CheckCadreExists(cadre, connection);
                    
                    //Kiểm tra xem cán bộ đã trực tại kỳ bầu cử này chưa
                    bool checkTheCadresWhoAttendedTheElection = await _workPlaceRepository._CheckTheCadresWhoAttendedTheElection(cadre, cadreListInElectionDto.ngayBD, connection);

                    const string sql = @"INSERT INTO hoatdong(ID_CanBo,ngayBD) VALUES(@ID_CanBo,@ngayBD);";
                    const string sqlTrangThaiBauCu = @"
                        INSERT INTO trangthaibaucu(ID_CanBo,ID_DonViBauCu,ngayBD,GhiNhan)
                        VALUES(@ID_CanBo,@ID_DonViBauCu,@ngayBD,@GhiNhan);";

                    if(checkExistsCadre && !checkTheCadresWhoAttendedTheElection){
                        using (var command = new MySqlCommand(sql + sqlTrangThaiBauCu, connection)){
                            command.Parameters.AddWithValue("@ID_CanBo", cadre);
                            command.Parameters.AddWithValue("@ngayBD", cadreListInElectionDto.ngayBD);
                            command.Parameters.AddWithValue("@ID_DonViBauCu", cadreListInElectionDto.ID_DonViBauCu);
                            command.Parameters.AddWithValue("@GhiNhan", "0");

                            await command.ExecuteNonQueryAsync();
                            dem++;
                        }
                    }
                }
               
                //Kiểm tra xem
                return dem;
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

        //11.Thông tin các kỳ bầu cử mà cán bộ đã tham dự để trực
        public async Task<List<CadreJoinedForElectionDTO>> _getListOfCadreJoinedForElection(string ID_CanBo){
            try{
                using var connection = await _context.Get_MySqlConnection();
                if(connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                //Kiểm tra cán bộ có tồn tại không
                bool checkExistsCadre = await _CheckCadreExists(ID_CanBo, connection);
                if(!checkExistsCadre)
                    return null;
                
                var list = new List<CadreJoinedForElectionDTO>();
                const string sql = @"
                SELECT kbc.ngayBD, kbc.ngayKT, kbc.TenKyBauCu, 
                kbc.MoTa, kbc.SoLuongToiDaCuTri, kbc.SoLuongToiDaUngCuVien, 
                kbc.SoLuotBinhChonToiDa, kbc.CongBo, dm.TenCapUngCu, dv.TenDonViBauCu
                FROM kybaucu kbc 
                JOIN hoatdong hd ON hd.ngayBD = kbc.ngayBD
                JOIN danhmucungcu dm ON dm.ID_Cap = kbc.ID_Cap
                JOIN donvibaucu dv ON dv.ID_DonViBauCu = dm.ID_DonViBauCu
                WHERE hd.ID_canbo = @ID_canbo;";

                using(var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ID_canbo", ID_CanBo);

                    using var reader = await command.ExecuteReaderAsync();
                    while(await reader.ReadAsync()){
                        list.Add(new CadreJoinedForElectionDTO{
                            ngayBD = reader.GetDateTime(reader.GetOrdinal("ngayBD")),
                            ngayKT = reader.GetDateTime(reader.GetOrdinal("ngayKT")),
                            TenKyBauCu = reader.GetString(reader.GetOrdinal("TenKyBauCu")),
                            MoTa = reader.GetString(reader.GetOrdinal("TenKyBauCu")),
                            SoLuongToiDaCuTri = reader.GetInt32(reader.GetOrdinal("SoLuongToiDaCuTri")),
                            SoLuongToiDaUngCuVien = reader.GetInt32(reader.GetOrdinal("SoLuongToiDaUngCuVien")),
                            SoLuotBinhChonToiDa = reader.GetInt32(reader.GetOrdinal("SoLuotBinhChonToiDa")),
                            CongBo =reader.GetString(reader.GetOrdinal("CongBo")),
                            TenCapUngCu = reader.GetString(reader.GetOrdinal("TenCapUngCu")),
                            TenDonViBauCu = reader.GetString(reader.GetOrdinal("TenDonViBauCu")),
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

        //Lấy danh sách ID cán bộ theo ngày bầu cử
        public async Task<List<CadreID_DTO>> _getCadreID_ListBasedOnElection(string ngayBD, MySqlConnection connect){
            try{
                //Kiểm tra trạng thái kết nối trước khi mở
                if(connect.State != System.Data.ConnectionState.Open)
                    await connect.OpenAsync();

                var list = new List<CadreID_DTO>();
                const string sql = @"
                SELECT ID_CanBo
                FROM hoatdong
                WHERE ngayBD =@ngayBD;";

                using(var command = new MySqlCommand(sql, connect)){
                    command.Parameters.AddWithValue("@ngayBD", ngayBD);
                    
                    using var reader = await command.ExecuteReaderAsync();
                    while(await reader.ReadAsync()){
                        list.Add(new CadreID_DTO{
                            ID_CanBo = reader.GetString(reader.GetOrdinal("ID_CanBo"))
                        });
                    }
                    return list;
                }

            }catch(MySqlException ex){
                _log.Error($"Error message: {ex.Message}");
                _log.Error($"Error Code: {ex.Code}");
                _log.Error($"Error Source: {ex.Source}");
                _log.Error($"Error HResult: {ex.HResult}");
                throw;
            }
            catch(Exception ex){
                _log.Error($"Error message: {ex.Message}");
                _log.Error($"Error Source: {ex.Source}");
                _log.Error($"Error StackTrace: {ex.StackTrace}");
                _log.Error($"Error TargetSite: {ex.TargetSite}");
                _log.Error($"Error HResult: {ex.HResult}");
                _log.Error($"Error InnerException: {ex.InnerException}");
                throw;
            }
        }

        //19. Kiểm tra xem cán bộ đã bỏ phiếu trong kỳ bầu cử chưa
        public async Task<bool> _CheckCadreHasVoted(string ID_CanBo, DateTime ngayBD, MySqlConnection connect){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connect.State != System.Data.ConnectionState.Open)
                await connect.OpenAsync();
                
            try{
                const string sql = @"
                SELECT GhiNhan
                FROM trangthaibaucu
                WHERE ngayBD = @ngayBD AND ID_CanBo =@ID_CanBo;";

                using (var command = new MySqlCommand(sql, connect)){
                    command.Parameters.AddWithValue("@ngayBD", ngayBD);
                    command.Parameters.AddWithValue("@ID_CanBo", ID_CanBo);

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
    }
}