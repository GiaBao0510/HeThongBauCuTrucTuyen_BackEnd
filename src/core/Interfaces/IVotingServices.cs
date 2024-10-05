
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;

namespace BackEnd.src.core.Interfaces
{
    public interface IVotingServices
    {
        Task<int> _VoterVote(VoterVoteDTO voterVoteDTO);
        Task<bool> _AddElectionDetailsBy_IDcutri(ElectionDetailsDTO electionDetailsDTO, MySqlConnection connection);
        Task<bool> _UpdateStatusElectionsBy_IDcutri(string id_cutri, MySqlConnection connection);
    }
}