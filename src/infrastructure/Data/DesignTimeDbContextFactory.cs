using BackEnd.src.infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace HeThongBauCuTrucTuyen_BackEnd.src.infrastructure.Data
{
    public class DesignTimeDbContextFactory: IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args){
            
            //Tạo cấu hình từ appsetings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
                
            //Kết nối đến CSDL
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("MySQL");
            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            
            return new ApplicationDbContext(builder.Options);
        }
    }
}