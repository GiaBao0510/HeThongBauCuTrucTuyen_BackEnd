using BackEnd.src.web_api.DTOs;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IElectionResponse
    {
        Task<List<ElectionResponseDTO>> _getListOfResponseBasedOnElection(string ngayBD);
        Task _addResponseForElection(ElectionResponseDTO electionResponseDTO);
        Task _editResponseForElection(string NoiDung, string ThoiDiem);
        Task _deleteResponseForElection(string ThoiDiem);
    }
}