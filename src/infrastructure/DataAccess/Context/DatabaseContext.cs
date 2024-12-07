using log4net;
using BackEnd.src.core.Interfaces;
using MySql.Data.MySqlClient;
using BackEnd.src.infrastructure.Config;

namespace BackEnd.src.infrastructure.DataAccess.Context
{
    public class DatabaseContext : IDisposable
    {
        //Thuộc tính
        private readonly string _connectionString;
        private readonly SemaphoreSlim _semaphore; // Giới hạn xử lý task đồng thòi là 10
        private const int MaxRetries = 3;
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program)); 
        private bool _disposed;
        private static readonly object _lock = new object();
        private readonly ConnectionPool _pool;

        //Khởi tạo
        public DatabaseContext(IAppConfig appConfig){
            _connectionString = GetConnectionStringWithPooling(appConfig.GetMySQLConnectionString());
            _semaphore = new SemaphoreSlim(20); // Giới hạn xử lý task đồng thời là 10
            _pool = new ConnectionPool(Max: 100); // Tạo pool kết nối
        }

        //Thiết lập trước khi kết nôi đến MySQL
        private string GetConnectionStringWithPooling(string originalConnection){

            return new MySqlConnectionStringBuilder(originalConnection){
                Pooling = true,             //Tái sử dụng kết nối , thay vì mỗi lần mở cổng kết nối
                ConnectionTimeout = 30,     //Thiết lập thời gian chờ để kết nối, Nếu gặp ngoại lệ sẽ ném ra
                DefaultCommandTimeout = 30, //Thiết lập thời gian chờ mặc định cho mỗi lệnh truy vấn sql
                MaximumPoolSize = 200,      //Giới hạn tối đa là 200 kết nối, nếu hơn thì sẽ chờ cho đến khi được giải phóng
                MinimumPoolSize = 5,        //Số kết nối tối thiểu
                ConnectionLifeTime = 300,   //Thời gian sống của kết nối
            }.ToString();
        }

        public MySqlConnection  CreateConnection(){
            return new MySqlConnection(_connectionString);
        }

        //Kết nối với MySQL
        public async Task<MySqlConnection> Get_MySqlConnection() {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DatabaseContext));

            await _semaphore.WaitAsync();

            try
            {
                var connection = await GetConnectionWithRetry();
                return connection;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to establish database connection", ex);
            }finally{
                _semaphore.Release();
            }
        }

        private async Task<MySqlConnection> GetConnectionWithRetry()
        {
            int retryCount = 0;
            var delay = TimeSpan.FromMilliseconds(100);

            while (true)
            {
                try
                {
                    var connection = new MySqlConnection(_connectionString);
                    await connection.OpenAsync();
                    
                    return connection;
                }
                catch (MySqlException ex) when (ex.Number == 1042 || ex.Number == 0)
                {
                    retryCount++;
                    if (retryCount >= MaxRetries)
                        throw new Exception($"Failed to connect to database after multiple retries {ex.Message}", ex);
                        
                    await Task.Delay(delay);
                    delay *= 2; // Exponential backoff
                    continue;
                }
                throw new Exception("Failed to establish database connection");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _semaphore.Dispose();
                    _pool.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}