using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.core.Interfaces;
using BackEnd.src.core.Interfaces;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.Context
{
    public class DatabaseContext : IDisposable
    {
        //Thuộc tính
        private readonly IAppConfig _appconfig;
        private MySqlConnection _MysqlConnection;
        //private 

        //Phương thức
        public DatabaseContext(IAppConfig appConfig){
            _appconfig = appConfig;
        }

        public MySqlConnection CreateConnection(){
            return new MySqlConnection(_appconfig.GetMySQLConnectionString());
        }

        //Kết nối với MySQL
        public async Task<MySqlConnection> Get_MySqlConnection(){
            
            //Nếu kiểm tra chưa thấy kết nối đến thì thực hiện kết nối hoặc
            if(_MysqlConnection == null || _MysqlConnection.State != ConnectionState.Open || _MysqlConnection.State == ConnectionState.Closed){
                _MysqlConnection = new MySqlConnection(_appconfig.GetMySQLConnectionString());
                await _MysqlConnection.OpenAsync();
            }   

            return _MysqlConnection;
        }

        public void Dispose(){
            //Ngắt kết nối
            if(_MysqlConnection != null){
                _MysqlConnection.Dispose();
                _MysqlConnection = null;
            }
        }
    }
}