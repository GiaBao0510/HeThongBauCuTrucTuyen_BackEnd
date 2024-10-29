using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class ElectionResultsRepository: IDisposable, IElectionResultsRepository
    {
        private readonly DatabaseContext _context;
        private readonly IVoterRepository _voterRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly ICadreRepository _cadreRepository;

        //Khởi tạo
        public ElectionResultsRepository(
            DatabaseContext context, 
            IVoterRepository voterRepository, 
            ICandidateRepository candidateRepository, 
            ICadreRepository cadreRepository
        ){
            _context = context;
            _voterRepository = voterRepository;
            _candidateRepository = candidateRepository;
            _cadreRepository = cadreRepository;
        }

        //HỦy
        public void Dispose() => _context.Dispose();

        //1. Lấy danh sách kết quả bầu cử đã công bố cho đối tượng
        public async Task<List<ElectionResultDetailsDTO>> _getDetailedListOfElectionResult(string BangChiTiet, string BangDoiTuong,string index, string value ,MySqlConnection connection){
            try{
                //Kiểm tra trạng thái kết nối trước khi mở
                if(connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();
                
                var list = new List<ElectionResultDetailsDTO>();
                string sql = @$"
                SELECT kbc.ngayBD, kbc.ngayKT, kbc.TenKyBauCu, dm.TenCapUngCu,dv.TenDonViBauCu,kbc.MoTa,kbc.CongBo
                FROM {BangDoiTuong} obj 
                JOIN {BangChiTiet} bct ON obj.{index} = bct.{index}
                JOIN kybaucu kbc ON kbc.ngayBD = bct.ngayBD
                JOIN danhmucungcu dm ON dm.ID_Cap = kbc.ID_Cap
                JOIN donvibaucu dv ON dm.ID_DonViBauCu = dv.ID_DonViBauCu
                WHERE kbc.CongBo = '1' AND obj.{index} = @value;";

                using(var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("value", value);
                    using var reader = await command.ExecuteReaderAsync();
                    while(await reader.ReadAsync()){
                        list.Add(new ElectionResultDetailsDTO{
                            ngayBD = reader.GetDateTime(reader.GetOrdinal("ngayBD")),
                            ngayKT = reader.GetDateTime(reader.GetOrdinal("ngayKT")),
                            TenKyBauCu = reader.GetString(reader.GetOrdinal("TenKyBauCu")),
                            TenCapUngCu = reader.GetString(reader.GetOrdinal("TenCapUngCu")),
                            TenDonViBauCu = reader.GetString(reader.GetOrdinal("TenDonViBauCu")),
                            MoTa = reader.GetString(reader.GetOrdinal("MoTa")),
                            CongBo = reader.GetString(reader.GetOrdinal("CongBo")),
                        });
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

        //2. Lấy bảng chi tiết kết quả bầu cử đã công bố cho cử tri
        public async Task<List<ElectionResultDetailsDTO>> _getDetailedListOfElectionResultForVoter(string ID_CuTri){
            try{
                using var connection = await _context.Get_MySqlConnection();
                
                //Kiểm tra xem cử tri có tồn tại không
                bool checkVoterExist = await _voterRepository._CheckVoterExists(ID_CuTri, connection);

                if(!checkVoterExist) return null;

                return await _getDetailedListOfElectionResult("trangthaibaucu","cutri","ID_CuTri",ID_CuTri, connection);

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

        //3. Lấy bảng chi tiết kết quả bầu cử đã công bố cho ứng cử viên
        public async Task<List<ElectionResultDetailsDTO>> _getDetailedListOfElectionResultForCandidate(string ID_ucv){
            try{
                using var connection = await _context.Get_MySqlConnection();
                
                //Kiểm tra xem cử tri có tồn tại không
                bool checkVoterExist = await _candidateRepository._CheckCandidateExists(ID_ucv, connection);

                if(!checkVoterExist) return null;

                return await _getDetailedListOfElectionResult("ketquabaucu","ungcuvien","ID_ucv",ID_ucv, connection);

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

        //4. Lấy bảng chi tiết kết quả bầu cử đã công bố cho cán bộ
        public async Task<List<ElectionResultDetailsDTO>> _getDetailedListOfElectionResultForCandre(string ID_canbo){
            try{
                using var connection = await _context.Get_MySqlConnection();
                
                //Kiểm tra xem cử tri có tồn tại không
                bool checkVoterExist = await _cadreRepository._CheckCadreExists(ID_canbo, connection);

                if(!checkVoterExist) return null;

                return await _getDetailedListOfElectionResult("hoatdong","canbo","ID_canbo",ID_canbo, connection);

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