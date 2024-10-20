
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface ILockRepository
    {
        //Lấy thông tin khóa công khai
        Task<LockDTO> _getLockBasedOnElectionDate(string ngayBD);
        Task<LockDTO> _getLockBasedOnElectionDate(string ngayBD, MySqlConnection connection);
        //2.Lấy tất cả thông tin khóa công khai
        Task<List<LockDTO>> _getListKey();
        //3.Xóa khóa theo ngày bắt đầu
        Task<bool> _deleteKeyBasedOnElectionDate(string ngayBD);
        //4.Lấy khóa thông tin khóa mật theo ngày bắt đầu
        Task<PrivateKeyDTO> _getPrivateKeyBasedOnElectionDateAndKey(string ngayBD);
        //5. Lấy danh sách giá trị phiếu đã giải mã theo thời điểm bầu cử
        Task<List<VoteDto>> _ListOfDecodedVotesBasedOnElection(string ngayBD);
    }
}