using BackEnd.core.Entities;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface ILoginHistoryRepository
    {
        //1. Lưu lịch sử đăng nhập của người dùng
        Task<bool> _SaveLoginHistory(string DiaChiIP, string taikhoan);
        //2. Lấy danh sách lịch sử đăng nhập
        Task<List<LoginHistory>> _GetLoginHistoryList();
        //3. Lấy danh sách lịch sử đăng nhập của cử tri
        Task<List<LoginHistory>> _GetListOfLoginHistory_byVote();
        //4. Lấy danh sách lịch sử đăng nhập của cán bộ
        Task<List<LoginHistory>> _GetListOfLoginHistory_byCadre();
        //5. Lấy danh sách lịch sử đăng nhập của ứng cử viên
        Task<List<LoginHistory>> _GetListOfLoginHistory_byCandidate();
    }
}