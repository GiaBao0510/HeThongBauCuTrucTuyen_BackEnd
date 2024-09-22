using BackEnd.core.Entities;
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IElectionsRepository
    {
        Task<List<ElectionDto>> _GetListOfElections();
        Task<bool> _AddElections(Elections kybaucu);
        Task<ElectionDto> _GetElectionsBy_ID(string id);
        Task<bool> _EditElectionsBy_ID(string ID, Elections Elections);
        Task<bool> _DeleteElectionsBy_ID(string ID);
        //Kiểm tra xem ngày bầu cử có tồn tại không
        Task<bool> _CheckIfElectionTimeExists(DateTime ngayBD, MySqlConnection connection);
        //Lấy số lượng cử tri tối đa theo kỳ bầu cử
        Task<int> _MaximumNumberOfVoters(DateTime ngayBD, MySqlConnection connection);
        //Lấy số lượng ứng cử viên tối đa theo kỳ bầu cử
        Task<int> _MaximumNumberOfCandidates(DateTime ngayBD, MySqlConnection connection);
        //Lấy số lượt bình chọn tối đa theo kỳ bầu cử
        Task<int> _MaximumNumberOfVotes(DateTime ngayBD, MySqlConnection connection);
        //Lấy số lượng cử tri hiện tại đang có trong kỳ bầu cử
        Task<int> _GetCurrentVoterCountByElection(DateTime ngayBD, MySqlConnection connection);
        //Lấy số lượng ứng cử viên hiện tại đang có trong kỳ bầu cử
        Task<int> _GetCurrentCandidateCountByElection(DateTime ngayBD, MySqlConnection connection);
    }
}