using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.infrastructure.DataAccess.Context;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class StatisticsRepository: IDisposable, IStatisticsRepository
    {
        private readonly DatabaseContext _context;
        private const int CommandTimeout = 30;
        private readonly IMemoryCache _cache;
        private Random random = new Random();
        private int CacheDurationMinutes = 5;

        // khởi tạo
        public StatisticsRepository(DatabaseContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
            CacheDurationMinutes += random.Next(1,4);       //Tránh tường hợp tất cả các key hết hạn cùng lúc
        }
        
        //Hàm hủy
        public void Dispose()
        {
            _context.Dispose();
        }

        // Hàm thực thi câu lệnh truy vấn từ csdl
        private async Task<int> ExecuteCountQuery(string sql, Dictionary<string, object> parameters = null){
            for (int retry = 0; retry < 3; retry++){
                try {
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
                }
                catch (MySqlException ex) when (ex.Number == 1042 || ex.Number == 0){
                    if (retry == 2) throw;
                    await Task.Delay(100 * (retry + 1));
                }
            }

            throw new Exception("Max retries reached");
        }

        //Hàm thực lưu lưu trong cache
        private async Task<int> ExecuteCountQueryWithCache(string cacheKey, Func<Task<int>> query){
            // Nếu có trong cache thì trả về
            if(_cache.TryGetValue(cacheKey, out int cachedResult)){
                return cachedResult;
            }

            //Nếu không có thì thực thi truy vấn và lưu vào cache
            var result = await query();
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
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
            
            return await ExecuteCountQueryWithCache("NumberOfConstituencies", async () =>{
                const string sql = @"
                    SELECT COUNT(ID_DonViBauCu) FROM donvibaucu;";
                
                return await ExecuteCountQuery(sql);
            });
        }
        
        //8. Số lượng danh mục ứng cử 
        public async Task<int> _NumberOfNominations(){

            return await ExecuteCountQueryWithCache("NumberOfNominations",async () =>{
                const string sql = @"
                SELECT COUNT(ID_Cap) FROM danhmucungcu;";
            
                return await ExecuteCountQuery(sql);
            });
        }

        //9. Số lượng chức vụ
        public async Task<int> _NumberOfPositions(){
            return await ExecuteCountQueryWithCache("NumberOfPositions",async () =>{
                const string sql = @"
                    SELECT COUNT(ID_ChucVu) FROM chucvu;";
                
                return await ExecuteCountQuery(sql);
            });
        }

        //10. Số lượng ban
        public async Task<int> _NumberOfBoards(){
            return await ExecuteCountQueryWithCache("NumberOfBoards",async () =>{
                const string sql = @"
                SELECT COUNT(ID_ChucVu) FROM chucvu;";
                
                return await ExecuteCountQuery(sql);
            });
        }
    }
}