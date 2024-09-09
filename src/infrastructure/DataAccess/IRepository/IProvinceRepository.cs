using BackEnd.core.Entities;
using BackEnd.src.web_api.DTOs;
using BackEnd.src.core.Entities;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IProvinceRepository
    {
        Task<List<Province>> _GetListOfProvice();
        Task<bool> _AddProvince(Province tinhthanh);
        Task<Province> _GetProvinceBy_ID(string id);
        Task<bool> _EditProvinceBy_ID(string ID, Province province);
        Task<bool> _DeleteProvinceBy_ID(string ID);
    }
}