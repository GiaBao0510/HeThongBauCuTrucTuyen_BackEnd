
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IElectionResultsRepository
    {
        //1. Lấy danh sách kết quả bầu cử đã công bố cho đối tượng
        Task<List<ElectionResultDetailsDTO>> _getDetailedListOfElectionResult(string BangChiTiet, string BangDoiTuong,string index, string value ,MySqlConnection connection);
        //2. Lấy bảng chi tiết kết quả bầu cử đã công bố cho cử tri
        Task<List<ElectionResultDetailsDTO>> _getDetailedListOfElectionResultForVoter(string ID_CuTri);
        //3. Lấy bảng chi tiết kết quả bầu cử đã công bố cho ứng cử viên
        Task<List<ElectionResultDetailsDTO>> _getDetailedListOfElectionResultForCandidate(string ID_ucv);
        //4. Lấy bảng chi tiết kết quả bầu cử đã công bố cho cán bộ
        Task<List<ElectionResultDetailsDTO>> _getDetailedListOfElectionResultForCandre(string ID_canbo);
        
    }
}