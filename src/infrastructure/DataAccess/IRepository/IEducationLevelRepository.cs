using BackEnd.core.Entities;
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;


namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IEducationLevelRepository
    {
        Task<List<EducationLevel>> _GetListOfEducationLevel();
        Task<bool> _AddEducationLevel(EducationLevel trinhdohocvan);
        Task<EducationLevel> _GetEducationLevelBy_ID(string id);
        Task<bool> _EditEducationLevelBy_ID(string ID, EducationLevel EducationLevel);
        Task<bool> _DeleteEducationLevelBy_ID(string ID);
        //Kiểm tra trình độ học vấn có tồn tại không
        Task<bool> _IsEducationLevelIDExist(int ID_TrinhDo, MySqlConnection connection);
        //Gắn ứng cử viên vào trình độ học vấn
        Task<bool> _AddEducationQualificationsToCandidates(int ID_TrinhDo, string ID_UCV, MySqlConnection connection);
        

    }
}