
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;

namespace BackEnd.src.core.Interfaces
{
    public interface IVotingServices
    {
        Task<int> _VoterVote(VoterVoteDTO voterVoteDTO);
         Task<int> _CadreVote(CadreVoteDTO cadreVoteDTO);
        Task<int> _CandidateVote(CandidateVoteDTO candidateVoteDTO);
        Task<bool> _AddElectionDetailsBy_IDcutri(ElectionDetailsDTO electionDetailsDTO, MySqlConnection connection);
        Task<bool> _UpdateStatusElectionsBy_IDcutri(string id_cutri, string ngayBD, MySqlConnection connection);
    }
}