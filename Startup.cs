using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BackEnd.infrastructure.config;
using BackEnd.src.core.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MySql.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.infrastructure.DataAccess.Repositories;
using BackEnd.src.infrastructure.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BackEnd
{
    public class Startup
    {
        public IConfiguration Configuration{get;}

        //Hàm khởi tạo
        public Startup(IConfiguration configuration){
            Configuration = configuration;
        }

        //Thường khai báo các services Dependency
        public void ConfigureServices(IServiceCollection services){
            var serverMysqlVersion = new MySqlServerVersion(new Version(9,0,1));
            
            services.AddControllers();
            services.AddSwaggerGen(c =>{
                c.SwaggerDoc("v1", new OpenApiInfo {Title="BauCuTrucTuyen", Version="v1"});
            });
            services.AddDbContext<ApplicationDbContext>(option =>{
                option.UseMySql( Configuration.GetConnectionString("MySQL") , serverMysqlVersion);
            });

            // --- Xác thực cookie
            services.AddRazorPages();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

            
            //-- Đăng ký dịch vụ tại đây
            services.AddSingleton<IAppConfig,AppConfig>();
            services.AddScoped<DatabaseContext>();
            services.AddScoped<RoleReposistory>();
            services.AddScoped<ProvinceReposistory>();
            services.AddScoped<DistrictReposistory>();
        }

        //Riêng các service muốn call thì sẽ goi trong đây
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env){
            if(env.IsDevelopment()){
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI( c => c.SwaggerEndpoint("/swagger/v1/swagger.json","BauCuTrucTuyen v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints=>{
                endpoints.MapControllers();
            });
        }

        // Cấu hình dịch vụ - đăng ký dịch vụ
        public static void _ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IAppConfig, AppConfig>();
        }
    }
}
