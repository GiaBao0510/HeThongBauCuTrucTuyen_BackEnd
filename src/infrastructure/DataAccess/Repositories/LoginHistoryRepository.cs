using BackEnd.core.Entities;
using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class LoginHistoryRepository : IDisposable, ILoginHistoryRepository 
    {
        private readonly DatabaseContext _context;
        public LoginHistoryRepository(DatabaseContext context) => _context = context;
        public void Dispose()=> _context.Dispose();

        //1. Lưu lịch sử đăng nhập của người dùng
        public async Task<bool> _SaveLoginHistory(string DiaChiIP, string taikhoan){
            
            try{
                using (var connect = _context.CreateConnection()){
                    if(connect.State == System.Data.ConnectionState.Closed){    //Nếu kết nối đóng thì mở 
                        await connect.OpenAsync();
                    }
                    const string sql = @"
                    INSERT INTO 
                    lichsudangnhap(ThoiDiem,DiaChiIP,taikhoan) 
                    VALUES(@ThoiDiem,@DiaChiIP,@taikhoan);";

                    using (var command = new MySqlCommand(sql, connect))
                    {
                        command.Parameters.AddWithValue("@ThoiDiem", DateTime.Now.ToLocalTime());
                        command.Parameters.AddWithValue("@DiaChiIP", DiaChiIP);
                        command.Parameters.AddWithValue("@taikhoan", taikhoan);

                        int rowAffect = await command.ExecuteNonQueryAsync();
                        return rowAffect > 0;
                    }
                }
            }catch(Exception ex){
                throw new Exception(ex.Message);
            }
        }

        //2. Lấy danh sách lịch sử đăng nhập
        public async Task<List<LoginHistory>> _GetLoginHistoryList(){
            var list = new List<LoginHistory>();

            using var connection = await _context.Get_MySqlConnection();
            const string sql = "SELECT * FROM lichsudangnhap;";
            using (var command = new MySqlCommand(sql, connection)){
                using var reader = await command.ExecuteReaderAsync();
                while(await reader.ReadAsync()){
                    list.Add(new LoginHistory{
                        ThoiDiem = reader.GetDateTime(reader.GetOrdinal("ThoiDiem")),
                        DiaChiIP = reader.GetString(reader.GetOrdinal("DiaChiIP")),
                        TaiKhoan = reader.GetString(reader.GetOrdinal("TaiKhoan")),
                    });
                }
            }

            return list;
        }
        //3. Lấy danh sách lịch sử đăng nhập của cử tri
        public async Task<List<LoginHistory>> _GetListOfLoginHistory_byVote(){
            var list = new List<LoginHistory>();

            using var connection = await _context.Get_MySqlConnection();
            const string sql = @"
            SELECT ls.ThoiDiem, ls.DiaChiIP ,ls.TaiKhoan 
            FROM lichsudangnhap ls 
            JOIN nguoidung nd ON nd.SDT = ls.TaiKhoan
            JOIN cutri ct ON ct.ID_user = nd.ID_user;";
            using (var command = new MySqlCommand(sql, connection)){
                using var reader = await command.ExecuteReaderAsync();
                while(await reader.ReadAsync()){
                    list.Add(new LoginHistory{
                        ThoiDiem = reader.GetDateTime(reader.GetOrdinal("ThoiDiem")),
                        DiaChiIP = reader.GetString(reader.GetOrdinal("DiaChiIP")),
                        TaiKhoan = reader.GetString(reader.GetOrdinal("TaiKhoan")),
                    });
                }
            }
            
            return list;
        }
        //4. Lấy danh sách lịch sử đăng nhập của cán bộ
        public async Task<List<LoginHistory>> _GetListOfLoginHistory_byCadre(){
             var list = new List<LoginHistory>();

            using var connection = await _context.Get_MySqlConnection();
            const string sql = @"
            SELECT ls.ThoiDiem, ls.DiaChiIP ,ls.TaiKhoan 
            FROM lichsudangnhap ls 
            JOIN nguoidung nd ON nd.SDT = ls.TaiKhoan
            JOIN canbo cb ON cb.ID_user = nd.ID_user;";
            using (var command = new MySqlCommand(sql, connection)){
                using var reader = await command.ExecuteReaderAsync();
                while(await reader.ReadAsync()){
                    list.Add(new LoginHistory{
                        ThoiDiem = reader.GetDateTime(reader.GetOrdinal("ThoiDiem")),
                        DiaChiIP = reader.GetString(reader.GetOrdinal("DiaChiIP")),
                        TaiKhoan = reader.GetString(reader.GetOrdinal("TaiKhoan")),
                    });
                }
            }
            
            return list;
        }
        //5. Lấy danh sách lịch sử đăng nhập của ứng cử viên
        public async Task<List<LoginHistory>> _GetListOfLoginHistory_byCandidate(){
             var list = new List<LoginHistory>();

            using var connection = await _context.Get_MySqlConnection();
            const string sql = @"
            SELECT ls.ThoiDiem, ls.DiaChiIP ,ls.TaiKhoan 
            FROM lichsudangnhap ls 
            JOIN nguoidung nd ON nd.SDT = ls.TaiKhoan
            JOIN ungcuvien ucv ON ucv.ID_user = nd.ID_user;";
            using (var command = new MySqlCommand(sql, connection)){
                using var reader = await command.ExecuteReaderAsync();
                while(await reader.ReadAsync()){
                    list.Add(new LoginHistory{
                        ThoiDiem = reader.GetDateTime(reader.GetOrdinal("ThoiDiem")),
                        DiaChiIP = reader.GetString(reader.GetOrdinal("DiaChiIP")),
                        TaiKhoan = reader.GetString(reader.GetOrdinal("TaiKhoan")),
                    });
                }
            }
            
            return list;
        }
    }
}