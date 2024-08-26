using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Hosting;
using BackEnd.src.core.Interfaces;
using BackEnd.infrastructure.config;
using BackEnd.src.infrastructure.Services;
using Microsoft.Extensions.Hosting;


namespace BackEnd
{
    class Program
    {
        static  void Main(string[] args)
        {
           CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] 
            args) => Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(webBuilder =>{
                        webBuilder.UseStartup<Startup>();
                    });
    }
}


