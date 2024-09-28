

using BackEnd.src.core.Models;

namespace BackEnd.src.core.Interfaces
{
    public interface IToken
    {
        public Task<TokenModel> GenerateToken(LoginModel loginModel);
        public int _CheckLogined(string token);
        public Task<TokenModel> _RenewToken(LoginModel loginModel);
    }
}