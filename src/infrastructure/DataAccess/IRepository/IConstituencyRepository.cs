using BackEnd.core.Entities;
using BackEnd.src.web_api.DTOs;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IConstituencyRepository
    {
        Task<List<ConstituencyDto>> _GetListOfConstituency();
        Task<bool> _AddConstituency(Constituency donvibaucu);
        Task<bool> _EditConstituencyBy_ID(string ID, Constituency Constituency);
        Task<Constituency> _GetConstituencyBy_ID(string id);
        Task<bool> _DeleteConstituencyBy_ID(string ID);
    }
}