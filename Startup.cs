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
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
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
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.infrastructure.Services;
using BackEnd.src.web_api.Mappings;

namespace BackEnd
{
    public class Startup
    {
        public IConfiguration Configuration{get;}

        //Hàm khởi tạo
        public Startup(IConfiguration configuration) => Configuration = configuration;
        
        //Thường khai báo các services Dependency
        public void ConfigureServices(IServiceCollection services){
            
                //--- Đăng ký các dịch vụ MVC
            services.AddControllersWithViews();

                //--- Kết nối với Mysql
            var serverMysqlVersion = new MySqlServerVersion(new Version(9,0,1));
            services.AddDbContext<ApplicationDbContext>(option =>{
                option.UseMySql( Configuration.GetConnectionString("MySQL") , serverMysqlVersion);
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>{
                c.SwaggerDoc("v1", new OpenApiInfo {Title="BauCuTrucTuyen", Version="v1"});

                //Cấu hình swagger để hỗ trợ xác thực jwt. Với định nghĩa bảo mật có tên là "Bearer"
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
                    Description=" JWT Authorization header using the Bearer scheme", //Thông tin mô tả
                    Name="Authorization",
                    In = ParameterLocation.Header,          //Chỉ định thông tin xác thực là truyền vào trong Header
                    Type = SecuritySchemeType.ApiKey,       //Loại bảo mật này là Apikey
                    Scheme = "Bearer"                       //scheme được sử dụng là bearer
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement(){
                    {   //Thêm yêu cầu bảo mật
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference{
                                Type = ReferenceType.SecurityScheme, 
                                Id = "Bearer"               //Chỉ định xác thực bằng JWT bằng cách sử dụng định nghĩa bảo mật có ID là Bearer
                            },
                            Scheme = "oauth2",              //Chỉ định loại scheme là oauth2
                            Name = "Bearer",                //Thông tin xác thực sẽ truyền trong header có tên là Bearer
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

                //Cấu hình xử lý IFormFile
                c.OperationFilter<FileUploadService>();
            });

                // --- Cấu hình để xử lý Multipart/form
            //Câu hình dịch vụ Controller trong ứng dụng
            services.AddControllers(options =>{
                options.EnableEndpointRouting = false;      //Tắt tính năng EndpointRouting tiếp cận cũ hơn dựa trên Route Template
            })
            .AddNewtonsoftJson()                        //Sử dụng thư viện NewtonsoftJson để serialize và deserialize Json
            .ConfigureApiBehaviorOptions(options => {   //Cấu hình hành vi cho api
                options.SuppressConsumesConstraintForFormFileParameters = true;     //Tắt kiểm tra định dạng media cho các tham số là FormFile
                options.SuppressInferBindingSourcesForParameters = true;            //Ngăn API xác định nguồn dữ liệu trong request
                options.SuppressModelStateInvalidFilter = true;                     //Vô hiệu quá bộ lọc trạng thái không hợp lệ
            });

                // ---- Cấu hình Cloudinary
            services.AddSingleton<CloudinaryService>();

            services.AddMemoryCache();

                // ---- Đăng ký AutoMapper
            services.AddControllersWithViews();
            services.AddAutoMapper(typeof(MappingProfile));

                // --- Xác thực JWT
            var accessToken = Configuration["AppSettings:AccessToken"];
            var accessTokenBytes = Encoding.UTF8.GetBytes(accessToken);

            //services.AddRazorPages();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>{
                opt.TokenValidationParameters = new TokenValidationParameters{
                    //Tự cấp token
                    ValidateIssuer = false,
                    ValidateAudience = false,

                    //Ký vào token
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(accessTokenBytes),
                    ClockSkew = TimeSpan.Zero
                };
            });
            
                //-- Đăng ký dịch vụ tại đây
            services.AddSingleton<IAppConfig,AppConfig>();
            services.AddScoped<DatabaseContext>();
            services.AddScoped<IRoleRepository,RoleReposistory>();
            services.AddScoped<IProvinceRepository,ProvinceReposistory>();
            services.AddScoped<IDistrictRepository,DistrictReposistory>();
            services.AddScoped<IVoterRepository, VoterReposistory>();
            services.AddScoped<ICandidateRepository, CandidateRepository>();
            services.AddScoped<IConstituencyRepository,ConstituencyReposistory>();
            services.AddScoped<IBoardRepository,BoardReposistory>();
            services.AddScoped<INotificationRepository,NotificationsReposistory>();
            services.AddScoped<IEducationLevelRepository,EducationLevelReposistory>();
            services.AddScoped<IElectionsRepository,ElectionsReposistory>();
            services.AddScoped<IEthnicityRepository,EthnicityReposistory>();
            services.AddScoped<IPositionsRepository,PositionReposistory>();
            services.AddScoped<IVoteRepository,VoteReposistory>();
            services.AddScoped<IListOfPositionRepository,ListOfPositionReposistory>();
            services.AddScoped<IUserRepository,UserRepository>();
            services.AddScoped<ICadreRepository,CadreRepository>();
            services.AddScoped<IWorkPlaceRepository,WorkPlaceReposistory>();
            services.AddScoped<IProfileRepository,ProfileRepository>();
            
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
