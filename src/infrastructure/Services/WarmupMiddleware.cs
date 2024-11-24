using System.Threading.Tasks;
using BackEnd.src.core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BackEnd.src.infrastructure.Services
{
    public class WarmupMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        private bool _isWarmedUp = false;
        private readonly object _lock = new object();

        //khởi tạo
        public WarmupMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync (HttpContext context){
            if(!_isWarmedUp){
                lock(_lock){
                    if(!_isWarmedUp){
                        using(var scope = _serviceProvider.CreateScope()){
                            var warmupService = scope.ServiceProvider.GetRequiredService<IWarmupService>();
                            warmupService.WarmupAsync().Wait();
                        }

                        _isWarmedUp = true;
                    }
                }
            }
            await _next(context);
        }
    }
}