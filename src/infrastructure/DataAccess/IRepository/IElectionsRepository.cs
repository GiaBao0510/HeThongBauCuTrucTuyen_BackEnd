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
    }
}