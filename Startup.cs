using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BackEnd.infrastructure.config;
using BackEnd.src.core.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.infrastructure.DataAccess.Repositories;
using BackEnd.src.infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.infrastructure.Services;
using BackEnd.src.web_api.Mappings;
using BackEnd.src.core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BackEnd
{
    public class Startup
    {
        public IConfiguration Configuration{get;}

        //Hàm khởi tạo
        public Startup(IConfiguration configuration) => Configuration = configuration;
        
        //Thường khai báo các services Dependency

        public void ConfigureServices(IServiceCollection services){
            
            // --- ??
            //services.AddIdentity<ApplicationU>

                //--- 1. Đăng ký các dịch vụ MVC
            services.AddControllersWithViews();

                //--- 2. Kết nối với Mysql
            var serverMysqlVersion = new MySqlServerVersion(new Version(9,0,1));
            services.AddDbContext<ApplicationDbContext>(option =>{
                option.UseMySql( Configuration.GetConnectionString("MySQL") , serverMysqlVersion);
            });

            services.AddControllers();

                // --- 3. Cấu hình với Appsetting để lấy SecreteKey
            services.Configure<AppSetting>(Configuration.GetSection("AppSettings"));

                // --- 4. Cấu hình khi test trên Swagger
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

                // --- 5. Cấu hình để xử lý Multipart/form
            //Câu hình dịch vụ Controller trong ứng dụng  
            services.AddControllers(options => options.EnableEndpointRouting = false)   //Tắt tính năng EndpointRouting tiếp cận cũ hơn dựa trên Route Template
            .AddNewtonsoftJson()                        //Sử dụng thư viện NewtonsoftJson để serialize và deserialize Json
            .ConfigureApiBehaviorOptions(options => {   //Cấu hình hành vi cho api
                options.SuppressConsumesConstraintForFormFileParameters = true;     //Tắt kiểm tra định dạng media cho các tham số là FormFile
                options.SuppressInferBindingSourcesForParameters = true;            //Ngăn API xác định nguồn dữ liệu trong request
                options.SuppressModelStateInvalidFilter = true;                     //Vô hiệu quá bộ lọc trạng thái không hợp lệ
            });

                // ---- 6. Cấu hình Cloudinary
            services.AddSingleton<CloudinaryService>();

            //     // ---- 7. Thiết lập giá trị cơ chế xác thực mặc định trên Cookie
            // services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //     .AddCookie("SecurityBauCuTrucTuyen", options =>{
            //         options.AccessDeniedPath = new PathString("/");         //Đặt đường dẫn khi truy cập bị từ chối
            //         options.Cookie = new CookieBuilder{                     // -- Cấu hình chi tiết cho cookie
            //             //Tên miền
            //             HttpOnly = true,                                    //Chỉ truy cập cookie thông qua http ,chứ không thông qua JS
            //             Name = ".aspNetCoreDemo.Security.Cookie",           //Tên của Cookie
            //             Path = "/",                                         //Cookie có hiệu lực cho toàn bộ trang
            //             SameSite = SameSiteMode.Lax,                        //Cấu hình bảo mật SameSite
            //             SecurePolicy = CookieSecurePolicy.SameAsRequest     //Cookie sẽ gửi qua Https nếu yêu cầu là Https
            //         };
            //         options.Events = new CookieAuthenticationEvents{        // -- cấu hình sự kiện xác thực
            //             OnSignedIn = context =>{                            //Ghi log nếu người dùng đăng nhập
            //                 Console.WriteLine($"{DateTime.Now} OnSignedIn, {context.Principal.Identity.Name}");
            //                 return Task.CompletedTask;
            //             },
            //             OnSigningOut = context =>{                          //Ghi log nếu người dùng đăng xuất
            //                 Console.WriteLine($"{DateTime.Now} OnSigningOut, {context.HttpContext.User.Identity.Name}");
            //                 return Task.CompletedTask;
            //             },
            //             OnValidatePrincipal = context => {                  //Ghi log xác thực người dùng
            //                 Console.WriteLine($"{DateTime.Now} OnValidatePrincipal, {context.Principal.Identity.Name}");
            //                 return Task.CompletedTask;
            //             }
            //         }; 
            //         options.LoginPath = new PathString("/api/Account/login");   //Đặt đường dẫn trang đăng nhập
            //         options.ReturnUrlParameter = "RequestPath";                 //Đặt tham số url chuyển hướng sau khi đăng nhập
            //         options.SlidingExpiration = true;                           //Cho phép gia hạn thời gian của cookie mỗi khi người dùng tương tác
            // });

            services.AddMemoryCache();

                // ---- 8. Đăng ký AutoMapper
            services.AddControllersWithViews();
            services.AddAutoMapper(typeof(MappingProfile));

                // --- 9. Đăng ký các dịch vụ Identity vào hệ thống
            // services.AddIdentity<LoginModel, IdentityRole>()
            //     .AddEntityFrameworkStores<ApplicationDbContext>()
            //     .AddDefaultTokenProviders();
            //  services.AddRazorPages();

                // ---- 10. Xác thực JWT
            /*
                Lấy khóa bí mật. Mục đích để ký vào các jwt để xác thực khi có người
                gửi yêu cầu.
            */
            var SecretKey = Configuration["AppSettings:SecretKey"];
            var SecretKeyBytes = Encoding.UTF8.GetBytes(SecretKey);     //Chuyển khóa bí mật thành mảng byte

            /*
                    Đăng ký dịch vụ xác thực cho ứng dụng .Để ứng dụng có thể dùng JwtBear,
                có nghĩa là xác thực người dùng trên Bearer Tokens(Jwt tokens).
                    JwtBearerDefaults.AuthenticationScheme: tên loại xác thực được sử dụng để
                phân biệt với các loại xác thực khác
            */
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>{     //Cấu hình chi tiết cho việc xác thực
                opt.TokenValidationParameters = new TokenValidationParameters{
                    //Tự cấp token
                    ValidateIssuer = false,     //Không kiểm tra nguoiwg phát hành token
                    ValidateAudience = false,   //Không kiểm tra đối tượng nhận token
                    ValidateLifetime = true,    //Kiểm tra thời hạn của token. Nếu hết hạn thì bị từ chối
                    IssuerSigningKey = new SymmetricSecurityKey(    //Khóa bí mật được sử dụng ở dạng byte sẽ được sử dụng để xác thực token, đây cũng là khóa sử dụng để ký xác thực
                        System.Text.Encoding.UTF8.GetBytes(Configuration["JWT:SigningKey"])
                    ),
                    ValidIssuer = Configuration["JWT:Issuer"],
                    ValidAudience = Configuration["JWT:Audience"],

                    //Ký vào token
                    ValidateIssuerSigningKey = true,                                //kiểm tra khóa bí mật đã được sử dụng ký vào token, đảm bảo việc token không bị thay đổi
                    ClockSkew = TimeSpan.Zero,                                      //Đặt độ lệch thời gian xác thực token là 0. 
                    RoleClaimType = ClaimTypes.Role
                };
                opt.Events = new JwtBearerEvents{
                    OnAuthenticationFailed = context => {
                        Console.WriteLine($"AuthenticationFailed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated =  context => {
                        Console.WriteLine($"OnTokenValidated: {context.SecurityToken}");
                        return Task.CompletedTask;
                    },
                };      
            });

                // --- 12 Thiết lập Endpoint cho các thiết bị khác sử dụng
            services.AddCors(options => options.AddDefaultPolicy(policy => 
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
            ));

                // --- 11. Thiết lập thay đổi câu hình mặc định của Identity
            // services.Configure<IdentityOptions>(options =>{
            //     //11.1 thiết lập về Password
            //     options.Password.RequireDigit = false; //Không bắt buộc phải có số
            //     options.Password.RequireLowercase = false; //Không bắt buộc phải có chữ thường
            //     options.Password.RequireUppercase = false; //Không bắt buộc phải có chữ in
            //     options.Password.RequireNonAlphanumeric = false; //Không bắt buộc có ký tự đặc biệt
            //     options.Password.RequiredLength = 6; //Bắt buộc phải có ít nhất 6 ký tự
            //     options.Password.RequiredUniqueChars = 1; //Không bắt buộc phải có ký tự đặc biệt duy nhất

            //     //11.2 Cấu hình LockOut - khóa người dùng
            //     options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //Thời lượng khóa 5 phút
            //     options.Lockout.MaxFailedAccessAttempts = 8; //Quy định số lần đăng nhập thất bại. Nếu trên 8 lần thì khóa
            //     options.Lockout.AllowedForNewUsers = true; //Cho phép khóa người dùng mới tạo

            //     //11.3 Cấu hình về User
            //     options.User.AllowedUserNameCharacters = //Các ký tự đặt tên user
            //         "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            //     options.User.RequireUniqueEmail = true; //Không cho phép người dùng có địa chỉ email trùng lặp
                
            //     //11.4 Cấu hình đăng nhập
            //     options.SignIn.RequireConfirmedPhoneNumber = false; //Khong cho xác thực bằng số điện thoại
            //     options.SignIn.RequireConfirmedPhoneNumber = true; //Cấu hình chỉ xác thực bằng Email
            // });
            
                //-- Đăng ký dịch vụ tại đây
            services.AddScoped<DatabaseContext>();
            services.AddSingleton<IAppConfig,AppConfig>();
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
            services.AddScoped<IUserServices,UserServices>();
            
        } 

        //Riêng các service muốn call thì sẽ goi trong đây
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env){
            
            //Cấu hình pipline của ứng dụng
            if(env.EnvironmentName == "Development"){
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
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("BauCuTrucTuyen");
            app.UseAuthentication();            //Phục hồi thông tin đăng nhập(xác thực)
            app.UseAuthorization();             //Phục hồi thông tin về quyền của User
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                //endpoints.MapRazorPages();
            });
        }

        // Cấu hình dịch vụ - đăng ký dịch vụ
        public static void _ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IAppConfig, AppConfig>();
        }
    }
}