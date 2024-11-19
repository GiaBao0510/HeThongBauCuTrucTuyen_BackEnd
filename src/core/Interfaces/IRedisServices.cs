
namespace BackEnd.src.core.Interfaces
{
    public interface IRedisServices
    {
        Task<bool> Set<T>(string key, T value, TimeSpan? expiry = null);
        Task<T> Get<T>(string key);
        Task<bool> Delete(string key);
        Task<bool> KeyExists(string key);
    }
}