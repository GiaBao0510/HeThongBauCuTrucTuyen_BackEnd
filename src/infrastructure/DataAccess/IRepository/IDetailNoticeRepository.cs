
using BackEnd.src.web_api.DTOs;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IDetailNoticeRepository
    {
        //1. Lấy danh sách thông báo cho cử tri
        Task<List<ViewNotificationBasedOnObjectsDTO>> _getVoterNotificationList(string ID_CuTri);
        //2. Lấy danh sách thông báo cho cán bộ
        Task<List<ViewNotificationBasedOnObjectsDTO>> _getCadreNotificationList(string ID_CanBo);
        //3. Lấy danh sách thông báo cho ứng cử viên
        Task<List<ViewNotificationBasedOnObjectsDTO>> _getCandidateNotificationList(string ID_ucv);
    }
}