using BackEnd.core.Entities;
using BackEnd.src.web_api.DTOs;


namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IEducationLevelRepository
    {
        Task<List<EducationLevel>> _GetListOfEducationLevel();
        Task<bool> _AddEducationLevel(EducationLevel trinhdohocvan);
        Task<EducationLevel> _GetEducationLevelBy_ID(string id);
        Task<bool> _EditEducationLevelBy_ID(string ID, EducationLevel EducationLevel);
        Task<bool> _DeleteEducationLevelBy_ID(string ID);
    }
}