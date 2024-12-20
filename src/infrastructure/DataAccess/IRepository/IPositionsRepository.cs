using BackEnd.core.Entities;
using BackEnd.src.core.Entities;
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IPositionsRepository
    {
        Task<List<Position>> _GetListOfPosition();
        Task<bool> _AddPosition(Position chucvu);
        Task<Position> _GetPositionBy_ID(string id);
        Task<bool> _EditPositionBy_ID(string ID, Position Position);
        Task<bool> _DeletePositionBy_ID(string ID);
        //Kiểm tra mã chức vụ xem có tồn tại chưa
        Task<bool> _CheckIfTheCodeIsInThePosition(int ID_ChucVu, MySqlConnection connection);
    }
}