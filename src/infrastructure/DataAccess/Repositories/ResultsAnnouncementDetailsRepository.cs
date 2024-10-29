using MySql.Data.MySqlClient;
using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.web_api.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class ResultsAnnouncementDetailsRepository : IDisposable, IResultsAnnouncementDetailsRepository
    {
        private readonly DatabaseContext _context;

        //Khởi tạo
        public ResultsAnnouncementDetailsRepository(DatabaseContext context) => _context = context;
        
        //hủy
        public void Dispose() => _context.Dispose();

        //1.Thêm thông tin vào bảng chi tiết bỏ phiếu
        public async Task<bool> _CadrePublicizeResult(string ID_CanBo, string ID_ucv, string ngayBD, MySqlConnection connect){
            try{
                //Kiểm tra trạng thái kết nối trước khi mở
                if(connect.State != System.Data.ConnectionState.Open)
                    await connect.OpenAsync();

                const string sql = @"
                INSERT INTO chitietcongboketqua(ID_CanBo,ID_ucv,ngayBD,ThoiDiemCongBo) 
                VALUES(@ID_CanBo,@ID_ucv,@ngayBD,@ThoiDiemCongBo);";
                
                using(var command = new MySqlCommand(sql, connect)){
                    command.Parameters.AddWithValue("@ID_CanBo", ID_CanBo);
                    command.Parameters.AddWithValue("@ID_ucv", ID_ucv);
                    command.Parameters.AddWithValue("@ngayBD", ngayBD);
                    command.Parameters.AddWithValue("@ThoiDiemCongBo", DateTime.Now);
                    
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
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

    }
}