﻿using log4net;
using log4net.Config;
using BackEnd.src.infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Net;


namespace BackEnd
{
    class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program)); 

        [STAThread]
        static void Main(string[] args)
        {
                // ----- Config log4net
            //BasicConfigurator.Configure();
            XmlConfigurator.Configure(new FileInfo("./LoggerConfig.xml"));
            _log.Info($"\t ---- Start Application - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")} ---- ");

                //Cấu hienhf bên Swagger
            // var builder = WebApplication.CreateBuilder(args);
            // builder.Services.AddSwaggerGen(c => {
            //     c.ResolveConflictingActions(apiDescription => apiDescription.First());
            //     c.SwaggerDoc("v1", new OpenApiInfo{Title="My API", Version="v1" });
            //     c.OperationFilter<FileUploadService>();
            // });
           CreateHostBuilder(args).Build().Run();
        }
         public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel(kestrelServerOptions=>{
                        
                        //Thiết lập lắng bất kỳ IP của thiết bị nào trên cổng 3000
                        kestrelServerOptions.Listen(IPAddress.Any,3000);
                        kestrelServerOptions.ListenAnyIP(5085);
                        
                        //Lắp nghe trên cổng 7147 với https
                        // kestrelServerOptions.Listen(IPAddress.Any, 7147, listenOpt =>{
                        //     listenOpt.UseHttps();
                        // });

                        //Lắng nghe cổng 3001 chạy server trên ứng dụng
                        kestrelServerOptions.ListenLocalhost(3001);
                    });
                });
    }
}


