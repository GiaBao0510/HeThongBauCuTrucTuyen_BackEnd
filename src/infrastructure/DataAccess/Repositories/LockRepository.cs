
using System.Globalization;
using System.Numerics;
using BackEnd.src.core.Entities;
using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class LockRepository : IDisposable, ILockRepository
    {
        private readonly DatabaseContext _context;
        private readonly IElectionsRepository _electionsRepository;

        //khởi tạo
        public LockRepository(DatabaseContext context, IElectionsRepository electionsRepository){
            _context = context;
            _electionsRepository = electionsRepository;
        }

        //Hàm hủy
        public void Dispose() => _context.Dispose();

        //1.Lấy thông tin khóa công khai theo ngày bắt đầu
        public async Task<LockDTO> _getLockBasedOnElectionDate(string ngayBD){
            using var connection = await _context.Get_MySqlConnection();
            try{

                //Kiểm tra ngày bắt đầu bầu cử có tồn tại không
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime votingDay = DateTime.ParseExact(ngayBD,"yyyy-MM-dd HH:mm:ss",provider);
                bool check_ngayBD = await _electionsRepository._CheckIfElectionTimeExists(votingDay, connection);
                if(!check_ngayBD) return null;

                //Lấy thông tin trả về kết quả
                const string sql = @"
                SELECT *
                FROM khoa WHERE ngayBD = @ngayBD;";

                using (var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ngayBD",ngayBD);
                    using var reader = await command.ExecuteReaderAsync();
                    if(await reader.ReadAsync()){
                        return new LockDTO{
                            ID_Khoa = reader.GetInt32(reader.GetOrdinal("ID_Khoa")),
                            NgayTao = reader.GetDateTime(reader.GetOrdinal("NgayTao")),
                            N = (BigInteger)reader.GetDecimal(reader.GetOrdinal("N")),
                            G = (BigInteger)reader.GetDecimal(reader.GetOrdinal("G")),
                            path_PK = reader.GetString(reader.GetOrdinal("path_PK")),
                        };
                    }
                }

                return null;
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

        //1.1 Lấy thông tin khóa công khai theo ngày bắt đầu
        public async Task<LockDTO> _getLockBasedOnElectionDate(string ngayBD, MySqlConnection connection){
            try{
                    //Kiểm tra trạng thái kết nối trước khi mở
                if(connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                //Kiểm tra ngày bắt đầu bầu cử có tồn tại không
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime votingDay = DateTime.ParseExact(ngayBD,"yyyy-MM-dd HH:mm:ss",provider);
                bool check_ngayBD = await _electionsRepository._CheckIfElectionTimeExists(votingDay, connection);
                if(!check_ngayBD) return null;

                //Lấy thông tin trả về kết quả
                const string sql = @"
                SELECT *
                FROM khoa WHERE ngayBD = @ngayBD;";

                using (var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ngayBD",ngayBD);
                    using var reader = await command.ExecuteReaderAsync();
                    if(await reader.ReadAsync()){
                        return new LockDTO{
                            ID_Khoa = reader.GetInt32(reader.GetOrdinal("ID_Khoa")),
                            NgayTao = reader.GetDateTime(reader.GetOrdinal("NgayTao")),
                            N = (BigInteger)reader.GetDecimal(reader.GetOrdinal("N")),
                            G = (BigInteger)reader.GetDecimal(reader.GetOrdinal("G")),
                            path_PK = reader.GetString(reader.GetOrdinal("path_PK")),
                        };
                    }
                }

                return null;
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

        //2.Lấy tất cả thông tin khóa
        public async Task<List<LockDTO>> _getListKey(){
            using var connection = await _context.Get_MySqlConnection();
            var list = new List<LockDTO>();
            try{

                //Lấy thông tin trả về kết quả
                const string sql = @"
                SELECT *
                FROM khoa;";

                using (var command = new MySqlCommand(sql, connection)){
                    using var reader = await command.ExecuteReaderAsync();
                        while(await reader.ReadAsync()){
                            list.Add(new LockDTO{
                                ID_Khoa = reader.GetInt32(reader.GetOrdinal("ID_Khoa")),
                                NgayTao = reader.GetDateTime(reader.GetOrdinal("NgayTao")),
                                N = (BigInteger)reader.GetDecimal(reader.GetOrdinal("N")),
                                G = (BigInteger)reader.GetDecimal(reader.GetOrdinal("G")),
                                path_PK = reader.GetString(reader.GetOrdinal("path_PK")),
                            });
                        }

                    return list;
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

        //3.Xóa khóa theo ngày bắt đầu
        public async Task<bool> _deleteKeyBasedOnElectionDate(string ngayBD){
            using var connection = await _context.Get_MySqlConnection();
            try{

                //Kiểm tra ngày bắt đầu bầu cử có tồn tại không
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime votingDay = DateTime.ParseExact(ngayBD,"yyyy-MM-dd HH:mm:ss",provider);
                bool check_ngayBD = await _electionsRepository._CheckIfElectionTimeExists(votingDay, connection);
                if(!check_ngayBD) return false;

                //Lấy thông tin trả về kết quả
                const string sql = @"
                DELETE FROM khoa WHERE ngayBD = @ngayBD;";

                using (var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ngayBD",ngayBD);
                    await command.ExecuteNonQueryAsync();
                }

                
                //Thiết phần xóa tệp tin dựa trên ngày BD

                return true;
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

        //4.Lấy khóa thông tin khóa mật theo ngày bắt đầu
        public async Task<PrivateKeyDTO> _getPrivateKeyBasedOnElectionDateAndKey(string ngayBD){
            using var connection = await _context.Get_MySqlConnection();
            try{
                 //Kiểm tra ngày bắt đầu bầu cử có tồn tại không
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime votingDay = DateTime.ParseExact(ngayBD,"yyyy-MM-dd HH:mm:ss",provider);
                bool check_ngayBD = await _electionsRepository._CheckIfElectionTimeExists(votingDay, connection);
                if(!check_ngayBD) return null;

                //Lấy thông tin trả về kết quả
                string path = null;
                const string sql = @"
                SELECT path_PK
                FROM khoa WHERE ngayBD = @ngayBD;";

                using (var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ngayBD",ngayBD);
                    using var reader = await command.ExecuteReaderAsync();
                    if(await reader.ReadAsync()){
                        path = reader.GetString(reader.GetOrdinal("path_PK"));
                    }
                }

                //Nếu đường dẫn tồn tại thì đọc
                if(File.Exists(path)){
                    //Đọc toàn bộ nội dung tệp
                    string content = File.ReadAllText(path);

                    //Tách các giá trị trong tệp tin dựa vào dấu ','
                    string[] values = content.Split(',');

                    if(values.Length == 2){
                        //Chuyển đổi giá trị đọc được thành BigInteger
                        return new PrivateKeyDTO{
                            lamda = BigInteger.Parse(values[0].Trim()),
                            muy = BigInteger.Parse(values[1].Trim())
                        };
                    }
                }else{
                    Console.WriteLine("File khong tồn tại");
                }
                

                return null;
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