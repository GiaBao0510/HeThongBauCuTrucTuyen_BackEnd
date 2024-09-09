using BackEnd.core.Entities;
using BackEnd.src.infrastructure.DataAccess.Context;
using MySql.Data.MySqlClient;
using BackEnd.src.web_api.DTOs;
using BackEnd.src.infrastructure.DataAccess.IRepository;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class DistrictReposistory : IDisposable,IDistrictRepository
    {
        private readonly DatabaseContext _context;
        //Khởi tạo
        public DistrictReposistory(DatabaseContext context) => _context = context;

        //Liệt kê
        public async Task<List<District>> _GetListOfDistrict(){
            var list = new List<District>();

            using var connection = await _context.Get_MySqlConnection();
            using var command = new MySqlCommand("SELECT * FROM quanhuyen", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new District{
                    STT = reader.GetInt32(reader.GetOrdinal("STT")),
                    ID_QH = reader.GetInt32(reader.GetOrdinal("ID_QH")),
                    TenQH = reader.GetString(reader.GetOrdinal("TenQH"))
                });
            }
            return list;
        }

        //Thêm
        public async Task<bool> _AddDistrict(DistrictDto quanhuyen){
            using var connection = await _context.Get_MySqlConnection();

            //Kiểm tra số thứ tự có tồn tại trong bảng tỉnh thành không, nếu không thì không thêm được
            string checkInput = "SELECT COUNT(*) FROM tinhthanh WHERE STT = @STT";
            using(var commandCheck = new MySqlCommand(checkInput, connection)){
                commandCheck.Parameters.AddWithValue("@STT",quanhuyen.STT);
                int count = Convert.ToInt32(await commandCheck.ExecuteScalarAsync());
                if(count < 1)
                    return false;
            }

            //Thực hiện thêm            
            string Input = $"INSERT INTO quanhuyen(STT,TenQH) VALUES(@STT ,@TenQH);";
            using (var commandAdd = new MySqlCommand(Input, connection)){
                commandAdd.Parameters.AddWithValue("@STT",quanhuyen.STT);
                commandAdd.Parameters.AddWithValue("@TenQH",quanhuyen.TenQH);
                await commandAdd.ExecuteNonQueryAsync();
            } 

            return true; 
        }

        //Lấy theo ID
        public async Task<District> _GetDistrictBy_ID(string id){
            using var connection = await _context.Get_MySqlConnection();

            const string sql = @"
                SELECT * FROM quanhuyen
                WHERE ID_QH = @ID_QH";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID_QH",id);

            using var reader = await command.ExecuteReaderAsync();
            if(await reader.ReadAsync()){
                return new District{
                    ID_QH = reader.GetInt32(reader.GetOrdinal("ID_QH")),
                    STT = reader.GetInt32(reader.GetOrdinal("STT")),
                    TenQH = reader.GetString(reader.GetOrdinal("TenQH"))
                };
            }

            return null;
        }

        //Lấy thông tin quận huyện theo stt tỉnh
        public async Task<List<District>> _GetListOfDistrictBy_STT(string stt){
            using var connection = await _context.Get_MySqlConnection();
            
            //Kiểm tra xem STT có tồn tại trong bảng tỉnh thành không không
            using(var command0 = new MySqlCommand("SELECT COUNT(*) FROM tinhthanh WHERE STT = @STT", connection)){
                command0.Parameters.AddWithValue("@STT", stt);
                int count = Convert.ToInt32(await command0.ExecuteScalarAsync());
                if(count < 1) 
                    return null;
            }

            //Thực hiện lấy danh sách theo STT
            var list = new List<District>();
            using (var command = new MySqlCommand("SELECT * FROM quanhuyen WHERE STT = @STT", connection)){
                command.Parameters.AddWithValue("@STT",stt);
                using var reader = await command.ExecuteReaderAsync();
                
                while(await reader.ReadAsync()){
                    list.Add(new District{
                        STT = reader.GetInt32(reader.GetOrdinal("STT")),
                        ID_QH = reader.GetInt32(reader.GetOrdinal("ID_QH")),
                        TenQH = reader.GetString(reader.GetOrdinal("TenQH"))
                    });
                }
            }
            return list;
        }

        //Sửa
        public async Task<bool> _EditDistrictBy_ID(string ID, DistrictDto District){
            using var connection = await _context.Get_MySqlConnection();

            const string sqlupdate = @"
                UPDATE quanhuyen 
                SET TenQH = @TenQH,
                    STT = @STT
                WHERE ID_QH = @ID_QH";
            
            using var command = new MySqlCommand(sqlupdate, connection);
            command.Parameters.AddWithValue("@ID_QH",ID);
            command.Parameters.AddWithValue("@STT",District.STT);
            command.Parameters.AddWithValue("@TenQH",District.TenQH);

            //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
            int rowAffected = await command.ExecuteNonQueryAsync();
            return rowAffected > 0;
        }

        //Xóa
        public async Task<bool> _DeleteDistrictBy_ID(string ID){
            using var connection = await _context.Get_MySqlConnection();

            const string sqlupdate = @"
                DELETE FROM quanhuyen
                WHERE ID_QH = @ID_QH";
            
            using var command = new MySqlCommand(sqlupdate, connection);
            command.Parameters.AddWithValue("@ID_QH",ID);
        
            //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
            int rowAffected = await command.ExecuteNonQueryAsync();
            return rowAffected > 0;
        }
        
        public void Dispose() => _context.Dispose();

    }
}