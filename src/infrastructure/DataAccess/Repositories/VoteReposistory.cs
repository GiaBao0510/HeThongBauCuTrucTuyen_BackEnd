using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;
using BackEnd.src.core.Common;
using BackEnd.src.infrastructure.DataAccess.IRepository;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class VoteReposistory : IDisposable, IVoteRepository
    {
        private readonly DatabaseContext _context;

        //Khởi tạo
        public VoteReposistory(DatabaseContext context) => _context = context;

        //hủy
        public void Dispose() => _context.Dispose();

        //Liệt kê
        public async Task<List<VoteDto>> _GetListOfVote(){
            var list = new List<VoteDto>();

            using var connection = await _context.Get_MySqlConnection();
            using var command = new MySqlCommand("SELECT * FROM phieubau", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new VoteDto{
                    ID_Phieu = reader.GetString(reader.GetOrdinal("ID_Phieu")),
                    GiaTriPhieuBau = reader.GetInt32(reader.GetOrdinal("GiaTriPhieuBau")),
                    ngayBD = reader.GetDateTime(reader.GetOrdinal("ngayBD")).ToString("dd-MM-yyyy ss:mm:HH")
                });
            }
            return list;
        }

        //thêm
        public async Task<bool> _AddVote(VoteDto phieubau, MySqlConnection connection){
            
            //Kiểm tra mã số đơn vị bầu cử tại phieubau có tồn tại không
            string checkInput = "SELECT COUNT(*) FROM kybaucu WHERE ngayBD = @ngayBD";
            using(var commandCheck = new MySqlCommand(checkInput, connection)){
                commandCheck.Parameters.AddWithValue("@ngayBD",phieubau.ngayBD);
                int count = Convert.ToInt32(await commandCheck.ExecuteScalarAsync());
                if(count < 1)
                    return false;
            }
            //Lấy 2 ký tự ngẫu nhiên
            string randomString = RandomString.ChuoiNgauNhien(2);
            DateTime currentDay = DateTime.Now;
            string ID_Phieu = randomString+$"{currentDay:yyyyMMddHHmmssff}";

            //Thực hiện thêm            
            string Input = $"INSERT INTO phieubau(ID_Phieu,GiaTriPhieuBau,ngayBD) VALUES(@ID_Phieu,@GiaTriPhieuBau,@ngayBD);";
            using (var commandAdd = new MySqlCommand(Input, connection)){
                commandAdd.Parameters.AddWithValue("@ID_Phieu",ID_Phieu);
                commandAdd.Parameters.AddWithValue("@GiaTriPhieuBau",phieubau.GiaTriPhieuBau);
                commandAdd.Parameters.AddWithValue("@ngayBD",phieubau.ngayBD);
                await commandAdd.ExecuteNonQueryAsync();
            } 

            return true; 
        }

        //Lấy theo ID
        public async Task<VoteDto> _GetVoteBy_ID(string id){
            using var connection = await _context.Get_MySqlConnection();

            const string sql = @"
                SELECT * FROM phieubau 
                WHERE ID_Phieu = @ID_Phieu";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID_Phieu",id);

            using var reader = await command.ExecuteReaderAsync();
            if(await reader.ReadAsync()){
                return new VoteDto{
                    ID_Phieu = reader.GetString(reader.GetOrdinal("ID_Phieu")),
                    GiaTriPhieuBau = reader.GetInt32(reader.GetOrdinal("GiaTriPhieuBau")),
                    ngayBD = reader.GetDateTime(reader.GetOrdinal("ngayBD")).ToString("dd-MM-yyyy ss:mm:HH")
                };
            }

            return null;
        }

        //Lấy theo Time
        public async Task<List<VoteDto>> _GetVoteBy_Time(string time){
            var list = new List<VoteDto>();
            using var connection = await _context.Get_MySqlConnection();
            using var command = new MySqlCommand("SELECT * FROM phieubau WHERE ngayBD=@ngayBD", connection);
            command.Parameters.AddWithValue("@ngayBD",time);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new VoteDto{
                    ID_Phieu = reader.GetString(reader.GetOrdinal("ID_Phieu")),
                    GiaTriPhieuBau = reader.GetInt32(reader.GetOrdinal("GiaTriPhieuBau")),
                    ngayBD = reader.GetDateTime(reader.GetOrdinal("ngayBD")).ToString("dd-MM-yyyy ss:mm:HH")
                });
            }
            return list;
        }

        //Sửa
        public async Task<bool> _EditVoteBy_ID(string ID, VoteDto Vote){
            using var connection = await _context.Get_MySqlConnection();

            //Tìm kiếm quận huyện có tồn tại không
            const string sqlCheck = "SELECT COUNT(*) FROM kybaucu WHERE ngayBD = @ngayBD";
            using(var command0 = new MySqlCommand(sqlCheck, connection)){
                command0.Parameters.AddWithValue("@ngayBD",Vote.ngayBD);
                int count = Convert.ToInt32(await command0.ExecuteScalarAsync());
                
                if(count < 1)
                    return false;
            }

            //Cập nhật
            const string sqlupdate = @"UPDATE phieubau 
            SET GiaTriPhieuBau = @GiaTriPhieuBau, ngayBD =@ngayBD 
            WHERE ID_Phieu = @ID_Phieu";
            using( var command = new MySqlCommand(sqlupdate, connection)){
                command.Parameters.AddWithValue("@GiaTriPhieuBau",Vote.GiaTriPhieuBau);
                command.Parameters.AddWithValue("@ngayBD",Vote.ngayBD);
                command.Parameters.AddWithValue("@ID_Phieu",ID);
                //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
                int rowAffected = await command.ExecuteNonQueryAsync();
                return rowAffected > 0;
            }
            
        }

        //Xóa phiếu theo ID
        public async Task<bool> _DeleteVoteBy_ID(string ID){
            using var connection = await _context.Get_MySqlConnection();

            const string sqlupdate = @"
                DELETE FROM phieubau
                WHERE ID_Phieu = @ID_Phieu";
            
            using var command = new MySqlCommand(sqlupdate, connection);
            command.Parameters.AddWithValue("@ID_Phieu",ID);
        
            //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
            int rowAffected = await command.ExecuteNonQueryAsync();
            return rowAffected > 0;
        }

        //Xóa phiếu theo kỳ bầu cử
        public async Task<bool> _DeleteVoteBy_Time(string time){
            using var connection = await _context.Get_MySqlConnection();

            const string sqlupdate = @"
                DELETE FROM phieubau
                WHERE ngayBD = @ngayBD";
            
            using var command = new MySqlCommand(sqlupdate, connection);
            command.Parameters.AddWithValue("@ngayBD",time);
        
            //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
            int rowAffected = await command.ExecuteNonQueryAsync();
            return rowAffected > 0;
        }

        //Tạo số lượng phiếu bầu theo kỳ nào đó
        public async Task<bool> _CreateVoteByNumber(int number, VoteDto vote ){
            try{
                using var connection = await _context.Get_MySqlConnection();
                for(int i = 0 ; i< number ;i++){
                    var result =await _AddVote(vote,connection);
                    if(!result)
                        return false;
                }
            return true;
            }catch(Exception ex){
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Error Source: {ex.Source}");
                Console.WriteLine($"Error HResult: {ex.HResult}");
                Console.WriteLine($"Error InnerException: {ex.InnerException}");
                return false;
            }
        }

    } 
}