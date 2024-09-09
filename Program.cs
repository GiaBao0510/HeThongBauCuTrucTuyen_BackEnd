using BackEnd.src.infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;


namespace BackEnd
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo{Title="My API", Version="v1" });
                c.OperationFilter<FileUploadService>();
            });

           CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] 
            args) => Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(webBuilder =>{
                        webBuilder.UseStartup<Startup>();
                    });
    }
}


