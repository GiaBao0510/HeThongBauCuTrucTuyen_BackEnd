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
using BackEnd.src.infrastructure.Config;
using Microsoft.AspNetCore.Authentication.Cookies;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace BackEnd
{
    public class Startup
    {
        public IConfiguration Configuration{get;}

        //Hàm khởi tạo
        public Startup(IConfiguration configuration) => Configuration = configuration;
        
        //Thường khai báo các services Dependency
        public void ConfigureServices(IServiceCollection services){
            
            //Đăng ký các dịch vụ MVC
            services.AddControllersWithViews();

            var serverMysqlVersion = new MySqlServerVersion(new Version(9,0,1));

            //Cấu hình Cloudinary
            var CloudinaryConfig = Configuration.GetSection("CloudinarySetting");
            var cloudinary = new Cloudinary(new Account(
                CloudinaryConfig["CloudName"],
                CloudinaryConfig["ApiKey"],
                CloudinaryConfig["ApiSecret"]
            ));
            services.AddSingleton(cloudinary);

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
            // services.AddScoped<VouterReposistory>();
            // services.AddScoped<CandidateReposistory>();
            services.AddScoped<ConstituencyReposistory>();
            services.AddScoped<BoardReposistory>();
            services.AddScoped<NotificationsReposistory>();
            services.AddScoped<EducationLevelReposistory>();
            services.AddScoped<ElectionsReposistory>();
            services.AddScoped<EthnicityReposistory>();
            services.AddScoped<PositionReposistory>();
            services.AddScoped<VoteReposistory>();
            services.AddScoped<ListOfPositionReposistory>();
        }

        //Riêng các service muốn call thì sẽ goi trong đây
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env){
            
            //Cấu hình pipline của ứng dụng
            if(env.IsDevelopment()){
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI( c => c.SwaggerEndpoint("/swagger/v1/swagger.json","BauCuTrucTuyen v1"));
            }else{
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //Xử lý lỗi trong môi trường Development và Production
            app.UseExceptionHandler(a => a.Run(async context => {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var error = new {message = "An unexpected error occurred"};
                await context.Response.WriteAsJsonAsync(error);
            }));

            //Xử lý lỗi Not Found 404
            app.UseStatusCodePages(async context => {
                if(context.HttpContext.Response.StatusCode  == (int)HttpStatusCode.NotFound){
                    context.HttpContext.Response.ContentType = "application/json";
                    var error = new {message = "Not Found"};
                    await context.HttpContext.Response.WriteAsJsonAsync(error);
                }
            });

            //Cấu hình các middleware khác
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

        }

        // Cấu hình dịch vụ - đăng ký dịch vụ
        public static void _ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IAppConfig, AppConfig>();
        }
    }
}
