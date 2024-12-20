using BackEnd.core.Entities;
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IElectionsRepository
    {
        Task<List<ElectionDto>> _GetListOfElections();
        Task<int> _AddElections(ElectionTempDTO kybaucu);
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
        //So sánh số lượng cử tri hiện tại với số lượng cử tri tối đa trong kỳ bầu cử
        Task<int> _CompareCurrentNumberCandidateWithSpecifieldNumber(DateTime ngayBD, MySqlConnection connection, MySqlTransaction transaction);
        //Lấy danh sách ID và tên ứng cử viên được sắp xếp dựa trên ngày bắt đầu bầu cử
        Task<List<CandidateNamesBasedOnElectionDateDto>> _GetListCandidateNamesBasedOnElections(DateTime ngayBD);
        //Lầy ngày kết thúc đăng ký ứng cử dựa trên ngày bắt đầu bầu cử
        Task<DateTime?> _GetRegistrationClosingDate(DateTime ngayBD, MySqlConnection connection);
        //lấy danh sách các kỳ bầu cử trong tương lai
        Task<List<ElectionDto>> _GetListOfFutureElections(); 
        //Trả về ngày kết thúc của kỳ bầu cử dựa trên thời điểm bắt đầu
        Task<TimeOfTheElectionDTO> _GetTimeOfElection(string ngayBD, MySqlConnection connection);
        //Kiểm tra xem kỳ bầu cử đã công bố kết quả chưa
        Task<bool> _checkResultAnnouncement(string ngayBD,  MySqlConnection connection);
        Task<List<CandidateNamesBasedOnElectionDateDto>> _GetListCandidateNamesBasedOnElections_OtherID_ucv(DateTime ngayBD,MySqlConnection connection);
        //Cập nhật số lượt bình chọn, tỉ lệ bình chọn cho ID_ucv tương ứng dựa trên ngày bắt đầu
        Task<bool> _updateVoteCountAndVotePercentage(DateTime ngayBD, int SoLuotBinhChon, float tiLeBinhChon, string ID_ucv ,MySqlConnection connection);
        //Cập nhật đã công bố kết quả bầu cử rồi dựa trên kỳ bầu cử
        Task<bool> _UpdateResultAnnouncementElectionBasedOnElectionDate(DateTime ngayBD, MySqlConnection connection);
        //Lấy danh sách chi tiết các kỳ bầu cử
        Task<List<CadreJoinedForElectionDTO>> _getDetailsListOfElectionBassedOnYear(string year);
        //Lấy danh sách các cử tri chưa tham dự bầu cử
        Task<List<UserNotYetJoinedDTO>> _listOfVotersWhoHaveNotYetParticipatedElection(string ngayBD);
        //Lấy danh sách các ứng cử viên chưa tham dự bầu cử
        Task<List<UserNotYetJoinedDTO>> _listOfCandidatesWhoHaveNotYetParticipatedElection(string ngayBD);
        //Lấy danh sách các cán bộ chưa tham dự bầu cử
        Task<List<UserNotYetJoinedDTO>> _listOfCadresWhoHaveNotYetParticipatedElection(string ngayBD);
    }
}