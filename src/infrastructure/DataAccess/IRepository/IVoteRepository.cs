using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IVoteRepository
    {
        Task<List<VoteDto>> _GetListOfVote();
        Task<bool> _AddVote(VoteDto phieubau, MySqlConnection connection);
        Task<VoteDto> _GetVoteBy_ID(string id);
        Task<List<VoteDto>> _GetVoteBy_Time(string time);
        Task<bool> _EditVoteBy_ID(string ID, VoteDto phieubau);
        Task<bool> _DeleteVoteBy_ID(string ID);
        Task<bool> _DeleteVoteBy_Time(string time);
        Task<bool>_CreateVoteByNumber(int number, VoteDto vote );
    }
}