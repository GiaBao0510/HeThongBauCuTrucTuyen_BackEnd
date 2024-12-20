
using BackEnd.src.core.Models;
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IUserServices
    {
        //0.Đăng nhập
        Task<LoginModel> _Login(LoginModel loginDto);//
        //1.Gửi mã otp qua email khi quên mật khẩu
        Task<bool> _SendOtpCodeWhenForgotPwd(EmailDTO emailDTO);//
        //2.Đăng ký tạo tài khoản

        //3.Gửi lại mã otp xác nhận
        Task<bool> _ResendOtpAsync(EmailDTO emailDTO);//
        //4. Gửi mã OTP khi đăng nhập
        Task _SendVerificationOTPcodeAfterLogin(string email);//
        //5.Xác nhận mã otp sau khi đăng nhập
        Task<TokenModel> _VerifyOtpCodeAfterLogin(VerifyOtpDto verifyOtpDto);//
        //6. Xác thực mã OTP
        Task<int> _VerifyOtp(VerifyOtpDto verifyOtpDto);//
        //7. Gửi mã OTP cùng với tiêu đề
        Task<bool> _SendOtpCodeWithTitle(string email, string title);//
        //8. Kiểm tra Email có tồn tại không
        Task<bool> _CheckEmailExists(string email, MySqlConnection connection);//
        //9. Đặt lại mật khẩu dựa trên email người dùng
        Task<bool> _ResetUserPassword(string email, string newPwd);//
        //10.Gửi mã OTP khi người dùng gửi bỏ phiếu
        Task<bool> _SendOtpCodeAfterUserVoted(EmailDTO emailDTO, MySqlConnection connection);
        //11. Kiểm tra xem Email người dùng có tồn tại trong hệ thống không
        Task<bool> _CheckUserEmail(string email);
        Task<int> _handleUserRegister(string sdt, string newPwd);
    }
}