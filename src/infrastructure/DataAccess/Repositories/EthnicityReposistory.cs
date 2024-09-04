using BackEnd.src.infrastructure.DataAccess.Context;
using MySql.Data.MySqlClient;
using BackEnd.src.web_api.DTOs;
using BackEnd.src.core.Entities;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class EthnicityReposistory : IDisposable
    {
        private readonly DatabaseContext _context;

        //Khởi tạo
        public EthnicityReposistory(DatabaseContext context) => _context = context;

        //Hủy
        public void Dispose() => _context.Dispose();

        //Liệt kê
        public async Task<List<Ethnicity>> _GetListOfEthnicity(){
            var list = new List<Ethnicity>();

            using var connection = await _context.Get_MySqlConnection();
            using var command = new MySqlCommand("SELECT * FROM dantoc", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new Ethnicity{
                    ID_DanToc = reader.GetInt32(reader.GetOrdinal("ID_DanToc")),
                    TenDanToc = reader.GetString(reader.GetOrdinal("TenDanToc")),
                    TenGoiKhac = reader.IsDBNull(reader.GetOrdinal("TenGoiKhac")) ? null : reader.GetString(reader.GetOrdinal("TenGoiKhac"))
                });
            }
            return list;
        }

        //Thêm
        public async Task<bool> _AddEthnicity(Ethnicity dantoc){
            using var connection = await _context.Get_MySqlConnection();

            //Thực hiện thêm            
            string Input = @"
                INSERT INTO dantoc(TenDanToc,TenGoiKhac) 
                VALUES(@TenDanToc,@TenGoiKhac);";
            
            using (var commandAdd = new MySqlCommand(Input, connection)){
                commandAdd.Parameters.AddWithValue("@TenDanToc",dantoc.TenDanToc);
                commandAdd.Parameters.AddWithValue("@TenGoiKhac",dantoc.TenGoiKhac);
                await commandAdd.ExecuteNonQueryAsync();
            } 

            return true; 
        }

        //Lấy theo ID
        public async Task<Ethnicity> _GetEthnicityBy_ID(string id){
            using var connection = await _context.Get_MySqlConnection();

            const string sql = @"
                SELECT * FROM dantoc 
                WHERE ID_DanToc = @ID_DanToc";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID_DanToc",id);

            using var reader = await command.ExecuteReaderAsync();
            if(await reader.ReadAsync()){
                return new Ethnicity{
                    ID_DanToc = reader.GetInt32(reader.GetOrdinal("ID_DanToc")),
                    TenDanToc = reader.GetString(reader.GetOrdinal("TenDanToc")),
                    TenGoiKhac = reader.IsDBNull(reader.GetOrdinal("TenGoiKhac")) ? null : reader.GetString(reader.GetOrdinal("TenGoiKhac"))
                };
            }

            return null;
        }

        //Sửa
        public async Task<bool> _EditEthnicityBy_ID(string ID, Ethnicity Ethnicity){
            using var connection = await _context.Get_MySqlConnection();

            //Cập nhật
            const string sqlupdate = @"UPDATE dantoc SET TenDanToc = @TenDanToc,TenGoiKhac=@TenGoiKhac  WHERE ID_DanToc = @ID_DanToc";
            using( var command = new MySqlCommand(sqlupdate, connection)){
                command.Parameters.AddWithValue("@ID_DanToc",ID);
                command.Parameters.AddWithValue("@TenDanToc",Ethnicity.TenDanToc);
                command.Parameters.AddWithValue("@TenGoiKhac",Ethnicity.TenGoiKhac);

                //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
                int rowAffected = await command.ExecuteNonQueryAsync();
                return rowAffected > 0;
            }
            
        }

        //Xóa
        public async Task<bool> _DeleteEthnicityBy_ID(string ID){
            using var connection = await _context.Get_MySqlConnection();

            const string sqlupdate = @"
                DELETE FROM dantoc
                WHERE ID_DanToc = @ID_DanToc";
            
            using var command = new MySqlCommand(sqlupdate, connection);
            command.Parameters.AddWithValue("@ID_DanToc",ID);
        
            //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
            int rowAffected = await command.ExecuteNonQueryAsync();
            return rowAffected > 0;
        }
    }
}