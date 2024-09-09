using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.core.Entities;
using BackEnd.src.web_api.DTOs;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IDistrictRepository
    {
        Task<List<District>> _GetListOfDistrict();
        Task<bool> _AddDistrict(DistrictDto quanhuyen);
        Task<District> _GetDistrictBy_ID(string id);
        Task<List<District>> _GetListOfDistrictBy_STT(string stt);
        Task<bool> _EditDistrictBy_ID(string ID, DistrictDto District);
        Task<bool> _DeleteDistrictBy_ID(string ID);
    }
}