using BackEnd.core.Entities;
using BackEnd.src.infrastructure.DataAccess.Context;
using MySql.Data.MySqlClient;
using BackEnd.src.web_api.DTOs;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using System.Data;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class ElectionsReposistory : IDisposable,IElectionsRepository
    {
        private readonly DatabaseContext _context;

        //Khởi tạo
        public ElectionsReposistory(
            DatabaseContext context
        ){
            _context = context;

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
                    MoTa = reader.GetString(reader.GetOrdinal("MoTa"))
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
                    MoTa = reader.GetString(reader.GetOrdinal("MoTa"))
                });
            }
            return list;
        }

        //Thêm
        public async Task<bool> _AddElections(Elections kybaucu){
            using var connection = await _context.Get_MySqlConnection();

            //Lấy thời điểm hiện tại Và kiểm tra nếu ngày bắt đầu để rỗng thì nó lấy thời điểm hiện tại
            DateTime currentDay = DateTime.Now;
            kybaucu.ngayBD = kybaucu.ngayBD != null ? kybaucu.ngayBD : currentDay; 
            
            //Thực hiện thêm            
            string Input = @"
                INSERT INTO kybaucu(ngayBD,ngayKT,NgayKT_UngCu,TenKyBauCu,MoTa,SoLuongToiDaCuTri,SoLuongToiDaUngCuVien,SoLuotBinhChonToiDa) 
                VALUES(@ngayBD,@ngayKT,@NgayKT_UngCu,@TenKyBauCu,@MoTa,@SoLuongToiDaCuTri,@SoLuongToiDaUngCuVien,@SoLuotBinhChonToiDa);";
            
            using (var commandAdd = new MySqlCommand(Input, connection)){
                commandAdd.Parameters.AddWithValue("@ngayBD",kybaucu.ngayBD);
                commandAdd.Parameters.AddWithValue("@ngayKT",kybaucu.ngayKT);
                commandAdd.Parameters.AddWithValue("@NgayKT_UngCu",kybaucu.NgayKT_UngCu);
                commandAdd.Parameters.AddWithValue("@TenKyBauCu",kybaucu.TenKyBauCu);
                commandAdd.Parameters.AddWithValue("@MoTa",kybaucu.MoTa);
                commandAdd.Parameters.AddWithValue("@SoLuongToiDaCuTri",kybaucu.SoLuongToiDaCuTri);
                commandAdd.Parameters.AddWithValue("@SoLuongToiDaUngCuVien",kybaucu.SoLuongToiDaUngCuVien);
                commandAdd.Parameters.AddWithValue("@SoLuotBinhChonToiDa",kybaucu.SoLuotBinhChonToiDa);

                await commandAdd.ExecuteNonQueryAsync();
            } 

            return true; 
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
                WHERE ngayBD = @ngayBD";
            
            using var command = new MySqlCommand(sqlupdate, connection);
            command.Parameters.AddWithValue("@ngayBD",ID);
        
            //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
            int rowAffected = await command.ExecuteNonQueryAsync();
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

                    if(await reader.ReadAsync())
                        return reader.GetDateTime(reader.GetOrdinal("NgayKT_UngCu")); 
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
            SELECT COUNT(ct.ID_CuTri) SoLuongHienTai
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
        public async Task<TimeOfTheElectionDTO> _GetTimeOfElection(DateTime ngayBD, MySqlConnection connection){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
            
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
            }

            return null;// Không tồn tại
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
    
    }
}