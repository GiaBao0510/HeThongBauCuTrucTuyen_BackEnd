using log4net;
using log4net.Config;
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
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program)); 

        static void Main(string[] args)
        {
                // ----- Config log4net
            //BasicConfigurator.Configure();
            XmlConfigurator.Configure(new FileInfo("./LoggerConfig.xml"));
            _log.Info($"\t ---- Start Application - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")} ---- ");

                //Cấu hienhf bên Swagger
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSwaggerGen(c => {
                c.ResolveConflictingActions(apiDescription => apiDescription.First());
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


