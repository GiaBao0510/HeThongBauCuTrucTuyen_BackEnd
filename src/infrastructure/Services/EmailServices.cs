using System.Net;
using System.Net.Mail;
using BackEnd.src.core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BackEnd.src.infrastructure.Services
{
    public class EmailServices: IEmailSender
    {
        private readonly string _email;
        private readonly string _pwd;
        private readonly string _host;
        private readonly int _port;
        private readonly SmtpClient _smtpClient;

        //Hàm khởi tạo
        public EmailServices(IConfiguration configuration){
            var emailSettings = configuration.GetSection("EmailSettings");
            _email = emailSettings["Mail"] ?? throw new ArgumentException("Email configuration is missing");
            _pwd = emailSettings["Password"] ?? throw new ArgumentException("Email password configuration is missing");
            _host = emailSettings["Host"] ?? throw new ArgumentException("SMTP host configuration is missing");
            _port = int.TryParse(emailSettings["Port"], out int port) ? port : throw new ArgumentException("SMTP port configuration is missing");
            _smtpClient = new SmtpClient(_host) {
                Port = _port,                                       //Thiết lập cổng mà smtpserver sử dụng
                Credentials = new NetworkCredential(_email,_pwd),   //Thiết lập xác thực
                EnableSsl =true                                     //Bật Ssl để mã hóa thông tin khi gửi đi ,giúp tăng cường bảo mật
            };
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage){
            
            //Kiểm tra xem tham số đầu vào là bắt buộc. Nếu không điền thì quăng ra ngoại lệ yêu cầu rằng tham số không được phép rỗng
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(htmlMessage) )
                throw new ArgumentNullException("Email, subject, and message are required");

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_email),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            //Thực hiển gửi email
            _ = Task.Run(async() =>{
                try{
                    await _smtpClient.SendMailAsync(mailMessage).ConfigureAwait(false);
                }catch (Exception ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
            });

        }
    }
}