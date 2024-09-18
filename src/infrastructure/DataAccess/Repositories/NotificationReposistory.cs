using BackEnd.core.Entities;
using BackEnd.src.infrastructure.DataAccess.Context;
using MySql.Data.MySqlClient;
using BackEnd.src.web_api.DTOs;
using BackEnd.src.infrastructure.DataAccess.IRepository;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class NotificationsReposistory : IDisposable,INotificationRepository
    {
        private readonly DatabaseContext _context;

        //Khởi tạo
        public NotificationsReposistory(DatabaseContext context) => _context = context;

        //Hủy
        public void Dispose() => _context.Dispose();

        //Liệt kê
        public async Task<List<Notifications>> _GetListOfNotifications(){
            var list = new List<Notifications>();

            using var connection = await _context.Get_MySqlConnection();
            using var command = new MySqlCommand("SELECT * FROM thongbao", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new Notifications{
                    ID_ThongBao = reader.GetInt32(reader.GetOrdinal("ID_ThongBao")),
                    NoiDungThongBao = reader.GetString(reader.GetOrdinal("NoiDungThongBao")),
                    ThoiDiem = reader.GetDateTime(reader.GetOrdinal("ThoiDiem"))
                });
            }
            return list;
        }

        //Thêm
        public async Task<bool> _AddNotifications(Notifications thongbao){
            using var connection = await _context.Get_MySqlConnection();

            //Thực hiện thêm            
            string Input = @"
                INSERT INTO thongbao(NoiDungThongBao,ThoiDiem) 
                VALUES(@NoiDungThongBao,@ThoiDiem);";
            
            using (var commandAdd = new MySqlCommand(Input, connection)){
                commandAdd.Parameters.AddWithValue("@NoiDungThongBao",thongbao.NoiDungThongBao);
                commandAdd.Parameters.AddWithValue("@ThoiDiem",thongbao.ThoiDiem);
                await commandAdd.ExecuteNonQueryAsync();
            } 

            return true; 
        }

        //Lấy theo ID
        public async Task<Notifications> _GetNotificationsBy_ID(string id){
            using var connection = await _context.Get_MySqlConnection();

            const string sql = @"
                SELECT * FROM thongbao 
                WHERE ID_ThongBao = @ID_ThongBao";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID_ThongBao",id);

            using var reader = await command.ExecuteReaderAsync();
            if(await reader.ReadAsync()){
                return new Notifications{
                    ID_ThongBao = reader.GetInt32(reader.GetOrdinal("ID_ThongBao")),
                    NoiDungThongBao = reader.GetString(reader.GetOrdinal("NoiDungThongBao")),
                    ThoiDiem = reader.GetDateTime(reader.GetOrdinal("ThoiDiem"))
                };
            }

            return null;
        }

        //Sửa
        public async Task<bool> _EditNotificationsBy_ID(string ID, Notifications Notifications){
            using var connection = await _context.Get_MySqlConnection();

            //Cập nhật
            const string sqlupdate = @"UPDATE thongbao SET NoiDungThongBao = @NoiDungThongBao, ThoiDiem = @ThoiDiem WHERE ID_ThongBao = @ID_ThongBao";
            using( var command = new MySqlCommand(sqlupdate, connection)){
                command.Parameters.AddWithValue("@ID_ThongBao",ID);
                command.Parameters.AddWithValue("@NoiDungThongBao",Notifications.NoiDungThongBao);
                command.Parameters.AddWithValue("@ThoiDiem",Notifications.ThoiDiem);

                //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
                int rowAffected = await command.ExecuteNonQueryAsync();
                return rowAffected > 0;
            }
            
        }

        //Xóa
        public async Task<bool> _DeleteNotificationsBy_ID(string ID){
            using var connection = await _context.Get_MySqlConnection();

            const string sqlupdate = @"
                DELETE FROM thongbao
                WHERE ID_ThongBao = @ID_ThongBao";
            
            using var command = new MySqlCommand(sqlupdate, connection);
            command.Parameters.AddWithValue("@ID_ThongBao",ID);
        
            //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
            int rowAffected = await command.ExecuteNonQueryAsync();
            return rowAffected > 0;
        }

        //Lấy danh sách thông báo theo ứng cử viên
        //Lấy danh sách thông báo theo cử tri
        //lấy danh sách thông báo theo cán bộ
    }
}