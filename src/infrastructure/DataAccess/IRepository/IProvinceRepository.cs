using BackEnd.core.Entities;
using BackEnd.src.web_api.DTOs;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IProvinceRepository
    {
        Task<List<ProvinceDTO>> _GetListOfProvice();
        Task<bool> _AddProvince(ProvinceDTO tinhthanh);
        Task<Province> _GetProvinceBy_ID(string id);
        Task<bool> _EditProvinceBy_ID(string ID, ProvinceDTO province);
        Task<bool> _DeleteProvinceBy_ID(string ID);
    }
}