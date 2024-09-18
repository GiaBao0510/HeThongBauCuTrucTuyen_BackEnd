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
        public ElectionsReposistory(DatabaseContext context) => _context = context;

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
                INSERT INTO kybaucu(ngayBD,ngayKT,TenKyBauCu,MoTa,SoLuongToiDaCuTri,SoLuongToiDaUngCuVien,SoLuotBinhChonToiDa) 
                VALUES(@ngayBD,@ngayKT,@TenKyBauCu,@MoTa,@SoLuongToiDaCuTri,@SoLuongToiDaUngCuVien,@SoLuotBinhChonToiDa);";
            
            using (var commandAdd = new MySqlCommand(Input, connection)){
                commandAdd.Parameters.AddWithValue("@ngayBD",kybaucu.ngayBD);
                commandAdd.Parameters.AddWithValue("@ngayKT",kybaucu.ngayKT);
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
            TenKyBauCu = @TenKyBauCu, 
            ngayKT = @ngayKT, 
            MoTa = @MoTa,
            SoLuongToiDaCuTri = @SoLuongToiDaCuTri,
            SoLuongToiDaUngCuVien = @SoLuongToiDaUngCuVien,
            SoLuotBinhChonToiDa = @SoLuotBinhChonToiDa
            WHERE ngayBD = @ngayBDCu;";

            using( var command = new MySqlCommand(sqlupdate, connection)){
                command.Parameters.AddWithValue("@ngayBDMoi",Elections.ngayBD);
                command.Parameters.AddWithValue("@TenKyBauCu",Elections.TenKyBauCu);
                command.Parameters.AddWithValue("@ngayKT",Elections.ngayKT);
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
            
            const string sql = "SELECT COUNT(ngayBD) FROM kybaucu WHERE ngayBD=@ngayBD;";
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ngayBD",ngayBD);
                
                int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                return count > 0;
            }
        }
    }
}