using BackEnd.core.Entities;
using BackEnd.src.infrastructure.DataAccess.Context;
using MySql.Data.MySqlClient;
using BackEnd.src.web_api.DTOs;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using log4net;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class ConstituencyReposistory : IDisposable,IConstituencyRepository
    {
        private readonly DatabaseContext _context;
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program)); 

        //Khởi tạo
        public ConstituencyReposistory(DatabaseContext context) => _context = context;

        //Hủy
        public void Dispose() => _context.Dispose();

        //Liệt kê
        public async Task<List<ConstituencyDto>> _GetListOfConstituency(){
            var list = new List<ConstituencyDto>();

            using var connection = await _context.Get_MySqlConnection();
            using var command = new MySqlCommand("SELECT * FROM donvibaucu", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while(await reader.ReadAsync()){
                list.Add(new ConstituencyDto{
                    ID_DonViBauCu = reader.GetInt32(reader.GetOrdinal("ID_DonViBauCu")),
                    TenDonViBauCu = reader.GetString(reader.GetOrdinal("TenDonViBauCu")),
                    DiaChi = reader.GetString(reader.GetOrdinal("DiaChi")),
                    ID_QH = reader.GetInt32(reader.GetOrdinal("ID_QH"))
                });
            }
            return list;
        }

        //Thêm
        public async Task<bool> _AddConstituency(Constituency donvibaucu){
            using var connection = await _context.Get_MySqlConnection();
            
            //Kiểm tra xem ID quận huyện có tồn tại không. Nếu không thì trả về false
            string checkInput = "SELECT COUNT(*) FROM quanhuyen WHERE ID_QH = @ID_QH";
            using(var commandCheck = new MySqlCommand(checkInput, connection)){
                commandCheck.Parameters.AddWithValue("@ID_QH",donvibaucu.ID_QH);
                int count = Convert.ToInt32(await commandCheck.ExecuteScalarAsync());
                if(count < 1)
                    return false;
            }

            //Thực hiện thêm            
            string Input = @"
                INSERT INTO donvibaucu(ID_DonViBauCu,TenDonViBauCu,DiaChi,ID_QH) 
                VALUES(@ID_DonViBauCu ,@TenDonViBauCu,@DiaChi,@ID_QH);";
            
            using (var commandAdd = new MySqlCommand(Input, connection)){
                commandAdd.Parameters.AddWithValue("@ID_DonViBauCu",donvibaucu.ID_DonViBauCu);
                commandAdd.Parameters.AddWithValue("@TenDonViBauCu",donvibaucu.TenDonViBauCu);
                commandAdd.Parameters.AddWithValue("@DiaChi",donvibaucu.DiaChi);
                commandAdd.Parameters.AddWithValue("@ID_QH",donvibaucu.ID_QH);
                await commandAdd.ExecuteNonQueryAsync();
            } 

            return true; 
        }

        //Lấy theo ID
        public async Task<Constituency> _GetConstituencyBy_ID(string id){
            using var connection = await _context.Get_MySqlConnection();

            const string sql = @"
                SELECT * FROM donvibaucu 
                WHERE ID_DonViBauCu = @ID_DonViBauCu";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ID_DonViBauCu",id);

            using var reader = await command.ExecuteReaderAsync();
            if(await reader.ReadAsync()){
                return new Constituency{
                    ID_DonViBauCu = reader.GetInt32(reader.GetOrdinal("ID_DonViBauCu")),
                    TenDonViBauCu = reader.GetString(reader.GetOrdinal("TenDonViBauCu")),
                    DiaChi = reader.GetString(reader.GetOrdinal("DiaChi")),
                    ID_QH = reader.GetInt32(reader.GetOrdinal("ID_QH"))
                };
            }

            return null;
        }

        //Sửa
        public async Task<bool> _EditConstituencyBy_ID(string ID, ConstituencyDto Constituency){
            using var connection = await _context.Get_MySqlConnection();

            //Tìm kiếm quận huyện có tồn tại không
            const string sqlCheck = "SELECT COUNT(*) FROM quanhuyen WHERE ID_QH = @ID_QH";
            using(var command0 = new MySqlCommand(sqlCheck, connection)){
                command0.Parameters.AddWithValue("@ID_QH",Constituency.ID_QH);
                int count = Convert.ToInt32(await command0.ExecuteScalarAsync());
                
                if(count < 1)
                    return false;
            }

            //Cập nhật
            const string sqlupdate = @"
            UPDATE donvibaucu 
            SET TenDonViBauCu = @TenDonViBauCu, DiaChi = @DiaChi ,ID_QH = @ID_QH 
            WHERE ID_DonViBauCu = @ID_DonViBauCu;";
            using( var command = new MySqlCommand(sqlupdate, connection)){
                command.Parameters.AddWithValue("@ID_DonViBauCu",ID);
                command.Parameters.AddWithValue("@TenDonViBauCu",Constituency.TenDonViBauCu);
                command.Parameters.AddWithValue("@DiaChi",Constituency.DiaChi);
                command.Parameters.AddWithValue("@ID_QH",Constituency.ID_QH);

                //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
                int rowAffected = await command.ExecuteNonQueryAsync();
                return rowAffected > 0;
            }
            
        }

        //Xóa
        public async Task<bool> _DeleteConstituencyBy_ID(string ID){
            using var connection = await _context.Get_MySqlConnection();

            const string sqlupdate = @"
                DELETE FROM donvibaucu
                WHERE ID_DonViBauCu = @ID_DonViBauCu";
            
            using var command = new MySqlCommand(sqlupdate, connection);
            command.Parameters.AddWithValue("@ID_DonViBauCu",ID);
        
            //Lấy số hàng bị tác động nếu > 0 thì true, ngược lại là false
            int rowAffected = await command.ExecuteNonQueryAsync();
            return rowAffected > 0;
        }

        //Kiểm tra xem đơn vj bầu cử có tồn tại không
        public async Task<bool> _CheckIfConstituencyExists(string ID_DonViBauCu, MySqlConnection connection){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
                
            const string sql = "SELECT COUNT(ID_DonViBauCu) FROM donvibaucu WHERE ID_DonViBauCu =@ID_DonViBauCu;";
            
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_DonViBauCu",ID_DonViBauCu);
                
                int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                return count > 0;
            }
        }

        //Kiểm tra ID của đơn vị bầu, mã cử tri và ngày bắt đầu có cùng tồn tại không
        public async Task<bool> _CheckVoterID_ConsituencyID_andPollingDateTogether(string ID_DonViBauCu, string IDcutri, DateTime ngayBD, MySqlConnection connection){
            _log.Info($">> Kiểm tra đơn vị bầu cử {ID_DonViBauCu} - cử tri: {IDcutri} <<");

            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
                
            const string sql = @"
            SELECT COUNT(ngayBD)
            FROM trangthaibaucu 
            WHERE ID_CuTri =@ID_CuTri AND ngayBD =@ngayBD AND ID_DonViBauCu =@ID_DonViBauCu; 
            ";
            
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_DonViBauCu",ID_DonViBauCu);
                command.Parameters.AddWithValue("@ID_CuTri", IDcutri);
                command.Parameters.AddWithValue("@ngayBD", ngayBD);
                
                int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                return count > 0;
            }
        }

        //Kiểm tra ID của đơn vị bầu, mã ững củ viên và ngày bắt đầu có cùng tồn tại không
        public async Task<bool> _CheckCandidateID_ConsituencyID_andPollingDateTogether(string ID_DonViBauCu, string ID_ucv, DateTime ngayBD, MySqlConnection connection){
            _log.Info($"Kiểm tra đơn vị bầu cử bên ứng cử viên");

            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
                
            const string sql = @"
            SELECT COUNT(ngayBD)
            FROM trangthaibaucu 
            WHERE ID_ucv =@ID_ucv AND ngayBD =@ngayBD AND ID_DonViBauCu =@ID_DonViBauCu; 
            ";
            
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_DonViBauCu",ID_DonViBauCu);
                command.Parameters.AddWithValue("@ID_ucv", ID_ucv);
                command.Parameters.AddWithValue("@ngayBD", ngayBD);
                
                int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                return count > 0;
            }
        }

        //Kiểm tra ID của đơn vị bầu, mã cán bộ và ngày bắt đầu có cùng tồn tại không
        public async Task<bool> _CheckCadreID_ConsituencyID_andPollingDateTogether(string ID_DonViBauCu, string ID_CanBo, DateTime ngayBD, MySqlConnection connection){
            _log.Info($"Kiểm tra đơn vị bầu cử bên cán bộ");
            
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
                
            const string sql = @"
            SELECT COUNT(ngayBD)
            FROM trangthaibaucu 
            WHERE ID_CanBo =@ID_CanBo AND ngayBD =@ngayBD AND ID_DonViBauCu =@ID_DonViBauCu; 
            ";
            
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@ID_DonViBauCu",ID_DonViBauCu);
                command.Parameters.AddWithValue("@ID_CanBo", ID_CanBo);
                command.Parameters.AddWithValue("@ngayBD", ngayBD);
                
                int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                return count > 0;
            }
        }

    }
}