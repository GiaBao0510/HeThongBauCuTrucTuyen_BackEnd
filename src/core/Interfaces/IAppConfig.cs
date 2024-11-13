namespace BackEnd.src.core.Interfaces
{
    public interface IAppConfig
    {
        public string GetServerIP();
        public string GetServerPort();
        public string GetServerAddress();
        public string GetMySQLConnectionString();
        public string GetMongoDBConnectionString();
    }
} 