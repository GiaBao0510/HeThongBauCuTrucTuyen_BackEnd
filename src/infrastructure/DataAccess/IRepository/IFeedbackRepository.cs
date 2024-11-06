
using BackEnd.src.web_api.DTOs;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IFeedbackRepository
    {
        //Lấy Thông tin phản hồi cư tri
        Task<List<FeedbackDTO>> _getVoterFeedbackList();
        //Lấy Thông tin phản hồi cán bộ
        Task<List<FeedbackDTO>> _getCadreFeedbackList();
        //Lấy Thông tin phản hồi ứng cử viên
        Task<List<FeedbackDTO>> _getCandidateFeedbackList();
    }
}