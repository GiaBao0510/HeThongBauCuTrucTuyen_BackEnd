using BackEnd.src.infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BackEnd.src.infrastructure.Services
{
    public class AotomaticNotificationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval  = TimeSpan.FromHours(24);
        private readonly IHubContext<NotificationHubs> _hubContext;

        public AotomaticNotificationService(
            IServiceProvider serviceProvider,
            IHubContext<NotificationHubs> hubContext
        ){
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var notificationHub = scope.ServiceProvider.GetRequiredService<NotificationHubs>();

                    //Gọi các phương thức gửi thông báo
                    await notificationHub._announceUpcomingElectionDayToVoter();
                    await notificationHub._announceUpcomingElectionDayToCandidate();
                    await notificationHub._announceUpcomingElectionDayToCandre();
                    
                    await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Server", "Automatic Notification Service is running...");
                }
                await Task.Delay(_interval, stoppingToken);
            }
        }
        
    }
}