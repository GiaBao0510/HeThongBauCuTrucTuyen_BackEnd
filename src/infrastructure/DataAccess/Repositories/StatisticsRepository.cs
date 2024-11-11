using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.infrastructure.DataAccess.Context;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class StatisticsRepository: IDisposable, IStatisticsRepository
    {
        private readonly DatabaseContext _context;
        private const int CommandTimeout = 30;

        // khởi tạo
        public StatisticsRepository(DatabaseContext context)
        {
            _context = context;
        }
        
        //Hàm hủy
        public void Dispose()
        {
            _context.Dispose();
        }

        // Hàm thực thi câu lệnh truy vấn
        private async Task<int> ExecuteCountQuery(string sql, Dictionary<string, object> parameters = null){
            try
            {
                using var connection = await _context.Get_MySqlConnection();
                using var command = new MySqlCommand(sql, connection)
                {
                    CommandTimeout = CommandTimeout
                };

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value);
                    }
                }

                return Convert.ToInt32(await command.ExecuteScalarAsync());
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

        //1.Số lượng các kỳ bầu cử trong năm
        public async Task<int> _countElectionsInYear(string year){
            const string sql = @"
                SELECT COUNT(ngayBD) FROM kybaucu WHERE year(ngayBD) = @year;";
            
            return await ExecuteCountQuery(sql, new Dictionary<string, object> { { "@year", year } });
        }

        //2.Số lượng cử tri tham gia bầu cử trong năm
        public async Task<int> _numberOfVotersParticipatingInElectionsByYear(string year){
            const string sql = @"
                SELECT COUNT(DISTINCT ID_CuTri) 
                FROM trangthaibaucu WHERE year(ngayBD) = @year;";
            
            return await ExecuteCountQuery(sql, new Dictionary<string, object> { { "@year", year } });
            
        }

        //3. Số lượng ứng cử viên đăng ký ghi danh kỳ bầu cử trong năm
        public async Task<int> _numberOfCandidatesParticipatingInElectionsByYear(string year){
            const string sql = @"
                SELECT COUNT(DISTINCT ID_ucv) 
                FROM ketquabaucu WHERE year(ngayBD) = @year;";

            return await ExecuteCountQuery(sql, new Dictionary<string, object> { { "@year", year } });
        }

        //4. Số lượng cán bộ tham dự bầu cử trong năm
        public async Task<int> _numberOfCadresParticipatingInElectionsByYear(string year){
            const string sql = @"
                SELECT  COUNT(DISTINCT ID_CanBo)
                FROM hoatdong WHERE year(ngayBD) = @year;";

            return await ExecuteCountQuery(sql, new Dictionary<string, object> { { "@year", year } });
        }

        //5. Số lượng kỳ bầu cử được công bố trong năm
        public async Task<int> _numberOfElectionsWithAnnouncedResultsBasedOnYear(string year){
            const string sql = @"
                SELECT COUNT(ngayBD) 
                FROM chitietcongboketqua WHERE year(ngayBD) = @year;";

            return await ExecuteCountQuery(sql, new Dictionary<string, object> { { "@year", year } });
        }

        //6.Số lượng tài khoản bị khóa
        public async Task<int> _countLockedAccounts(){
            const string sql = @"
                SELECT COUNT(taikhoan)
                FROM taikhoan WHERE BiKhoa = '1';";

            return await ExecuteCountQuery(sql);
        }

        //7.Số lượng đơn vị bầu cử
        public async Task<int> _NumberOfConstituencies(){
            const string sql = @"
                SELECT COUNT(ID_DonViBauCu) FROM donvibaucu;";
            
            return await ExecuteCountQuery(sql);
        }
        
        //8. Số lượng danh mục ứng cử 
        public async Task<int> _NumberOfNominations(){
            const string sql = @"
                SELECT COUNT(ID_Cap) FROM danhmucungcu;";
            
            return await ExecuteCountQuery(sql);
        }

        //9. Số lượng chức vụ
        public async Task<int> _NumberOfPositions(){
            const string sql = @"
                SELECT COUNT(ID_ChucVu) FROM chucvu;";
            
            return await ExecuteCountQuery(sql);
        }

        //10. Số lượng ban
        public async Task<int> _NumberOfBoards(){
            const string sql = @"
                SELECT COUNT(ID_ChucVu) FROM chucvu;";
                
            return await ExecuteCountQuery(sql);
        }
    }
}