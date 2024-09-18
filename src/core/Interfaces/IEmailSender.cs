
namespace BackEnd.src.core.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}