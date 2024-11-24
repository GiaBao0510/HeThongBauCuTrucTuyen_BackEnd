using BackEnd.src.core.Interfaces;
using BackEnd.src.infrastructure.Data;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class WarmupService : IWarmupService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        
        //Khoi tao
        public WarmupService(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
        }

        //Phuong thuc khoi tao du lieu
        public async Task WarmupAsync(){
            // Làm thức DBContext
            await _dbContext.Database.OpenConnectionAsync();
            await _dbContext.Database.CloseConnectionAsync();

            // Warm up các service quan trọng
            using(var scope = _serviceProvider.CreateScope()){
                var userService = scope.ServiceProvider.GetRequiredService<IUserServices>();
                var tokenService = scope.ServiceProvider.GetRequiredService<IToken>();
                
            }

        }
    }
}