using BackEnd.core.Entities;
using BackEnd.src.web_api.DTOs;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface INotificationRepository
    {
        Task<List<Notifications>> _GetListOfNotifications();
        Task<bool> _AddNotifications(Notifications thongbao);
        Task<Notifications> _GetNotificationsBy_ID(string id);
        Task<bool> _EditNotificationsBy_ID(string ID, Notifications Notifications);
        Task<bool> _DeleteNotificationsBy_ID(string ID);
    }
}