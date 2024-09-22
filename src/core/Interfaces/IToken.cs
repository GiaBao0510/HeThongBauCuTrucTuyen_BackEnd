

using BackEnd.src.core.Models;

namespace BackEnd.src.core.Interfaces
{
    public interface IToken
    {
        public string GenerateRefreshToken();
        public Task<TokenModel> GenerateToken(LoginModel loginModel);
        public Task SaveOrUpdateRefreshToken(string jwtId, string refreshToken, string account);
        public Task InvalidateRefreshTokenForUser(string TaiKhoan);
        public int _CheckLogined(string token);
    }
}