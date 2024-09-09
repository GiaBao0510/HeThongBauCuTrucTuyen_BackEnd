using BackEnd.core.Entities;
using BackEnd.src.web_api.DTOs;
using BackEnd.src.core.Entities;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IBoardRepository
    {
       Task<List<BoardDto>> _GetListOfBoard();              //Liệt kê
       Task<bool> _AddBoard(Board ban);                     //Thêm
       Task<Board> _GetBoardBy_ID(string id);               //Lấy theo ID
       Task<bool> _EditBoardBy_ID(string ID, Board Board);  //Sửa theo ID
       Task<bool>_DeleteBoardBy_ID(string ID);              //Xóa theo ID

    }
}