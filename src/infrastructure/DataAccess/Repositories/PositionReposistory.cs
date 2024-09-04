using BackEnd.core.Entities;
using BackEnd.src.core.Entities;
using BackEnd.src.infrastructure.DataAccess.Context;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class PositionReposistory : IDisposable
    {
        private readonly DatabaseContext _context;

        //Khởi tạo
        public PositionReposistory(DatabaseContext context) => _context = context;

        //Hủy
        public void Dispose() => _context.Dispose();

        //Liệt kê
        public async Task<List<Position>> _GetListOfPosition(){
            var list = new List<Position>();

            using var connection = await _context.Get_MySqlConnection();
            using var command = new MySqlCommand("SELECT * FROM chucvu", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new Position{
                    ID_ChucVu = reader.GetInt32(reader.GetOrdinal("ID_ChucVu")),
                    TenChucVu = reader.GetString(reader.GetOrdinal("TenChucVu"))
                });
            }
            return list;
        }

        //Thêm
        public async Task<bool> _AddPosition(Position chucvu){
            using var connection = await _context.Get_MySqlConnection();

            //Thực hiện thêm            
            string Input = @"
                INSERT INTO chucvu(TenChucVu) 
                VALUES(@TenChucVu);";
            
            using (var commandAdd = new MySqlCommand(Input, connection)){
                commandAdd.Parameters.AddWithValue("@TenChucVu",chucvu.TenChucVu);
                await commandAdd.ExecuteNonQueryAsync();
            } 

            return true; 
        }

        //Lấy theo ID
        public async Task<Position> _GetPositionBy_ID(string id){
            using var connection = await _context.Get_MySqlConnection();

            const string sql = @"
                SELECT * FROM chucvu 
                WHERE ID_ChucVu = @ID_ChucVu";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID_ChucVu",id);

            using var reader = await command.ExecuteReaderAsync();
            if(await reader.ReadAsync()){
                return new Position{
                    ID_ChucVu = reader.GetInt32(reader.GetOrdinal("ID_ChucVu")),
                    TenChucVu = reader.GetString(reader.GetOrdinal("TenChucVu"))
                };
            }

            return null;
        }

        //Sửa
        public async Task<bool> _EditPositionBy_ID(string ID, Position Position){
            using var connection = await _context.Get_MySqlConnection();

            //Cập nhật
            const string sqlupdate = @"UPDATE chucvu SET TenChucVu = @TenChucVu WHERE ID_ChucVu = @ID_ChucVu";
            using( var command = new MySqlCommand(sqlupdate, connection)){
                command.Parameters.AddWithValue("@ID_ChucVu",ID);
                command.Parameters.AddWithValue("@TenChucVu",Position.TenChucVu);

                //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
                int rowAffected = await command.ExecuteNonQueryAsync();
                return rowAffected > 0;
            }
            
        }

        //Xóa
        public async Task<bool> _DeletePositionBy_ID(string ID){
            using var connection = await _context.Get_MySqlConnection();

            const string sqlupdate = @"
                DELETE FROM chucvu
                WHERE ID_ChucVu = @ID_ChucVu";
            
            using var command = new MySqlCommand(sqlupdate, connection);
            command.Parameters.AddWithValue("@ID_ChucVu",ID);
        
            //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
            int rowAffected = await command.ExecuteNonQueryAsync();
            return rowAffected > 0;
        }
    }
}