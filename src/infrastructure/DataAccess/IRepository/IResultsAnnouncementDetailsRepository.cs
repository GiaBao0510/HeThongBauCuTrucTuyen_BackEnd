using MySql.Data.MySqlClient;
using BackEnd.src.web_api.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IResultsAnnouncementDetailsRepository
    {
        //1. Thêm thông tin cán bộ công bố kết quả
        Task<bool> _CadrePublicizeResult(string ID_CanBo, string ID_ucv, string ngayBD, MySqlConnection connect);
    }
}