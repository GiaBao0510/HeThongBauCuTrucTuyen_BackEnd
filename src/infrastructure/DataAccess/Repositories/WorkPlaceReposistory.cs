using BackEnd.src.core.Entities;
using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class WorkPlaceReposistory : IDisposable, IWorkPlaceRepository
    {
        private readonly DatabaseContext _context;

        //Khởi tạo
        public WorkPlaceReposistory(DatabaseContext context) => _context = context;

        //Hủy
        public void Dispose() => _context.Dispose();

        //0.1. Kiểm tra ID_ban của ban có tồn tại không
        public async Task<bool> _CheckIfBoardCodeExists(int ID_ban, MySqlConnection connection){
            
            const string sqlCount = @"
            SELECT COUNT(ID_Ban) 
            FROM ban WHERE ID_Ban = @ID_Ban";

            using (var command = new MySqlCommand(sqlCount, connection)){
                command.Parameters.AddWithValue("@ID_Ban", ID_ban);
                return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
            }
        }

        //0.2. Kiểm tra ID_chucvu của ban có tồn tại không
        public async Task<bool> _CheckIfPositionCodeExists(int ID_ChucVu, MySqlConnection connection){
            
            const string sqlCount = @"
            SELECT COUNT(ID_ChucVu) 
            FROM chucvu WHERE ID_ChucVu = @ID_ChucVu";

            using (var command = new MySqlCommand(sqlCount, connection)){
                command.Parameters.AddWithValue("@ID_ChucVu", ID_ChucVu);
                return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
            }
        }

        //0.3. Kiểm tra ID_can bo của ban có tồn tại không
        public async Task<bool> _CheckIfCadreCodeExists(string ID_canbo, MySqlConnection connection){
            
            const string sqlCount = @"
            SELECT COUNT(ID_canbo) 
            FROM canbo WHERE ID_canbo = @ID_canbo";

            using (var command = new MySqlCommand(sqlCount, connection)){
                command.Parameters.AddWithValue("@ID_canbo", ID_canbo);
                return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
            }
        }

        //0.4. Kiểm tra tổng quát trên các hàm kiểm tra trên
        public async Task<int> _CheckIfTheCodeExistsAtWork(WorkPlace workPlaceDto, MySqlConnection connection){
            if(!await _CheckIfPositionCodeExists(workPlaceDto.ID_ChucVu, connection)) return 0;
            if(!await _CheckIfCadreCodeExists(workPlaceDto.ID_canbo, connection)) return -1;
            if(!await _CheckIfBoardCodeExists(workPlaceDto.ID_Ban, connection)) return -2;
            return 1;
        }

        //1. Liệt kê các ID
        public async Task<List<WorkPlace>> _GetWorkplaces(){
            var list = new List<WorkPlace>();

            using var connection = await _context.Get_MySqlConnection();
            using var command = new MySqlCommand("SELECT * FROM HoatDong", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new WorkPlace{
                    ID_Ban = reader.GetInt32(reader.GetOrdinal("ID_Ban")),
                    ID_canbo = reader.GetString(reader.GetOrdinal("ID_canbo")),
                    ID_ChucVu = reader.GetInt32(reader.GetOrdinal("ID_ChucVu"))
                });
            }
            return list;
        }

        //2. Liệt kê các ID .Nhưng chi tiết hơn
        public async Task<List<WorkPlaceDto>> _GetWorkplacesDetail(){
            var list = new List<WorkPlaceDto>();

            using var connection = await _context.Get_MySqlConnection();
            const string sql = @"
            SELECT hd.ID_canbo, hd.ID_ChucVu, hd.ID_Ban, nd.HoTen, ban.TenBan, cv.TenChucVu
            FROM hoatdong hd INNER JOIN canbo cb ON cb.ID_canbo = hd.ID_canbo
            JOIN nguoidung nd ON nd.ID_user = cb.ID_user 
            JOIN chucvu cv ON cv.ID_ChucVu = hd.ID_ChucVu
            JOIN ban ON ban.ID_Ban = hd.ID_Ban";
            using var command = new MySqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new WorkPlaceDto{
                    ID_Ban = reader.GetInt32(reader.GetOrdinal("ID_Ban")),
                    ID_canbo = reader.GetString(reader.GetOrdinal("ID_canbo")),
                    ID_ChucVu = reader.GetInt32(reader.GetOrdinal("ID_ChucVu")),
                    HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                    TenBan = reader.GetString(reader.GetOrdinal("TenBan")),
                    TenChucVu = reader.GetString(reader.GetOrdinal("TenChucVu"))
                });
            }
            return list;
        }

        //3. Thêm
        public async Task<int> _AddDataToTheWorkplace(WorkPlace CongTac){
            using var connection = await _context.Get_MySqlConnection();

            //Kiểm tra điều kiện trước khi thêm
            var result = await _CheckIfTheCodeExistsAtWork(CongTac, connection);
            if(result <= 0) return result;  //Nếu ID_Ban, ID_canbo, ID_ChucVu không tồn tại thì trả về false

            //Thực hiện thêm            
            string Input = @"
            INSERT INTO hoatdong(ID_canbo,ID_ChucVu,ID_Ban) 
            VALUES (@ID_canbo,@ID_ChucVu,@ID_Ban);";
            
            using (var commandAdd = new MySqlCommand(Input, connection)){
                commandAdd.Parameters.AddWithValue("@ID_canbo",CongTac.ID_canbo);
                commandAdd.Parameters.AddWithValue("@ID_ChucVu",CongTac.ID_ChucVu);
                commandAdd.Parameters.AddWithValue("@ID_Ban",CongTac.ID_Ban);

                await commandAdd.ExecuteNonQueryAsync();
            } 

            return 1; 
        }

        //4. Cập nhật dữ liệu nơi công tác- mã cán bộ
        public async Task<bool> _UpdateWorkplaceBy_IDcadre(WorkPlaceDto workPlaceDto){
            using var connection = await _context.Get_MySqlConnection();

            //Cập nhật
            const string sqlupdate = @"UPDATE hoatdong SET ID_Ban=@ID_Ban WHERE ID_canbo=@ID_canbo;";
            using( var command = new MySqlCommand(sqlupdate, connection)){
                command.Parameters.AddWithValue("@ID_canbo",workPlaceDto.ID_canbo);
                command.Parameters.AddWithValue("@ID_Ban",workPlaceDto.ID_Ban);

                //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
                int rowAffected = await command.ExecuteNonQueryAsync();
                return rowAffected > 0;
            }
        }

        //5.Cập nhật dữ liệu nơi công tác theo mã chức vụ
        public async Task<bool> _UpdateWorkplaceBy_IDposition(WorkPlaceDto workPlaceDto){
            using var connection = await _context.Get_MySqlConnection();

            //Cập nhật
            const string sqlupdate = @"UPDATE hoatdong SET ID_Ban=@ID_Ban WHERE ID_ChucVu=@ID_ChucVu;";
            using( var command = new MySqlCommand(sqlupdate, connection)){
                command.Parameters.AddWithValue("@ID_ChucVu",workPlaceDto.ID_ChucVu);
                command.Parameters.AddWithValue("@ID_Ban",workPlaceDto.ID_Ban);

                //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
                int rowAffected = await command.ExecuteNonQueryAsync();
                return rowAffected > 0;
            }
        }
        
        //6.Cập nhật dữ liệu chức vụ theo mã ban
         public async Task<bool> _UpdateWorkplaceBy_IDboard(WorkPlaceDto workPlaceDto){
            using var connection = await _context.Get_MySqlConnection();

            //Cập nhật
            const string sqlupdate = @"UPDATE hoatdong SET ID_ChucVu=@ID_ChucVu WHERE ID_Ban=@ID_Ban;";
            using( var command = new MySqlCommand(sqlupdate, connection)){
                command.Parameters.AddWithValue("@ID_Ban",workPlaceDto.ID_Ban);
                command.Parameters.AddWithValue("@ID_ChucVu",workPlaceDto.ID_ChucVu);

                //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
                int rowAffected = await command.ExecuteNonQueryAsync();
                return rowAffected > 0;
            }
        }

        //7. Xóa dữ liệu nơi công tác theo mã cán bộ
        public async Task<bool> _DeleteWorkplaceBy_IDcadre(string ID_canbo){
            using var connection = await _context.Get_MySqlConnection();
            const string sql = "DELETE FROM hoatdong WHERE ID_canbo=@ID_canbo;";

            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_canbo", ID_canbo);

                int rowAffect = await command.ExecuteNonQueryAsync();
                return rowAffect > 0;
            }
        }

        //8. Xóa dữ liệu nơi công tác theo mã chức vụ
        public async Task<bool> _DeleteWorkplaceBy_IDposition(int ID_ChucVu){
            using var connection = await _context.Get_MySqlConnection();
            const string sql = "DELETE FROM hoatdong WHERE ID_ChucVu=@ID_ChucVu;";

            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_ChucVu", ID_ChucVu);

                int rowAffect = await command.ExecuteNonQueryAsync();
                return rowAffect > 0;
            }
        }

        //9. Xóa dữ liệu nơi công tác theo mã ban
        public async Task<bool> _DeleteWorkplaceBy_IDboard(int ID_Ban){
            using var connection = await _context.Get_MySqlConnection();
            const string sql = "DELETE FROM hoatdong WHERE ID_Ban=@ID_Ban;";

            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_Ban", ID_Ban);

                int rowAffect = await command.ExecuteNonQueryAsync();
                return rowAffect > 0;
            }
        }

        //10.Đặt lại chức vụ cho cán bộ - ID cán bộ
        public async Task<bool> _UpdatePositionBy_IDcadre(string ID_canbo, int ID_chucvu){
            using var connection = await _context.Get_MySqlConnection();
            const string sql = @"UPDATE hoatdong SET ID_ChucVu=@ID_ChucVu WHERE ID_canbo=@ID_canbo;";

            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_canbo", ID_canbo);
                command.Parameters.AddWithValue("@ID_ChucVu", ID_chucvu);
                
                int count = Convert.ToInt32(await command.ExecuteNonQueryAsync());
                if(count < 0) return false;
            }
            return true; 
        }

        //11.Đặt lại ban mà cán bộ đã làm việc theo -ID cán bộ
        public async Task<bool> _UpdateBoardBy_IDcadre(string ID_canbo, int ID_Ban){
            using var connection = await _context.Get_MySqlConnection();
            const string sql = @"UPDATE hoatdong SET ID_Ban=@ID_Ban WHERE ID_canbo=@ID_canbo;";

            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_canbo", ID_canbo);
                command.Parameters.AddWithValue("@ID_Ban", ID_Ban);
                
                int count = Convert.ToInt32(await command.ExecuteNonQueryAsync());
                if(count < 0) return false;
            }
            return true; 
        }
    }
}