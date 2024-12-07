using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.web_api.DTOs;
using log4net;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class ElectionResponse :IDisposable
    {
        private readonly DatabaseContext _context;
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program)); 

        //khởi tạo
        public ElectionResponse(DatabaseContext context)
        {
            _context = context;
        }

        //Hàm hủy
        public void Dispose() => _context.Dispose();

        //Liệt kê
        public async Task<List<ElectionResponseDTO>> _getListOfResponseBasedOnElection(string ngayBD){
            try{
                using var connection = await _context.Get_MySqlConnection();
                var list = new List<ElectionResponseDTO>();
                const string sql = @"
                SELECT ThoiDiem, NoiDung, ID_CuTri
                FROM phanhoicuocbaucu 
                WHERE ngayBD= @ngayBD;";

                using(var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ngayBD", ngayBD);
                    using var reader = await command.ExecuteReaderAsync();

                    while(await reader.ReadAsync()){
                        list.Add(new ElectionResponseDTO{
                            ThoiDiem = reader.GetDateTime(reader.GetOrdinal("ThoiDiem")),
                            NoiDung = reader.GetString(reader.GetOrdinal("NoiDung")),
                            ID_CuTri = reader.GetString(reader.GetOrdinal("ID_CuTri"))
                        });
                    }

                    return list;
                }
            }catch(MySqlException ex){
                _log.Error($"Error message: {ex.Message}");
                _log.Error($"Error Code: {ex.Code}");
                _log.Error($"Error Source: {ex.Source}");
                _log.Error($"Error HResult: {ex.HResult}");
                throw;
            }
            catch(Exception ex){
                _log.Error($"Error message: {ex.Message}");
                _log.Error($"Error Source: {ex.Source}");
                _log.Error($"Error StackTrace: {ex.StackTrace}");
                _log.Error($"Error TargetSite: {ex.TargetSite}");
                _log.Error($"Error HResult: {ex.HResult}");
                _log.Error($"Error InnerException: {ex.InnerException}");
                throw;
            }
        }

        //Thêm
        public async Task _addResponseForElection(ElectionResponseDTO electionResponseDTO){
            try{
                using var connection = await _context.Get_MySqlConnection();
                const string sql = @"
                INSERT INTO  phanhoicuocbaucu(NoiDung,ID_CuTri,ngayBD)
                	VALUES(@NoiDung,@ID_CuTri,@ngayBD);";
                
                using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@NoiDung", electionResponseDTO.NoiDung);
                command.Parameters.AddWithValue("@ID_CuTri", electionResponseDTO.ID_CuTri);
                command.Parameters.AddWithValue("@ngayBD", electionResponseDTO.ngayBD);

                await command.ExecuteNonQueryAsync();
                
            }catch(MySqlException ex){
                _log.Error($"Error message: {ex.Message}");
                _log.Error($"Error Code: {ex.Code}");
                _log.Error($"Error Source: {ex.Source}");
                _log.Error($"Error HResult: {ex.HResult}");
                throw;
            }
            catch(Exception ex){
                _log.Error($"Error message: {ex.Message}");
                _log.Error($"Error Source: {ex.Source}");
                _log.Error($"Error StackTrace: {ex.StackTrace}");
                _log.Error($"Error TargetSite: {ex.TargetSite}");
                _log.Error($"Error HResult: {ex.HResult}");
                _log.Error($"Error InnerException: {ex.InnerException}");
                throw;
            }
        }

        //Sửa đổi thông tin phản hồi theo thời điểm
        public async Task _editResponseForElection(string NoiDung, string ThoiDiem){
            try{
                using var connection = await _context.Get_MySqlConnection();
                const string sql = @"
                UPDATE phanhoicuocbaucu
                SET NoiDung = @NoiDung
                WHERE ThoiDiem =@ThoiDiem;";
                
                using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ThoiDiem", ThoiDiem);
                command.Parameters.AddWithValue("@NoiDung", NoiDung);

                await command.ExecuteNonQueryAsync();
                
            }catch(MySqlException ex){
                _log.Error($"Error message: {ex.Message}");
                _log.Error($"Error Code: {ex.Code}");
                _log.Error($"Error Source: {ex.Source}");
                _log.Error($"Error HResult: {ex.HResult}");
                throw;
            }
            catch(Exception ex){
                _log.Error($"Error message: {ex.Message}");
                _log.Error($"Error Source: {ex.Source}");
                _log.Error($"Error StackTrace: {ex.StackTrace}");
                _log.Error($"Error TargetSite: {ex.TargetSite}");
                _log.Error($"Error HResult: {ex.HResult}");
                _log.Error($"Error InnerException: {ex.InnerException}");
                throw;
            }
        }

        //Xóa
        public async Task _deleteResponseForElection(string ThoiDiem){
            try{
                using var connection = await _context.Get_MySqlConnection();
                const string sql = @"
                DELETE FROM phanhoicuocbaucu WHERE ThoiDiem = @ThoiDiem;";
                
                using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ThoiDiem", ThoiDiem);

                await command.ExecuteNonQueryAsync();
            }catch(MySqlException ex){
                _log.Error($"Error message: {ex.Message}");
                _log.Error($"Error Code: {ex.Code}");
                _log.Error($"Error Source: {ex.Source}");
                _log.Error($"Error HResult: {ex.HResult}");
                throw;
            }
            catch(Exception ex){
                _log.Error($"Error message: {ex.Message}");
                _log.Error($"Error Source: {ex.Source}");
                _log.Error($"Error StackTrace: {ex.StackTrace}");
                _log.Error($"Error TargetSite: {ex.TargetSite}");
                _log.Error($"Error HResult: {ex.HResult}");
                _log.Error($"Error InnerException: {ex.InnerException}");
                throw;
            }
        }

    }
}