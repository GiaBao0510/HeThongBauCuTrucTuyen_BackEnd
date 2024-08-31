using System;
using System.IO;
using DotNetEnv;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using BackEnd.src.core.Interfaces;

namespace BackEnd.infrastructure.config
{
    class AppConfig : IAppConfig
    {
        //Thuộc tính
        private readonly string Server_Host, Server_Port,
        MYSQL_HOST, MYSQL_DBNAME, MYSQL_USER, MYSQL_PASSWORD, MYSQL_PORT,
        MONGODB_HOST, MONGODB_PORT, 
        CLOUDINARY_URL;

        //Hàm khởi tạo
        public AppConfig()
        {
            //Load dữ liệu từ file .env
            DotNetEnv.Env.Load("src/infrastructure/Config/.env");

            Server_Host = Environment.GetEnvironmentVariable("SERVER_HOST");
            Server_Port = Environment.GetEnvironmentVariable("SERVER_PORT");
            MYSQL_HOST = Environment.GetEnvironmentVariable("MYSQL_HOST");
            MYSQL_DBNAME = Environment.GetEnvironmentVariable("MYSQL_DBNAME");
            MYSQL_USER = Environment.GetEnvironmentVariable("MYSQL_USER");
            MYSQL_PASSWORD = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
            MYSQL_PORT = Environment.GetEnvironmentVariable("MYSQL_PORT");
            MONGODB_HOST = Environment.GetEnvironmentVariable("MONGODB_HOST");
            MONGODB_PORT= Environment.GetEnvironmentVariable("MONGODB_PORT");
            CLOUDINARY_URL = Environment.GetEnvironmentVariable("CLOUDINARY_URL");
            Console.WriteLine($"Loaded SERVER_HOST: {Server_Host}");
            Console.WriteLine($"Loaded SERVER_PORT: {Server_Port}");
        }
        
        // --- Phương thức lấy thông tin
        public string GetServerIP(){
            return Server_Host;
        }
        public string GetServerPort(){
            return Server_Port;
        }
        public string GetServerAddress(){
            return $"http://{Server_Host}:{Server_Port}/";
        }
        public string GetMySQLConnectionString(){
            return $"Server ={MYSQL_HOST},{MYSQL_PORT}; Database={MYSQL_DBNAME}; User ID={MYSQL_USER}; Password={MYSQL_PASSWORD}";
        }
        public string GetMongoDBConnectionString(){
            return $"mongodb://{MONGODB_HOST}:{MONGODB_PORT}/";
        }
        public string GetCloudinaryURL(){
            return CLOUDINARY_URL;
        }
    }
}