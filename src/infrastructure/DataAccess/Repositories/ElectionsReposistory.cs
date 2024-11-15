using BackEnd.core.Entities;
using BackEnd.src.infrastructure.DataAccess.Context;
using MySql.Data.MySqlClient;
using BackEnd.src.web_api.DTOs;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using System.Data;
using System.Globalization;
using BackEnd.src.core.Interfaces;
using System.Numerics;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using BackEnd.src.infrastructure.Services;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class ElectionsReposistory : IDisposable,IElectionsRepository
    {
        private readonly DatabaseContext _context;
        private readonly IPaillierServices _PaillierServices;
        private readonly IListOfPositionRepository _listOfPositionRepository;
        public IConfiguration Configuration{get;}
         private readonly GoogleDriveService _googleDriveService;

        //Khởi tạo
        public ElectionsReposistory(
            DatabaseContext context,
            IListOfPositionRepository listOfPositionRepository,
            IPaillierServices paillierServices,
            IConfiguration configuration,
            GoogleDriveService googleDriveService
        ){
            _context = context;
            _listOfPositionRepository = listOfPositionRepository;
            _PaillierServices = paillierServices;
            Configuration = configuration;
            _googleDriveService = googleDriveService;
        } 

        //Hủy
        public void Dispose() => _context.Dispose();

        //Liệt kê
        public async Task<List<ElectionDto>> _GetListOfElections(){
            var list = new List<ElectionDto>();
 
            using var connection = await _context.Get_MySqlConnection();
            using var command = new MySqlCommand("SELECT * FROM kybaucu", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new ElectionDto{
                    ngayBD = reader.GetDateTime(reader.GetOrdinal("ngayBD")).ToString("dd/MM/yyyy HH:mm:ss"),
                    ngayKT = reader.GetDateTime(reader.GetOrdinal("ngayKT")).ToString("dd/MM/yyyy HH:mm:ss"),
                    NgayKT_UngCu = reader.GetDateTime(reader.GetOrdinal("NgayKT_UngCu")).ToString("dd/MM/yyyy HH:mm:ss"),
                    SoLuongToiDaCuTri = reader.GetInt32(reader.GetOrdinal("SoLuongToiDaCuTri")),
                    SoLuongToiDaUngCuVien = reader.GetInt32(reader.GetOrdinal("SoLuongToiDaUngCuVien")),
                    SoLuotBinhChonToiDa = reader.GetInt32(reader.GetOrdinal("SoLuongToiDaUngCuVien")),
                    TenKyBauCu = reader.GetString(reader.GetOrdinal("TenKyBauCu")),
                    MoTa = reader.GetString(reader.GetOrdinal("MoTa")),
                    ID_Cap = reader.GetInt32(reader.GetOrdinal("ID_Cap")),
                    CongBo = reader.GetString(reader.GetOrdinal("CongBo")),
                });
            }
            return list;
        }

        //Liệt kê các kỳ bầu cử trong tương lai
        public async Task<List<ElectionDto>> _GetListOfFutureElections(){
            var list = new List<ElectionDto>();

            using var connection = await _context.Get_MySqlConnection();
            using var command = new MySqlCommand("SELECT * FROM kybaucu WHERE NOW() <= ngayBD;", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new ElectionDto{
                    ngayBD = reader.GetDateTime(reader.GetOrdinal("ngayBD")).ToString("dd/MM/yyyy HH:mm:ss"),
                    ngayKT = reader.GetDateTime(reader.GetOrdinal("ngayKT")).ToString("dd/MM/yyyy HH:mm:ss"),
                    NgayKT_UngCu = reader.GetDateTime(reader.GetOrdinal("NgayKT_UngCu")).ToString("dd/MM/yyyy HH:mm:ss"),
                    SoLuongToiDaCuTri = reader.GetInt32(reader.GetOrdinal("SoLuongToiDaCuTri")),
                    SoLuongToiDaUngCuVien = reader.GetInt32(reader.GetOrdinal("SoLuongToiDaUngCuVien")),
                    SoLuotBinhChonToiDa = reader.GetInt32(reader.GetOrdinal("SoLuongToiDaUngCuVien")),
                    TenKyBauCu = reader.GetString(reader.GetOrdinal("TenKyBauCu")),
                    MoTa = reader.GetString(reader.GetOrdinal("MoTa")),
                    ID_Cap = reader.GetInt32(reader.GetOrdinal("ID_Cap")),
                    
                });
            }
            return list;
        }

        //Thêm
        public async Task<int> _AddElections(ElectionTempDTO kybaucu){
            using var connection = await _context.Get_MySqlConnection();
            using var transaction =await connection.BeginTransactionAsync();
            try{
                //Lấy thời điểm hiện tại Và kiểm tra nếu ngày bắt đầu để rỗng thì nó lấy thời điểm hiện tại
                DateTime currentDay = DateTime.Now;
                kybaucu.ngayBD = kybaucu.ngayBD != null ? kybaucu.ngayBD : currentDay; 
                
                //Kiểm tra ID cấp của danh mục bầu cử có tồn tại không
                bool checkID_cap = await _listOfPositionRepository._CheckIfTheCodeIsInTheListOfPosition(kybaucu.ID_Cap,connection);
                if(checkID_cap == false) return 0;

                //Tạo khóa ngoại và khóa riêng tư
                (BigInteger N, BigInteger G, BigInteger lamda, BigInteger muy) 
                    = _PaillierServices.GenerateKey_public_private(kybaucu.SoLuongToiDaCuTri, kybaucu.SoLuongToiDaCuTri +1, kybaucu.SoLuotBinhChonToiDa);
                
                //Thực hiện thêm kỳ bầu cử           
                const string Input = @"
                    INSERT INTO kybaucu(ngayBD,ngayKT,NgayKT_UngCu,TenKyBauCu,MoTa,SoLuongToiDaCuTri,SoLuongToiDaUngCuVien,SoLuotBinhChonToiDa,ID_Cap) 
                    VALUES(@ngayBD,@ngayKT,@NgayKT_UngCu,@TenKyBauCu,@MoTa,@SoLuongToiDaCuTri,@SoLuongToiDaUngCuVien,@SoLuotBinhChonToiDa,@ID_Cap);";
                
                //Thực hiện thêm khóa ngoai
                const string InputPublicKey = @"
                    INSERT INTO khoa(NgayTao,N,G,path_PK,ngayBD)
                    VALUES(@NgayTao,@N,@G,@path_PK,@ngayBD);
                ";

                //Cấc biến hỗ trợ
                string now = currentDay.ToString("yyyy-mm-dd_HH-mm-ss");
                string directoryPath  = Configuration["AppSettings:PrivateKeyPath"];   //Thư mục lưu khóa chính
                string filePk = $@"\{now}.txt";
                string filePath = Path.Combine(directoryPath, filePk);
                Console.WriteLine($"\nĐường dẫn: {filePath}");
                Console.WriteLine($"\nĐường dẫn đầy đủ: {directoryPath+filePath}");
                
                //Kiểm tra xem thư mục có tồn tại không
                if(!Directory.Exists(directoryPath)){
                    await transaction.RollbackAsync();
                    return -1;
                }

                //Lưu vào csdl
                using (var commandAdd = new MySqlCommand( Input + InputPublicKey, connection)){
                    commandAdd.Parameters.AddWithValue("@ngayBD",kybaucu.ngayBD);
                    commandAdd.Parameters.AddWithValue("@ngayKT",kybaucu.ngayKT);
                    commandAdd.Parameters.AddWithValue("@NgayKT_UngCu",kybaucu.NgayKT_UngCu);
                    commandAdd.Parameters.AddWithValue("@TenKyBauCu",kybaucu.TenKyBauCu);
                    commandAdd.Parameters.AddWithValue("@MoTa",kybaucu.MoTa);
                    commandAdd.Parameters.AddWithValue("@SoLuongToiDaCuTri",kybaucu.SoLuongToiDaCuTri);
                    commandAdd.Parameters.AddWithValue("@SoLuongToiDaUngCuVien",kybaucu.SoLuongToiDaUngCuVien);
                    commandAdd.Parameters.AddWithValue("@SoLuotBinhChonToiDa",kybaucu.SoLuotBinhChonToiDa);
                    commandAdd.Parameters.AddWithValue("@NgayTao",currentDay);
                    commandAdd.Parameters.AddWithValue("@N",N);
                    commandAdd.Parameters.AddWithValue("@G",G);
                    commandAdd.Parameters.AddWithValue("@path_PK",directoryPath+$"{now}.txt");
                    commandAdd.Parameters.AddWithValue("@ID_Cap",kybaucu.ID_Cap);

                    await commandAdd.ExecuteNonQueryAsync();
                }

                //Ghi nội dung vào tệp tin 
                string content = $"{lamda},{muy}";
                File.WriteAllText(directoryPath+filePath, content);

                //Thêm vào google drive
                bool addFileToDrive = await _googleDriveService.UploadFileAsync(directoryPath+filePath, $"{now}.txt", "text/plain");
                if(!addFileToDrive){
                    await transaction.RollbackAsync();
                    return -2;
                }

                await transaction.CommitAsync();
                return 1; 
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

        //Lấy theo ID
        public async Task<ElectionDto> _GetElectionsBy_ID(string id){
            using var connection = await _context.Get_MySqlConnection();

            const string sql = $@"
                SELECT * FROM kybaucu 
                WHERE ngayBD = @ngayBD;";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ngayBD",id);

            using var reader = await command.ExecuteReaderAsync();
            if(await reader.ReadAsync()){
                return new ElectionDto{
                    ngayBD = reader.GetDateTime(reader.GetOrdinal("ngayBD")).ToString("dd/MM/yyyy HH:mm:ss"),
                    ngayKT = reader.GetDateTime(reader.GetOrdinal("ngayKT")).ToString("dd/MM/yyyy HH:mm:ss"),
                    TenKyBauCu = reader.GetString(reader.GetOrdinal("TenKyBauCu")),
                    MoTa = reader.GetString(reader.GetOrdinal("MoTa")),
                    SoLuongToiDaCuTri = reader.GetInt32(reader.GetOrdinal("SoLuongToiDaCuTri")),
                    SoLuongToiDaUngCuVien = reader.GetInt32(reader.GetOrdinal("SoLuongToiDaUngCuVien")),
                    SoLuotBinhChonToiDa = reader.GetInt32(reader.GetOrdinal("SoLuotBinhChonToiDa"))
                };
            }

            return null;
        }

        //Sửa
        public async Task<bool> _EditElectionsBy_ID(string ID, Elections Elections){
            using var connection = await _context.Get_MySqlConnection();

            //Cập nhật
            const string sqlupdate = @"UPDATE kybaucu 
            SET ngayBD = @ngayBDMoi,
            NgayKT_UngCu = @NgayKT_UngCu, 
            TenKyBauCu = @TenKyBauCu, 
            ngayKT = @ngayKT, 
            MoTa = @MoTa,
            SoLuongToiDaCuTri = @SoLuongToiDaCuTri,
            SoLuongToiDaUngCuVien = @SoLuongToiDaUngCuVien,
            SoLuotBinhChonToiDa = @SoLuotBinhChonToiDa
            WHERE ngayBD = @ngayBDCu ;";

            using( var command = new MySqlCommand(sqlupdate, connection)){
                command.Parameters.AddWithValue("@ngayBDMoi",Elections.ngayBD);
                command.Parameters.AddWithValue("@TenKyBauCu",Elections.TenKyBauCu);
                command.Parameters.AddWithValue("@ngayKT",Elections.ngayKT);
                command.Parameters.AddWithValue("@NgayKT_UngCu",Elections.NgayKT_UngCu);
                command.Parameters.AddWithValue("@MoTa",Elections.MoTa);
                command.Parameters.AddWithValue("@ngayBDCu",ID);
                command.Parameters.AddWithValue("@SoLuongToiDaCuTri",Elections.SoLuongToiDaCuTri);
                command.Parameters.AddWithValue("@SoLuongToiDaUngCuVien",Elections.SoLuongToiDaUngCuVien);
                command.Parameters.AddWithValue("@SoLuotBinhChonToiDa",Elections.SoLuotBinhChonToiDa);
                
                //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
                int rowAffected = await command.ExecuteNonQueryAsync();
                return rowAffected > 0;
            }
        }

        //Xóa
        public async Task<bool> _DeleteElectionsBy_ID(string ID){
            using var connection = await _context.Get_MySqlConnection();

            const string sqlupdate = @"
                DELETE FROM kybaucu
                WHERE ngayBD = @ngayBD;";
            
            //Xóa khóa dự trên ngày BD
            const string sqlupdate2 = @"
                DELETE FROM khoa
                WHERE ngayBD = @ngayBD;";

            //Lấy đường dẫn khóa riêng tư - và xóa tệp tin dựa trên đường dẫn
            string pathPK = await _getPrivateKeyPathBasedOnElectionDate(ID,connection);
            
            //Xóa tệp tin privatekey trên google drive
            int lastIndex = pathPK.LastIndexOf('\\');
            string fileName = pathPK.Substring(lastIndex + 1);
            Console.WriteLine($"Tên tệp tin cần xóa: {fileName}");
            bool deleteFileOnDrive = await _googleDriveService.deleteFileAssync(fileName, "1lfcfq-fMMOMXQlXwSLYUGkpJDVDoMM7U");
            if(!deleteFileOnDrive){
                Console.WriteLine("Error: Xóa tệp tin trên google drive không thành công");
                return false;
            }

            //Xóa tệp tin trên usb
            if(File.Exists(pathPK)){
                File.Delete(pathPK);
            }else{
                Console.WriteLine("Error: Không tìm thấy tệp tin privatekey");
                return false;
            }

            using var command = new MySqlCommand(sqlupdate2 + sqlupdate, connection);
            command.Parameters.AddWithValue("@ngayBD",ID);
        
            //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
            int rowAffected = await command.ExecuteNonQueryAsync();
            Console.WriteLine($"Số hàng tác động khi xóa:{rowAffected}");
            return rowAffected > 0;
        }

        //Kiểm tra xem ngày bầu cử có tồn tại không
        public async Task<bool> _CheckIfElectionTimeExists(DateTime ngayBD, MySqlConnection connection){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            const string sql = "SELECT COUNT(ngayBD) FROM kybaucu WHERE ngayBD=@ngayBD;";
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ngayBD",ngayBD);
                
                int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                return count > 0;
            }
        }

        //Lấy số lượng cử tri tối đa theo kỳ bầu cử
        public async Task<int> _MaximumNumberOfVoters(DateTime ngayBD, MySqlConnection connection){
            //Kiểm tra kết nối
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
            
            const string sql = "SELECT SoLuongToiDaCuTri FROM kybaucu WHERE ngayBD =@ngayBD; ";
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ngayBD",ngayBD);
                using var reader = await command.ExecuteReaderAsync();

                if(await reader.ReadAsync())
                    return reader.GetInt32(reader.GetOrdinal("SoLuongToiDaCuTri"));
            }
            return -1;
        }

        //Lấy số lượng ứng cử viên tối đa theo kỳ bầu cử
        public async Task<int> _MaximumNumberOfCandidates(DateTime ngayBD, MySqlConnection connection){
            const string sql = "SELECT SoLuongToiDaUngCuVien FROM kybaucu WHERE ngayBD =@ngayBD; ";
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ngayBD",ngayBD);
                using var reader = await command.ExecuteReaderAsync();

                if(await reader.ReadAsync())
                    return reader.GetInt32(reader.GetOrdinal("SoLuongToiDaUngCuVien"));
            }
            return -1;
        }

        //Lầy ngày kết thúc đăng ký ứng cử dựa trên ngày bắt đầu bầu cử
        public async Task<DateTime?> _GetRegistrationClosingDate(DateTime ngayBD, MySqlConnection connection){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
            
            try{
                const string sql = "SELECT NgayKT_UngCu FROM kybaucu WHERE ngayBD =@ngayBD; ";
                using(var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ngayBD",ngayBD);
                    using var reader = await command.ExecuteReaderAsync();

                    if(await reader.ReadAsync()){
                        return reader.GetDateTime(reader.GetOrdinal("NgayKT_UngCu")); 
                    }
                        
                }
                return null;
            }catch(MySqlException ex){ 
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }catch(Exception){
                throw;
            } 
        }

        //Lấy số lượt bình chọn tối đa theo kỳ bầu cử
        public async Task<int> _MaximumNumberOfVotes(DateTime ngayBD, MySqlConnection connection){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
            
            const string sql = "SELECT SoLuotBinhChonToiDa FROM kybaucu WHERE ngayBD =@ngayBD; ";
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ngayBD",ngayBD);
                using var reader = await command.ExecuteReaderAsync();

                if(await reader.ReadAsync())
                    return reader.GetInt32(reader.GetOrdinal("SoLuotBinhChonToiDa"));
            }
            return -1;
        }
        
         //Lấy số lượng cử tri hiện tại đang có trong kỳ bầu cử
        public async Task<int> _GetCurrentVoterCountByElection(DateTime ngayBD, MySqlConnection connection){
            const string sql = @"
            SELECT COUNT(ct.ID_CuTri) SoLuongHienTai
            FROM trangthaibaucu tt 
            JOIN cutri ct ON ct.ID_CuTri = tt.ID_CuTri
            JOIN kybaucu ky ON ky.ngayBD = tt.ngayBD
            WHERE ky.ngayBD = @ngayBD;";
            
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ngayBD",ngayBD);
                using var reader = await command.ExecuteReaderAsync();

                if(await reader.ReadAsync())
                    return reader.GetInt32(reader.GetOrdinal("SoLuongHienTai"));
            }
            return -1;
        } 

        //Lấy số lượng ứng cử viên hiện tại đang có trong kỳ bầu cử
        public async Task<int> _GetCurrentCandidateCountByElection(DateTime ngayBD, MySqlConnection connection){
            const string sql = @"
            SELECT COUNT(ucv.ID_ucv) SoLuongHienTai
            FROM ketquabaucu kq  
            JOIN ungcuvien ucv on kq.ID_ucv = ucv.ID_ucv
            JOIN kybaucu ky ON  ky.ngayBD = kq.ngayBD
            WHERE ky.ngayBD = @ngayBD;";
            
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ngayBD",ngayBD);
                using var reader = await command.ExecuteReaderAsync();

                if(await reader.ReadAsync())
                    return reader.GetInt32(reader.GetOrdinal("SoLuongHienTai"));
            }
            return -1;
        }

        //Trả về ngày kết thúc của kỳ bầu cử dựa trên thời điểm bắt đầu
        public async Task<TimeOfTheElectionDTO> _GetTimeOfElection(string ngayBD, MySqlConnection connection){
            
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            //Tìm ngày bắt đầu có không nếu không thì trả về null
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime votingDay = DateTime.ParseExact(ngayBD,"yyyy-MM-dd HH:mm:ss",provider);
            var check_ngayBD = await _CheckIfElectionTimeExists(votingDay,connection);
            if(!check_ngayBD) return null;

            TimeOfTheElectionDTO result = new TimeOfTheElectionDTO();
            const string sql = @"
            SELECT ngayBD, ngayKT FROM kybaucu 
            WHERE ngayBD = @ngayBD";

            using (var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ngayBD", ngayBD);
                using var reader = await command.ExecuteReaderAsync();
                
                if(await reader.ReadAsync()){
                    result.ngayBD = reader.GetDateTime(reader.GetOrdinal("ngayBD"));
                    result.ngayKT = reader.GetDateTime(reader.GetOrdinal("ngayKT"));
                    
                    return result;
                }
                return null;// Không tồn tại
            }
        }

        //So sánh số lượng ứng cử viên tại kỳ bầu cử hiện tại với số lượng ứng cử viên quy định từ trước theo ngày bầu cử
        public async Task<int> _CompareCurrentNumberCandidateWithSpecifieldNumber(DateTime ngayBD, MySqlConnection connection, MySqlTransaction transaction){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            try{
                const string sql = @"
                SELECT 
                case when ky.SoLuongToiDaUngCuVien >(
                SELECT COUNT(kq.ngayBD)
                FROM ketquabaucu kq
                WHERE kq.ngayBD = @ngayBD
                ) then 1
                ELSE 0
                END AS Result
                FROM kybaucu ky
                WHERE ky.ngayBD = @ngayBD;";

                using (var command = new MySqlCommand(sql,connection)){
                    command.Parameters.AddWithValue("@ngayBD",ngayBD);

                    using var reader = await command.ExecuteReaderAsync();
                    int count = 0;
                    if(await reader.ReadAsync()){
                        count = reader.GetInt32(reader.GetOrdinal("Result"));    
                    }
                    return count;
                }

            }catch(MySqlException ex){ 
                Console.WriteLine("Error: " + ex.Message);
                if(transaction.Connection != null)
                    await transaction.RollbackAsync();
                return -1;
            }catch(Exception){
                await transaction.RollbackAsync();
                throw;
            }   
        }

        //Lấy danh sách ID và tên ứng cử viên được sắp xếp dựa trên ngày bắt đầu bầu cử
        public async Task<List<CandidateNamesBasedOnElectionDateDto>> _GetListCandidateNamesBasedOnElections(DateTime ngayBD){
           var connection = await _context.Get_MySqlConnection();
            List<CandidateNamesBasedOnElectionDateDto> result = new List<CandidateNamesBasedOnElectionDateDto>();
            
            const string sql = @"
            SELECT ucv.ID_ucv, nd.HoTen
            FROM ungcuvien ucv JOIN nguoidung nd ON ucv.ID_user = nd.ID_user
            JOIN ketquabaucu kq ON kq.ID_ucv = ucv.ID_ucv
            WHERE kq.ngayBD = @ngayBD
            ORDER BY nd.HoTen";

            using(var command =  new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ngayBD", ngayBD);
                using var reader = await command.ExecuteReaderAsync();
                
                while(await reader.ReadAsync()){
                    result.Add(new CandidateNamesBasedOnElectionDateDto{
                        ID_ucv = reader.GetString(reader.GetOrdinal("ID_ucv")),
                        HoTen = reader.GetString(reader.GetOrdinal("HoTen"))
                    });
                }
            }

            return result;
        }

        //Lấy private key path dựa trên ngày bắt đầu
        public async Task<string> _getPrivateKeyPathBasedOnElectionDate(string ngayBD, MySqlConnection connection){
            try{
                //Kiểm tra xem ngày bắt đầu có tồn tại không
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime votingDay = DateTime.ParseExact(ngayBD,"yyyy-MM-dd HH:mm:ss",provider);
                bool check_ngayBD = await _CheckIfElectionTimeExists(votingDay, connection);
                if(!check_ngayBD) return null;

                const string sql = @"
                SELECT path_PK
                FROM khoa 
                WHERE ngayBD = @ngayBD;";
                using (var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ngayBD",ngayBD);
                    using var reader = await command.ExecuteReaderAsync();

                    if(await reader.ReadAsync()){
                        return reader.GetString(reader.GetOrdinal("path_PK"));
                    }
                    return null;
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

        //Kiểm tra xem kỳ bầu cử đã công bố kết quả chưa
        public async Task<bool> _checkResultAnnouncement(string ngayBD,  MySqlConnection connection){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            try{
                //Kiểm tra ngày bắt đầu bầu cử có tồn tại không
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime votingDay = DateTime.ParseExact(ngayBD,"yyyy-MM-dd HH:mm:ss",provider);
                bool check_ngayBD = await _CheckIfElectionTimeExists(votingDay, connection);
                if(!check_ngayBD) return false;

                const string sql = @"SELECT CongBo FROM kybaucu WHERE ngayBD = @ngayBD;";
                using(var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ngayBD", ngayBD);
                    using var reader = await command.ExecuteReaderAsync();
                    if(await reader.ReadAsync()){
                        string congBo = reader.GetString(reader.GetOrdinal("CongBo"));
                        return congBo.Equals("1");
                    }
                    return false;
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

        //Lấy danh sách ID và tên ứng cử viên được sắp xếp dựa trên ngày bắt đầu bầu cử
        public async Task<List<CandidateNamesBasedOnElectionDateDto>> _GetListCandidateNamesBasedOnElections_OtherID_ucv(DateTime ngayBD,MySqlConnection connection){
           try{
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            List<CandidateNamesBasedOnElectionDateDto> result = new List<CandidateNamesBasedOnElectionDateDto>();
            const string sql = @"
            SELECT kq.ID_ucv, nd.HoTen  
            FROM kybaucu kbc 
            JOIN ketquabaucu kq ON kbc.ngayBD = kq.ngayBD
            JOIN ungcuvien ucv ON ucv.ID_ucv = kq.ID_ucv
            JOIN nguoidung nd ON nd.ID_user = ucv.ID_user
            WHERE kq.ngayBD = @ngayBD 
            ORDER BY ucv.ID_ucv;";

            using(var command =  new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ngayBD", ngayBD);
                using var reader = await command.ExecuteReaderAsync();
                
                while(await reader.ReadAsync()){
                    result.Add(new CandidateNamesBasedOnElectionDateDto{
                        ID_ucv = reader.GetString(reader.GetOrdinal("ID_ucv")),
                    });
                }
            }

            return result;
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

        //Cập nhật số lượt bình chọn, tỉ lệ bình chọn cho ID_ucv tương ứng dựa trên ngày bắt đầu
        public async Task<bool> _updateVoteCountAndVotePercentage(DateTime ngayBD, int SoLuotBinhChon, float tiLeBinhChon, string ID_ucv ,MySqlConnection connection){
            try{
                //Kiểm tra trạng thái kết nối trước khi mở
                if(connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                const string sql = @"
                UPDATE kybaucu
                SET SoLuotBinhChon =@SoLuotBinhChon, TyLeBinhChon =@ 
                WHERE ngayBD =@ngayBD AND ID_ucv =@ID_ucv;";

                using(var commad = new MySqlCommand(sql, connection)){
                    commad.Parameters.AddWithValue("@SoLuotBinhChon",SoLuotBinhChon);
                    commad.Parameters.AddWithValue("@tiLeBinhChon",tiLeBinhChon);
                    commad.Parameters.AddWithValue("@ngayBD",ngayBD);
                    commad.Parameters.AddWithValue("@ID_ucv",ID_ucv);

                    int rowAffected = await commad.ExecuteNonQueryAsync();
                    return rowAffected > 0;
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

        //Cập nhật đã công bố kết quả bầu cử rồi dựa trên kỳ bầu cử
        public async Task<bool> _UpdateResultAnnouncementElectionBasedOnElectionDate(DateTime ngayBD, MySqlConnection connection){
            try{
                //Kiểm tra trạng thái kết nối trước khi mở
                if(connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                const string sql = @"
                UPDATE kybaucu
                SET CongBo = '1'
                WHERE ngayBD = @ngayBD;";

                using(var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ngayBD",ngayBD);
                    
                    int rowAffected = await command.ExecuteNonQueryAsync();
                    return rowAffected > 0;
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

        //Lấy danh sách chi tiết các kỳ bầu cử
        public async Task<List<CadreJoinedForElectionDTO>> _getDetailsListOfElectionBassedOnYear(string year){
            try{
                using var connection = await _context.Get_MySqlConnection();
                
                var list = new List<CadreJoinedForElectionDTO>();
                const string sql = @"
                SELECT kbc.ngayBD, kbc.ngayKT, kbc.TenKyBauCu, kbc.ID_Cap,
                    kbc.MoTa, kbc.SoLuongToiDaCuTri,  kbc.SoLuongToiDaUngCuVien, 
                    kbc.SoLuotBinhChonToiDa, kbc.CongBo, dm.TenCapUngCu, 
                    dv.ID_DonViBauCu, dv.TenDonViBauCu,
                    COUNT(DISTINCT kq.ID_ucv) AS SoLuongUngCuVienHienTai,
                    COUNT(DISTINCT tt.ID_CuTri) AS SoLuongCuTriHienTai,
                    COUNT(DISTINCT hd.ID_CanBO) AS SoLuongCanBoHienTai
                FROM kybaucu kbc
                INNER JOIN danhmucungcu dm ON dm.ID_Cap = kbc.ID_Cap
                INNER JOIN donvibaucu dv ON dv.ID_DonViBauCu = dm.ID_DonViBauCu
                LEFT JOIN ketquabaucu kq ON kq.ngayBD = kbc.ngayBD
                LEFT JOIN trangthaibaucu tt ON tt.ngayBD = kbc.ngayBD
                LEFT JOIN hoatdong hd ON hd.ngayBD = kbc.ngayBD
                WHERE YEAR(kbc.ngayBD) = @year
                GROUP BY 
                    kbc.ngayBD, kbc.ngayKT, kbc.TenKyBauCu, 
                    kbc.MoTa, kbc.SoLuongToiDaCuTri, kbc.SoLuongToiDaUngCuVien, 
                    kbc.SoLuotBinhChonToiDa, kbc.CongBo, dm.TenCapUngCu, 
                    dv.ID_DonViBauCu, dv.TenDonViBauCu;";

                using(var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@year", year);

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
                            ID_DonViBauCu = reader.GetInt32(reader.GetOrdinal("ID_DonViBauCu")),
                            ID_Cap = reader.GetInt32(reader.GetOrdinal("ID_Cap")),
                            TenDonViBauCu = reader.GetString(reader.GetOrdinal("TenDonViBauCu")),
                            SoLuongCuTriHienTai = reader.GetInt32(reader.GetOrdinal("SoLuongCuTriHienTai")),
                            SoLuongUngCuVienHienTai = reader.GetInt32(reader.GetOrdinal("SoLuongUngCuVienHienTai")),
                            SoLuongCanBoHienTai = reader.GetInt32(reader.GetOrdinal("SoLuongCanBoHienTai"))
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

        //Lấy danh sách các cử tri chưa tham dự bầu cử
        public async Task<List<UserNotYetJoinedDTO>> _listOfVotersWhoHaveNotYetParticipatedElection(string ngayBD){
            try{
                using var connection = await _context.Get_MySqlConnection();
                
                var list = new List<UserNotYetJoinedDTO>();
                const string sql = @"
                SELECT ct.ID_CuTri, nd.HoTen, nd.HinhAnh
                FROM cutri ct 
                INNER JOIN nguoidung nd ON nd.ID_user = ct.ID_user 
                WHERE NOT EXISTS ( 
                	SELECT 1
                	FROM trangthaibaucu tt
                	WHERE tt.ID_CuTri = ct.ID_CuTri 
                	AND tt.ngayBD = @ngayBD
                );";

                using(var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ngayBD", ngayBD);

                    using var reader = await command.ExecuteReaderAsync();
                    while(await reader.ReadAsync()){
                        list.Add(new UserNotYetJoinedDTO{
                            ID_object = reader.GetString(reader.GetOrdinal("ID_CuTri")),
                            HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                            HinhAnh = reader.GetString(reader.GetOrdinal("HinhAnh")),
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

        //Lấy danh sách ứng cử viên chưa tham dự bầu cử
        public async Task<List<UserNotYetJoinedDTO>> _listOfCandidatesWhoHaveNotYetParticipatedElection(string ngayBD){
            try{
                using var connection = await _context.Get_MySqlConnection();
                
                var list = new List<UserNotYetJoinedDTO>();
                const string sql = @"
                SELECT ucv.ID_ucv, nd.HoTen, nd.HinhAnh
                FROM ungcuvien ucv
                INNER JOIN nguoidung nd ON ucv.ID_user = nd.ID_user
                WHERE NOT EXISTS(
                	SELECT 1 FROM ketquabaucu kq
                    WHERE kq.ID_ucv = ucv.ID_ucv 
                    AND kq.ngayBD = @ngayBD
                );";

                using(var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ngayBD", ngayBD);

                    using var reader = await command.ExecuteReaderAsync();
                    while(await reader.ReadAsync()){
                        list.Add(new UserNotYetJoinedDTO{
                            ID_object = reader.GetString(reader.GetOrdinal("ID_ucv")),
                            HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                            HinhAnh = reader.GetString(reader.GetOrdinal("HinhAnh")),
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

        //Lấy danh sách cán bộ chưa tham dự bầu cử
        public async Task<List<UserNotYetJoinedDTO>> _listOfCadresWhoHaveNotYetParticipatedElection(string ngayBD){
            try{
                using var connection = await _context.Get_MySqlConnection();
                
                var list = new List<UserNotYetJoinedDTO>();
                const string sql = @"
                SELECT cb.ID_CanBo, nd.HoTen, nd.HinhAnh
                FROM canbo cb
                INNER JOIN nguoidung nd ON nd.ID_user = cb.ID_user 
                WHERE NOT EXISTS ( 
                	SELECT 1
                    FROM hoatdong hd
                    WHERE hd.ID_canbo = cb.ID_canbo 
                    AND hd.ngayBD = @ngayBD
                );";

                using(var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ngayBD", ngayBD);

                    using var reader = await command.ExecuteReaderAsync();
                    while(await reader.ReadAsync()){
                        list.Add(new UserNotYetJoinedDTO{
                            ID_object = reader.GetString(reader.GetOrdinal("ID_CanBo")),
                            HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                            HinhAnh = reader.GetString(reader.GetOrdinal("HinhAnh")),
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

    }
}