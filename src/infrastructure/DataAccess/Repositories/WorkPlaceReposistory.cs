using BackEnd.core.Entities;
using BackEnd.src.core.Entities;
using BackEnd.src.infrastructure.DataAccess.Context;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class WorkPlaceReposistory : IDisposable
    {
        private readonly DatabaseContext _context;

        //Khởi tạo
        public WorkPlaceReposistory(DatabaseContext context) => _context = context;

        //Hủy
        public void Dispose() => _context.Dispose();

        //Liệt kê các ID
        public async Task<List<WorkPlace>> _GetListOfWorkPlace(){
            var list = new List<WorkPlace>();

            using var connection = await _context.Get_MySqlConnection();
            using var command = new MySqlCommand("SELECT * FROM CongTac", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new WorkPlace{
                    ID_Ban = reader.GetInt32(reader.GetOrdinal("ID_Ban")),
                    ID_CanBo = reader.GetString(reader.GetOrdinal("ID_CanBo")),
                    ID_ChucVu = reader.GetInt32(reader.GetOrdinal("ID_ChucVu"))
                });
            }
            return list;
        }

        //Liệt kê tất cả nhưng chi tiết hơn

        //Thêm
        public async Task<bool> _AddWorkPlace(WorkPlace CongTac){
            using var connection = await _context.Get_MySqlConnection();

            //Thực hiện thêm            
            string Input = @"
                INSERT INTO CongTac(TenCongTac) 
                VALUES(@TenCongTac);";
            
            using (var commandAdd = new MySqlCommand(Input, connection)){
                commandAdd.Parameters.AddWithValue("@TenCongTac",CongTac.ID_Ban);
                await commandAdd.ExecuteNonQueryAsync();
            } 

            return true; 
        }

        //Lấy theo ID
        public async Task<WorkPlace> _GetWorkPlaceBy_ID(string id){
            using var connection = await _context.Get_MySqlConnection();

            const string sql = @"
                SELECT * FROM CongTac 
                WHERE ID_TrinhDo = @ID_TrinhDo";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID_TrinhDo",id);

            using var reader = await command.ExecuteReaderAsync();
            if(await reader.ReadAsync()){
                return new WorkPlace{
                    ID_Ban = reader.GetInt32(reader.GetOrdinal("ID_Ban")),
                    ID_CanBo = reader.GetString(reader.GetOrdinal("ID_CanBo")),
                    ID_ChucVu = reader.GetInt32(reader.GetOrdinal("ID_ChucVu"))
                };
            }

            return null;
        }

        //Sửa
        public async Task<bool> _EditWorkPlaceBy_ID(string ID, WorkPlace WorkPlace){
            using var connection = await _context.Get_MySqlConnection();

            //Cập nhật
            const string sqlupdate = @"UPDATE CongTac SET TenCongTac = @TenCongTac WHERE ID_TrinhDo = @ID_TrinhDo";
            using( var command = new MySqlCommand(sqlupdate, connection)){
                command.Parameters.AddWithValue("@ID_TrinhDo",ID);
                command.Parameters.AddWithValue("@TenCongTac",WorkPlace.ID_CanBo);

                //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
                int rowAffected = await command.ExecuteNonQueryAsync();
                return rowAffected > 0;
            }
            
        }

        //Xóa
        public async Task<bool> _DeleteWorkPlaceBy_ID(string ID){
            using var connection = await _context.Get_MySqlConnection();

            const string sqlupdate = @"
                DELETE FROM CongTac
                WHERE ID_TrinhDo = @ID_TrinhDo";
            
            using var command = new MySqlCommand(sqlupdate, connection);
            command.Parameters.AddWithValue("@ID_TrinhDo",ID);
        
            //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
            int rowAffected = await command.ExecuteNonQueryAsync();
            return rowAffected > 0;
        }
    }
}