using BackEnd.src.infrastructure.DataAccess.Context;
using MySql.Data.MySqlClient;
using BackEnd.src.web_api.DTOs;
using Microsoft.AspNetCore.Http;
using BackEnd.src.infrastructure.Services;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.core.Common;
using Isopoh.Cryptography.Argon2;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class CandidateRepository : UserRepository, IDisposable, ICandidateRepository
    {
        private readonly DatabaseContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IListOfPositionRepository _listOfPositionRepository;
        private readonly IElectionsRepository _ElectionsRepository;
        private readonly IProfileRepository _profileRepository;
        
        //Khởi tạo
        public CandidateRepository(
            DatabaseContext context,CloudinaryService cloudinaryService,
            IListOfPositionRepository listOfPositionRepository,
            IElectionsRepository electionsRepository,
            IProfileRepository profileRepository
        ): base(context, cloudinaryService){
            _context=context;
            _cloudinaryService = cloudinaryService;
            _listOfPositionRepository = listOfPositionRepository;
            _ElectionsRepository = electionsRepository;
            _profileRepository = profileRepository;
        }
        //Hủy
        public new void Dispose() => _context.Dispose();


        //-2 .Kiểm tra ID ứng cử viên có tồn tại không
        public async Task<bool> _CheckCandidateExists(string ID, MySqlConnection connection){
            const string sqlCount = "SELECT COUNT(ID_ucv) FROM ungcuvien WHERE ID_ucv = @ID_ucv";
            using(var command = new MySqlCommand(sqlCount, connection)){
                command.Parameters.AddWithValue("@ID_ucv",ID);
                
                int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                if(count < 1) return false;
            }
            return true;
        }

        //-1. Kiểm tra điều kiện lưu thông tin vào bảng kết quả bầu cử. (Việc này cũng nhưng việc đăng ký ứng cử viên vào vị trí ứng cử)
        public async Task<int> _CheckInformationBeforeEnterInTableElectionResults(CandidateDto Candidate, MySqlConnection connection){
            if(! await _listOfPositionRepository._CheckIfTheCodeIsInTheListOfPosition(Candidate.ID_Cap, connection)) return -5;
            if(! await _ElectionsRepository._CheckIfElectionTimeExists(Candidate.ngayBD.GetValueOrDefault(), connection)) return -6;
            return 1;
        }

        //0. Lấy ID người dùng dựa trên ID ứng cử viên
        public async Task<string> GetIDUserBaseOnIDUngCuVien(string id, MySqlConnection connection){
            string ID_user = null;
            const string sql = "SELECT ID_user FROM UngCuVien WHERE ID_ucv = @ID_ucv;";
            
            using (var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_ucv", id);

                using var reader = await command.ExecuteReaderAsync();
                if(await reader.ReadAsync())
                    ID_user = reader.GetString(reader.GetOrdinal("ID_user"));
            }
            return ID_user;
        }

        //1.Thêm (Kèm thêm chức vụ cho ứng cử viên)
        public async Task<int> _AddCandidate(CandidateDto Candidate, IFormFile fileAnh){
            using var connect = await _context.Get_MySqlConnection();

            //Kiểm tra kết nối
            if(connect.State != System.Data.ConnectionState.Open )
                await connect.OpenAsync();
            
            using var transaction =await connect.BeginTransactionAsync();
            bool transactionCommitted = false; //Kiểm tra xem đã rolleback chưa

            try{
                Candidate.RoleID = 2;   //Gán vai trò mặc định khi tạo ứng cử viên

                //Thêm thông tin cơ sở
                var FillInBasicInfo = await _AddUserWithConnect(Candidate, fileAnh, connect, transaction);
                
                //Kiểm tra đầu vào để đăng ký thông tin ứng cử viên vào bảng kết quả bầu cử (Nếu có lỗi thì quăng ra)
                int CheckInputInElectionResults = await _CheckInformationBeforeEnterInTableElectionResults(Candidate, connect);
                if(CheckInputInElectionResults < 0){
                    return CheckInputInElectionResults;
                }

                //Nếu có lỗi thì in ra
                if(FillInBasicInfo is int result && result <=0){
                    return result;
                }
                
                //Lấy ID người dùng vừa tạo và tạo ID ứng cử viên
                string ID_ucv = RandomString.CreateID(),
                        ID_user = FillInBasicInfo.ToString();

                //Ngược lại thêm ứng cử viên, thêm phần vào bảng kết quả bầu cử
                const string sqlCandidate = "INSERT INTO UngCuVien(ID_ucv,TrangThai,ID_user) VALUES(@ID_ucv,@TrangThai,@ID_user);";
                const string sqlElectionResult = @"
                INSERT INTO ketquabaucu(SoLuotBinhChon,ThoiDiemDangKy,TyLeBinhChon,ngayBD,ID_ucv,ID_Cap) 
                VALUES(@SoLuotBinhChon,@ThoiDiemDangKy,@TyLeBinhChon,@ngayBD,@ID_ucv,@ID_Cap);";
                
                using(var command = new MySqlCommand($"{sqlCandidate} {sqlElectionResult}", connect)){
                    command.Parameters.AddWithValue("@ID_ucv", ID_ucv);
                    command.Parameters.AddWithValue("@ID_user", ID_user);
                    command.Parameters.AddWithValue("@TrangThai", Candidate.TrangThai);
                    command.Parameters.AddWithValue("@SoLuotBinhChon", 0);
                    command.Parameters.AddWithValue("@ThoiDiemDangKy", DateTime.Now);
                    command.Parameters.AddWithValue("@TyLeBinhChon", 0f);
                    command.Parameters.AddWithValue("@ngayBD", Candidate.ngayBD);
                    command.Parameters.AddWithValue("@ID_Cap", Candidate.ID_Cap);

                    await command.ExecuteNonQueryAsync();
                }

                //Tạo hồ sơ
                bool AddProfile = await _profileRepository._AddProfile(ID_user, "1", connect);
                if(!AddProfile){
                    Console.WriteLine("Lỗi khi tạo hồ sơ cho ứng cử viên");
                    await transaction.RollbackAsync();
                    return -100;
                }

                await transaction.CommitAsync();
                transactionCommitted = true;
                return 1;
            }
            catch(Exception ex){
                if (!transactionCommitted){
                    try{
                        await transaction.RollbackAsync();
                    }
                    catch (Exception rollbackEx){
                        Console.WriteLine($"Rollback Exception: {rollbackEx.Message}");
                    }
                }
                // Log lỗi và ném lại exception để controller xử lý
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return -100;
            }
        }

        //2. Lấy thông tin người dùng theo ID
        public async Task<CandidateDto> _GetCandidateBy_ID(string IDCandidate){
            using var connection = await _context.Get_MySqlConnection();
            
            const string sql = @"
            SELECT * 
            FROM UngCuVien ct 
            INNER JOIN nguoidung nd ON nd.ID_user = ct.ID_user 
            WHERE ct.ID_ucv = @ID_ucv;";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID_ucv", IDCandidate);

            using var reader = await command.ExecuteReaderAsync();
            if(await reader.ReadAsync()){
                return new CandidateDto{
                    ID_ucv = reader.GetString(reader.GetOrdinal("ID_ucv")),
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

        //3. Sửa thông tin ứng cử viên dựa trên ID ứng cử viên
        public async Task<int> _EditCandidateBy_ID(string IDCandidate ,CandidateDto Candidate){
            using var connection = await _context.Get_MySqlConnection();

            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync(); 

            using var transaction = await connection.BeginTransactionAsync();

            try{
                //Lấy ID_user từ ứng cử viên
                string ID_user = await GetIDUserBaseOnIDUngCuVien(IDCandidate, connection);
                if(string.IsNullOrEmpty(ID_user))
                    return -5;
                
                //Kiểm tra thông tin đầu vào không được trùng
                int KiemTraThongTinTrungNhau = await CheckForDuplicateUserInformation(1, Candidate, connection);
                if(KiemTraThongTinTrungNhau <= 0){
                    Console.WriteLine("Thông tin đầu vào trùng nhau");
                    return KiemTraThongTinTrungNhau;
                }

                //Kiểm tra đầu vào không được null
                if(CheckUserInformationIsNotEmpty(Candidate) == false){
                    Console.WriteLine("Thông tin đầu vào chưa điền đây đủ");
                    return -4;
                }

                //Cập nhật thông tin ứng cử viên - Và cập nhật phần tài khoảng ứng cử viên, nếu SDT thay đổi
                const string sqlNguoiDung = @"
                    UPDATE nguoidung 
                    SET HoTen = @HoTen, GioiTinh=@GioiTinh, NgaySinh=@NgaySinh,
                        DiaChiLienLac=@DiaChiLienLac, CCCD=@CCCD, SDT=@SDT, ID_DanToc=@ID_DanToc,
                        Email=@Email, RoleID=@RoleID
                    WHERE ID_user = @ID_user;";
                const string sqlCandidateAccount = @"
                UPDATE taikhoan
                SET taikhoan = @SDT
                WHERE Taikhoan = @Taikhoan;";
                const string sqlCandidate = @"UPDATE ungcuvien SET TrangThai = @TrangThai WHERE ID_ucv = @ID_ucv;";

                using(var command1 = new MySqlCommand($"{sqlNguoiDung} {sqlCandidateAccount} {sqlCandidate}", connection)){
                    command1.Parameters.AddWithValue("@ID_user", ID_user);
                    command1.Parameters.AddWithValue("@ID_ucv", IDCandidate);
                    command1.Parameters.AddWithValue("@TrangThai", Candidate.TrangThai);
                    command1.Parameters.AddWithValue("@HoTen", Candidate.HoTen);
                    command1.Parameters.AddWithValue("@GioiTinh", Candidate.GioiTinh);
                    command1.Parameters.AddWithValue("@NgaySinh", Candidate.NgaySinh);
                    command1.Parameters.AddWithValue("@DiaChiLienLac", Candidate.DiaChiLienLac);
                    command1.Parameters.AddWithValue("@CCCD", Candidate.CCCD);
                    command1.Parameters.AddWithValue("@SDT", Candidate.SDT);
                    command1.Parameters.AddWithValue("@Taikhoan", Candidate.TaiKhoan);
                    command1.Parameters.AddWithValue("@Email", Candidate.Email);
                    command1.Parameters.AddWithValue("@RoleID", Candidate.RoleID);
                    command1.Parameters.AddWithValue("@ID_DanToc", Candidate.ID_DanToc);
                
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

        //4. Lấy tất cả thông tin ứng cử viên
        public async Task<List<CandidateDto>> _GetListOfCandidate(){
            var list = new List<CandidateDto>();
            using var connection = await _context.Get_MySqlConnection();
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            const string sql = @"
            SELECT * FROM nguoidung nd 
            INNER JOIN UngCuVien ct ON ct.ID_user = nd.ID_user";
            using var command = new MySqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new CandidateDto{
                    ID_ucv =reader.GetString(reader.GetOrdinal("ID_ucv")),
                    TrangThai =reader.GetString(reader.GetOrdinal("TrangThai")),  
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
                    PublicID = reader.GetString(reader.GetOrdinal("PublicID"))
                });
            }
            return list;
        }

        //5. Đặt mật khẩu ứng cử viên
        public async Task<bool> _SetCandidatePassword(string id,string newPwd){
            using var connection = await _context.Get_MySqlConnection();

            //Băm mật khẩu
            newPwd = Argon2.Hash(newPwd);

            //Cập nhật mật khẩu dựa trên ID
            const string sql = @"
            UPDATE taikhoan tk 
            JOIN nguoidung nd ON nd.sdt = tk.TaiKhoan
            JOIN UngCuVien ct ON nd.ID_user = ct.ID_user
            SET tk.MatKhau = @MatKhau
            WHERE ct.ID_ucv = @ID_ucv;";
            
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_ucv", id);
                command.Parameters.AddWithValue("@MatKhau", newPwd);
                
                int rowAffect = await command.ExecuteNonQueryAsync();
                if(rowAffect < 0)
                    return false;
            }
            return true;
        }

        //6. ứng cử viên thay đổi mật khẩu
        public async Task<int> _ChangeCandidatePassword(string id, string oldPwd, string newPwd){
            using var connect = await _context.Get_MySqlConnection();

            //Lấy mật khẩu đã băm của ứng cử viên
            string hashedPwd = null;
            const string sqlGetHashedPwd = @"
            SELECT tk.MatKhau 
            FROM taikhoan tk
            INNER JOIN nguoidung nd ON tk.TaiKhoan = nd.SDT
            INNER JOIN UngCuVien ct ON ct.ID_user = nd.ID_user
            WHERE ct.ID_ucv = @ID_ucv;";

            using(var command0 = new MySqlCommand(sqlGetHashedPwd,connect)){
                command0.Parameters.AddWithValue("@ID_ucv", id);
                
                using var reader = await command0.ExecuteReaderAsync();
                if(await reader.ReadAsync())
                    hashedPwd = reader.GetString(reader.GetOrdinal("MatKhau"));
            }

            //Nếu tìm không thấy ID ứng cử viên thì trả về 0
            if(string.IsNullOrEmpty(hashedPwd)) return 0;

            //So sánh 2 mật khẩu đã băm và mật khẩu cũ
            if(Argon2.Verify(hashedPwd, oldPwd) != true) return -1;

            //Băm mật khẩu
            newPwd = Argon2.Hash(newPwd);

            //Truy xuất ID ứng cử viên rồi cập nhật lại
            const string sqlUpdatePwd = @"
            UPDATE taikhoan tk 
            JOIN nguoidung nd ON nd.sdt = tk.TaiKhoan
            JOIN UngCuVien ct ON nd.ID_user = ct.ID_user
            SET tk.MatKhau = @MatKhau
            WHERE ct.ID_ucv = @ID_ucv;";

            using(var command1 = new MySqlCommand(sqlUpdatePwd, connect)){
                command1.Parameters.AddWithValue("@ID_ucv", id);
                command1.Parameters.AddWithValue("@MatKhau", newPwd);
                
                int rowAffect = await command1.ExecuteNonQueryAsync();
                if(rowAffect < 0)
                    return -2;
            }

            return 1;
        }

        //7. Xóa tài khoảng người dùng dựa trên ID cư tri
        public async Task<bool> _DeleteCandidateBy_ID(string IDCandidate){
            using var connect = await _context.Get_MySqlConnection();

            //Truy xuất lấy ID ứng cử viên
            string ID_user = await GetIDUserBaseOnIDUngCuVien(IDCandidate, connect);
            if(string.IsNullOrEmpty(ID_user)) return false;

            //Xóa tài khoản ứng cử viên trước rồi xóa tài khoản người dùng sau
            const string sqlDeleteCandidate = @"DELETE FROM UngCuVien WHERE ID_ucv = @ID_ucv;";
            using(var command1 = new MySqlCommand(sqlDeleteCandidate, connect)){
                command1.Parameters.AddWithValue("@ID_ucv", IDCandidate);
                
                int rowAffect = await command1.ExecuteNonQueryAsync();
                if(rowAffect < 0)
                    return false;
            }

            if(await _DeleteUserBy_ID_withConnection(ID_user, connect) == false)
                return false;

            return true;
        }

        //8. Lấy thông tin ứng cử viên và kèm theo tài khoản
        public async Task<List<CandidateDto>> _GetListOfCandidatesAndAccounts(){
            var list = new List<CandidateDto>();
            using var connection = await _context.Get_MySqlConnection();
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            const string sql = @"
            SELECT nd.ID_user,nd.HoTen,nd.GioiTinh,nd.NgaySinh,nd.DiaChiLienLac,nd.CCCD,nd.Email,nd.SDT,
            nd.HinhAnh,nd.PublicID,nd.ID_DanToc,nd.RoleID, ct.ID_ucv, ct.TrangThai 
            ,tk.TaiKhoan,tk.MatKhau,tk.BiKhoa,tk.LyDoKhoa,tk.NgayTao,tk.SuDung,tk.RoleID 
            FROM nguoidung nd 
            INNER JOIN taikhoan tk ON tk.TaiKhoan = nd.SDT 
            JOIN UngCuVien ct ON ct.ID_user = nd.ID_user";
            using var command = new MySqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new CandidateDto{
                    ID_ucv = reader.GetString(reader.GetOrdinal("ID_ucv")),
                    TrangThai = reader.GetString(reader.GetOrdinal("TrangThai")),
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

        //9. ứng cử viên phản hồi
        public async Task<bool> _CandidateSubmitReport(SendReportDto reportDto){
            using var connect = await _context.Get_MySqlConnection();
            using var transaction =await connect.BeginTransactionAsync();

            try{
                //Kiểm tra ứng cử viên có tồn tại hay không
                bool CheckExists = await _CheckCandidateExists(reportDto.IDSender, connect);
                if(!CheckExists) return false;

                //Gửi báo cáo
                const string sqlSend = @"INSERT INTO phanhoiungcuvien(ID_ucv,ThoiDiem,Ykien) 
                VALUES(@ID_ucv,@ThoiDiem,@Ykien);";

                using(var command = new MySqlCommand(sqlSend, connect)){
                    command.Parameters.AddWithValue("@ID_ucv", reportDto.IDSender);
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

        //10. Kiểm tra xem ứng cử viên đã tham giá bầu cử tại thời điểm đó chưa .Dựa trên mã ứng cử viên
        public async Task<bool> _CheckCandidatForA_ParicularElection(string ID_ucv, DateTime ngayBD ,MySqlConnection connection){
            
            //kiểm tra xem ứng cử viên có tồn tại không
            bool checkCandidateExist = await _CheckCandidateExists(ID_ucv, connection);
            if(!checkCandidateExist) return false;

            //Kiểm tra xem ứng cử viên đã tham gia chưa
            const string sql = @"
            SELECT COUNT(ID_ucv) FROM ketquabaucu 
            WHERE ngayBD = @ngayBD and ID_ucv = @ID_ucv;";
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ngayBD", ngayBD);
                command.Parameters.AddWithValue("@ID_ucv", ID_ucv);

                int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                if(count < 1 ) return false;
            }

            return true;
        } 

        //11.Thêm danh sách ứng cử viên vào cuộc bầu cử
        public async Task<int> _AddListCandidatesToTheElection(CandidateListInElectionDto CandidateListInElectionDto){
            using var connect = await _context.Get_MySqlConnection();
            
            //Kiểm tra xem ngày bầu cử có tồn tại không
            bool checkElectionExist = await _ElectionsRepository._CheckIfElectionTimeExists(CandidateListInElectionDto.ngayBD, connect);
            if(!checkElectionExist) return 0;

            //Kiểm tra xem vị trí ứng cử có hợp lệ không
            bool checkConstituencyExist = await _listOfPositionRepository._CheckIfTheCodeIsInTheListOfPosition(CandidateListInElectionDto.ID_Cap, connect);
            if(!checkConstituencyExist) return -1;

            //Nếu danh sách ứng cử viên vượt quá số lượt bình chọn tối đa đặt cho nó thì báo lỗi
            int sl_cuTriToiDa = await  _ElectionsRepository._MaximumNumberOfCandidates(CandidateListInElectionDto.ngayBD, connect);
            if(sl_cuTriToiDa < 0) return -2;

            //Lấy số lượng ứng cử viên trong kỳ bầu cử này ở hiện tại
            int sl_cuTriHienTai = await  _ElectionsRepository._GetCurrentCandidateCountByElection(CandidateListInElectionDto.ngayBD, connect);
            if(sl_cuTriToiDa < 0) return -4;

            if((sl_cuTriHienTai + CandidateListInElectionDto.listIDCandidate.Count) > sl_cuTriToiDa) return -3;

            //thêm từng ứng cử viên trong danh sách vào cuộc bầu cử
            const string sql = @"
            INSERT INTO ketquabaucu(SoLuotBinhChon,ThoiDiemDangKy,TyLeBinhChon,ngayBD,ID_ucv,ID_Cap) 
            VALUES(@SoLuotBinhChon,@ThoiDiemDangKy,@TyLeBinhChon,@ngayBD,@ID_ucv,@ID_Cap);";

            foreach(string Candidate in CandidateListInElectionDto.listIDCandidate){
                
                //Kiểm tra xem nếu ứng cử viên đã tồn tại trong cuộc bầu cử này thì bỏ qua hoặc ứng cử viên này không tồn tại trong danh sách thì bỏ qua
                bool checkInput = await _CheckCandidatForA_ParicularElection(Candidate,CandidateListInElectionDto.ngayBD,connect);
                
                if(!checkInput){
                    using(var command = new MySqlCommand(sql, connect)){
                        command.Parameters.AddWithValue("@SoLuotBinhChon", 0);
                        command.Parameters.AddWithValue("@ThoiDiemDangKy", DateTime.Now);
                        command.Parameters.AddWithValue("@TyLeBinhChon",0f);
                        command.Parameters.AddWithValue("@ngayBD", CandidateListInElectionDto.ngayBD);
                        command.Parameters.AddWithValue("@ID_ucv", Candidate);
                        command.Parameters.AddWithValue("@ID_Cap", CandidateListInElectionDto.ID_Cap);
                        
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            return 1;
        }

        //12. Xóa ứng cử viên khỏi kỳ bầu cử cụ thể
        public async Task<int> _RemoveCandidateOfElection(string Id_ucv, DateTime ngayBD){
            using var connect = await _context.Get_MySqlConnection();
            
            //Kiểm tra xem ứng cử viên có tồn tại không
            bool checkCandidateExist = await _CheckCandidateExists(Id_ucv, connect);
            if(!checkCandidateExist) return 0;

            //Kiểm tra xem ngày bầu cử có tồn tại không
            bool checkElectionExist = await _ElectionsRepository._CheckIfElectionTimeExists(ngayBD, connect);
            if(!checkElectionExist) return -1;

            const string sql = @"
            DELETE FROM kybaucu
            WHERE ID_ucv =@ID_ucv AND ngayBD =@ngayBD;";
            using(var command = new MySqlCommand(sql, connect)){
                command.Parameters.AddWithValue("@ID_ucv", Id_ucv);
                command.Parameters.AddWithValue("@ngayBD", ngayBD);
                
                int rowAffect = await command.ExecuteNonQueryAsync();
                if(rowAffect < 0)
                    return -2;
            }
            return 1;
        }

    }
}