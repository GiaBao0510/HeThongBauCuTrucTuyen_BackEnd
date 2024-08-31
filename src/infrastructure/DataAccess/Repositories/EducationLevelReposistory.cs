using BackEnd.core.Entities;
using BackEnd.src.infrastructure.DataAccess.Context;
using MySql.Data.MySqlClient;
using BackEnd.src.web_api.DTOs;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class EducationLevelReposistory : IDisposable
    {
         private readonly DatabaseContext _context;

        //Khởi tạo
        public EducationLevelReposistory(DatabaseContext context) => _context = context;

        //Hủy
        public void Dispose() => _context.Dispose();

        //Liệt kê
        public async Task<List<EducationLevel>> _GetListOfEducationLevel(){
            var list = new List<EducationLevel>();

            using var connection = await _context.Get_MySqlConnection();
            using var command = new MySqlCommand("SELECT * FROM trinhdohocvan", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new EducationLevel{
                    ID_TrinhDo = reader.GetInt32(reader.GetOrdinal("ID_TrinhDo")),
                    TenTrinhDoHocVan = reader.GetString(reader.GetOrdinal("TenTrinhDoHocVan"))
                });
            }
            return list;
        }

        //Thêm
        public async Task<bool> _AddEducationLevel(EducationLevel trinhdohocvan){
            using var connection = await _context.Get_MySqlConnection();

            //Thực hiện thêm            
            string Input = @"
                INSERT INTO trinhdohocvan(TenTrinhDoHocVan) 
                VALUES(@TenTrinhDoHocVan);";
            
            using (var commandAdd = new MySqlCommand(Input, connection)){
                commandAdd.Parameters.AddWithValue("@TenTrinhDoHocVan",trinhdohocvan.TenTrinhDoHocVan);
                await commandAdd.ExecuteNonQueryAsync();
            } 

            return true; 
        }

        //Lấy theo ID
        public async Task<EducationLevel> _GetEducationLevelBy_ID(string id){
            using var connection = await _context.Get_MySqlConnection();

            const string sql = @"
                SELECT * FROM trinhdohocvan 
                WHERE ID_TrinhDo = @ID_TrinhDo";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID_TrinhDo",id);

            using var reader = await command.ExecuteReaderAsync();
            if(await reader.ReadAsync()){
                return new EducationLevel{
                    ID_TrinhDo = reader.GetInt32(reader.GetOrdinal("ID_TrinhDo")),
                    TenTrinhDoHocVan = reader.GetString(reader.GetOrdinal("TenTrinhDoHocVan"))
                };
            }

            return null;
        }

        //Sửa
        public async Task<bool> _EditEducationLevelBy_ID(string ID, EducationLevel EducationLevel){
            using var connection = await _context.Get_MySqlConnection();

            //Cập nhật
            const string sqlupdate = @"UPDATE trinhdohocvan SET TenTrinhDoHocVan = @TenTrinhDoHocVan WHERE ID_TrinhDo = @ID_TrinhDo";
            using( var command = new MySqlCommand(sqlupdate, connection)){
                command.Parameters.AddWithValue("@ID_TrinhDo",ID);
                command.Parameters.AddWithValue("@TenTrinhDoHocVan",EducationLevel.TenTrinhDoHocVan);

                //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
                int rowAffected = await command.ExecuteNonQueryAsync();
                return rowAffected > 0;
            }
            
        }

        //Xóa
        public async Task<bool> _DeleteEducationLevelBy_ID(string ID){
            using var connection = await _context.Get_MySqlConnection();

            const string sqlupdate = @"
                DELETE FROM trinhdohocvan
                WHERE ID_TrinhDo = @ID_TrinhDo";
            
            using var command = new MySqlCommand(sqlupdate, connection);
            command.Parameters.AddWithValue("@ID_TrinhDo",ID);
        
            //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
            int rowAffected = await command.ExecuteNonQueryAsync();
            return rowAffected > 0;
        }

    }
}