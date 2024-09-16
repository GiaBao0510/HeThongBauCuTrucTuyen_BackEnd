
using BackEnd.src.core.Models;
using BackEnd.src.web_api.DTOs;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IUserServices
    {
        //0.Đăng nhập
        Task<LoginModel> _Login(LoginModel loginDto);
        //1.Quên mật khẩu
        //2.Đăng ký tạo tài khoản
    }
}