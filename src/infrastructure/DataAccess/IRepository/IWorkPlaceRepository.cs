using BackEnd.src.core.Entities;
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IWorkPlaceRepository
    {
        //Kiểm tra xem Mã cán bộ đã tồn tại
        Task<bool> _CheckIfCadreCodeExists(string ID_CanBo, MySqlConnection connection);
        //Kiểm tra xem Mã chức vụ đã tồn tại
        Task<bool> _CheckIfPositionCodeExists(int ID_ChucVu, MySqlConnection connection);
        //Kiểm tra xem Mã ban đã tồn tại
        Task<bool> _CheckIfBoardCodeExists(int ID_ban, MySqlConnection connection);
        //Kiểm tra tổng quát về mã tồn tại
        Task<int> _CheckIfTheCodeExistsAtWork(WorkPlace workPlaceDto,MySqlConnection connection);
        //Thêm dữ liệu
        Task<int> _AddDataToTheWorkplace(WorkPlace workPlaceDto);
        //Cập nhật dữ liệu nơi công tác- mã cán bộ
        Task<bool> _UpdateWorkplaceBy_IDcadre(WorkPlaceDto workPlaceDto);
        //Cập nhật dữ liệu nơi công tác theo mã chức vụ
        Task<bool> _UpdateWorkplaceBy_IDposition(WorkPlaceDto workPlaceDto);
        //Cập nhật dữ liệu nơi công tác theo mã ban
        Task<bool> _UpdateWorkplaceBy_IDboard(WorkPlaceDto workPlaceDto);
        //Xóa dữ liệu nơi công tác theo mã cán bộ
        Task<bool> _DeleteWorkplaceBy_IDcadre(string ID_canBo);
        //Xóa dữ liệu nơi công tác theo mã chức vụ
        Task<bool> _DeleteWorkplaceBy_IDposition(int ID_chucvu);
        //Xóa dữ liệu nơi công tác theo mã ban
        Task<bool> _DeleteWorkplaceBy_IDboard(int ID_ban);
        //Liệt kê các ID nơi công tác
        Task<List<WorkPlaceDto>> _GetWorkplaces();
        //Liệt kê thông tin công tác, nhuwg chi tiết hơn
        Task<List<WorkPlaceDto>> _GetWorkplacesDetail();
        //Đặt lại chức vụ cho cán bộ - ID cán bộ
        Task<bool> _UpdatePositionBy_IDcadre(string ID_canBo, int ID_chucvu);
        //Đặt lại ban mà cán bộ đã làm việc theo -ID cán bộ
        Task<bool> _UpdateBoardBy_IDcadre(string ID_canBo, int ID_ban);
        //12. Kiểm tra cán bộ đã tham dự kỳ bầu cử chưa
        Task<bool> _CheckTheCadresWhoAttendedTheElection(string ID_CanBo, DateTime ngayBD ,MySqlConnection connection);
    }
}