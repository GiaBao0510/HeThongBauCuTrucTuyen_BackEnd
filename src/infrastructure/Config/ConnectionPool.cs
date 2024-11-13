
using System.Collections.Concurrent;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.Config
{
    public class ConnectionPool: IDisposable
    {
        private readonly ConcurrentBag<MySqlConnection> _connections;
        private readonly int _maxSize;
        private bool _disposed;

        //Khởi tạo
        public ConnectionPool(int Max){
            _connections = new ConcurrentBag<MySqlConnection>();
            _maxSize = Max;
        }

        //Trả về 
        public void Return(MySqlConnection connection){
            if(!_disposed && _connections.Count < _maxSize)
                _connections.Add(connection);
            else
                connection.Dispose();
        }
        
        public void Dispose()
        {
            if (!_disposed)
            {
                foreach (var connection in _connections)
                    connection.Dispose();
                
                _disposed = true;
            }
        }
    }
}