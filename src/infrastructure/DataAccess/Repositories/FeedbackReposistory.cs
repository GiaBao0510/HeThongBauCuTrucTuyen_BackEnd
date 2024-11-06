
using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    
    public class FeedbackReposistory : IDisposable, IFeedbackRepository
    {
        private readonly DatabaseContext _context;
        
        //Khởi tạo
        public FeedbackReposistory(DatabaseContext context) => _context = context;

        //Hủy
        public void Dispose() => _context.Dispose();

        //Lấy Thông tin phản hồi cư tri
        public async Task<List<FeedbackDTO>> _getVoterFeedbackList(){
            using var connection = await _context.Get_MySqlConnection();
            try{
                var list = new List<FeedbackDTO>();
                const string sql = @"
                SELECT ph.Ykien, ph.ThoiDiem, ph.ID_CuTri, nd.HoTen 
                FROM phanhoicutri ph 
                INNER JOIN cutri ob ON ob.ID_CuTri = ph.ID_CuTri
                INNER JOIN nguoidung nd ON nd.ID_user = ob.ID_user;";

                using(var cmd = new MySqlCommand(sql, connection)){
                    using(var reader = await cmd.ExecuteReaderAsync()){
                        while(await reader.ReadAsync()){
                            list.Add(new FeedbackDTO{
                                yKien = reader.GetString(reader.GetOrdinal("Ykien")),
                                ThoiDiem = reader.GetDateTime(reader.GetOrdinal("ThoiDiem")),
                                UserID = reader.GetString(reader.GetOrdinal("ID_CuTri")),
                                HoTen = reader.GetString(reader.GetOrdinal("HoTen"))
                            });
                        }
                    }
                }
                return list;
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

        //Lấy thông tin phản hồi cán bộ
        public async Task<List<FeedbackDTO>> _getCadreFeedbackList(){
            using var connection = await _context.Get_MySqlConnection();
            try{
                var list = new List<FeedbackDTO>();
                const string sql = @"
                SELECT ph.Ykien, ph.ThoiDiem, ph.ID_CanBo, nd.HoTen 
                FROM phanhoicanbo ph 
                INNER JOIN canbo ob ON ob.ID_CanBo = ph.ID_CanBo
                INNER JOIN nguoidung nd ON nd.ID_user = ob.ID_user;";

                using(var cmd = new MySqlCommand(sql, connection)){
                    using(var reader = await cmd.ExecuteReaderAsync()){
                        while(await reader.ReadAsync()){
                            list.Add(new FeedbackDTO{
                                yKien = reader.GetString(reader.GetOrdinal("Ykien")),
                                ThoiDiem = reader.GetDateTime(reader.GetOrdinal("ThoiDiem")),
                                UserID = reader.GetString(reader.GetOrdinal("ID_CanBo")),
                                HoTen = reader.GetString(reader.GetOrdinal("HoTen"))
                            });
                        }
                    }
                }
                return list;
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

        //Lấy thông tin phản hồi ứng cử viên
                public async Task<List<FeedbackDTO>> _getCandidateFeedbackList(){
            using var connection = await _context.Get_MySqlConnection();
            try{
                var list = new List<FeedbackDTO>();
                const string sql = @"
                SELECT ph.Ykien, ph.ThoiDiem, ph.ID_ucv, nd.HoTen 
                FROM phanhoiungcuvien ph 
                INNER JOIN ungcuvien ob ON ob.ID_ucv = ph.ID_ucv
                INNER JOIN nguoidung nd ON nd.ID_user = ob.ID_user;";

                using(var cmd = new MySqlCommand(sql, connection)){
                    using(var reader = await cmd.ExecuteReaderAsync()){
                        while(await reader.ReadAsync()){
                            list.Add(new FeedbackDTO{
                                yKien = reader.GetString(reader.GetOrdinal("Ykien")),
                                ThoiDiem = reader.GetDateTime(reader.GetOrdinal("ThoiDiem")),
                                UserID = reader.GetString(reader.GetOrdinal("ID_ucv")),
                                HoTen = reader.GetString(reader.GetOrdinal("HoTen"))
                            });
                        }
                    }
                }
                return list;
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