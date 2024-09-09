using BackEnd.core.Entities;
using BackEnd.src.web_api.DTOs;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IListOfPositionRepository
    {
        Task<List<ListOfPositions>> _GetListOfListOfPositions();
        Task<bool> _AddListOfPositions(ListOfPositions danhmucungcu);
        Task<ListOfPositions> _GetListOfPositionsBy_ID(string id);
        Task<bool> _EditListOfPositionsBy_ID(string ID, ListOfPositions ListOfPositions);
        Task<bool> _DeleteListOfPositionsBy_ID(string ID);
    }
}