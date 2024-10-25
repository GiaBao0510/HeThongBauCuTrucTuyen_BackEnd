using BackEnd.core.Entities;
using BackEnd.src.web_api.DTOs;
using BackEnd.src.core.Entities;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IBoardRepository
    {
       Task<List<BoardDto>> _GetListOfBoard();              //Liệt kê
       Task<bool> _AddBoard(Board ban);                     //Thêm
       Task<Board> _GetBoardBy_ID(string id);               //Lấy theo ID
       Task<bool> _EditBoardBy_ID(string ID, Board Board);  //Sửa theo ID
       Task<bool>_DeleteBoardBy_ID(string ID);     
       Task<Board> _GetBoardBy_ID(string id,  MySqlConnection connection);         //Xóa theo ID
        //Kiểm tra mã chức vụ xem có tồn tại chưa
        Task<bool> _CheckIfTheCodeIsInTheBoard(int ID_Ban, MySqlConnection connection);
    }
}