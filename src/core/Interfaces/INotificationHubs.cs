
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;

namespace BackEnd.src.core.Interfaces
{
    public interface INotificationHubs
    {
        Task<int> _getLastestNoticeID(MySqlConnection connect);
        Task<bool> _createNotice(string content, MySqlConnection connect);
        public Task SaveNotificationDetail(string tableName, string userIdColumn, string userId, int noticeId, MySqlConnection connection);
        Task<List<VoterNoticeDetailsDTO>> _getElectionInformationToVoters(MySqlConnection connect);
        Task<List<CandidateNoticeDetailsDTO>> _getElectionInformationToCandidates(MySqlConnection connect);
        Task<List<CadreNoticeDetailsDTO>> _getElectionInformationToCandres(MySqlConnection connect);
        Task _announceUpcomingElectionDayToVoter();
        Task _announceUpcomingElectionDayToCandidate();
        Task _announceUpcomingElectionDayToCandre();
        Task _announceElectionResult(string ngayBD, MySqlConnection connect);
        //Xem thông báo dựa trên ID đối tượng
        Task<List<ViewNotificationBasedOnObjectsDTO>> NotificationViewer(string bangChiTiet, string ID_ObColumn, string ID_ob,  MySqlConnection connect);
    }
}