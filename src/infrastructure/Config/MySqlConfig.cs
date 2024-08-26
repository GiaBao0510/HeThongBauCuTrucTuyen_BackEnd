using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;
using System.Security.Cryptography.X509Certificates;
using BackEnd.src.core.Interfaces;
using BackEnd.infrastructure.config;
using System.Data.Common;

namespace BackEnd.src.infrastructure.Config
{
    public class MySqlConfig
    {
        private IAppConfig cauHinhMySQL = new AppConfig();
        private string sqlConnectString;

        //Khởi tạo 
        public MySqlConfig(){
            sqlConnectString = cauHinhMySQL.GetMySQLConnectionString();
        }

        //Thực hiện câu truy vấn không cần trả dữ liệu
        public async Task ExecuteMysqlCommandNotQuery(string sql){
            sqlConnectString = cauHinhMySQL.GetMySQLConnectionString();

            var sqlConnection = new MySqlConnection(sqlConnectString);

            //Mở kết nối
            await sqlConnection.OpenAsync();

            //Thi hành câu truy vấn
            using(DbCommand command = sqlConnection.CreateCommand()){
                try{
                    //Thêm mệnh đề truy vấn
                    command.CommandText = sql;

                    //Thực hiện truy vấn ko trả về
                    await command.ExecuteNonQueryAsync();
                }catch(Exception ex){
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            //Đóng kết nối
            await  sqlConnection.CloseAsync();
        }

    }
}