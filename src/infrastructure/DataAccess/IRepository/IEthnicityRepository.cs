using BackEnd.core.Entities;
using BackEnd.src.core.Entities;
using BackEnd.src.web_api.DTOs;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IEthnicityRepository
    {
        Task<List<Ethnicity>> _GetListOfEthnicity();
        Task<bool> _AddEthnicity(Ethnicity dantoc);
        Task<Ethnicity> _GetEthnicityBy_ID(string id);
        Task<bool> _EditEthnicityBy_ID(string ID, Ethnicity Ethnicity);
        Task<bool> _DeleteEthnicityBy_ID(string ID);
    }
}