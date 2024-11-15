
using BackEnd.src.web_api.DTOs;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface ICadreRepository
    {
        //Thêm
        Task<int> _AddCadre(CadreDto Cadre, IFormFile fileAnh);
        //Sửa
        Task<int> _EditCadreBy_ID(string IDCadre ,CadreDto Cadre);
        //Xóa
        Task<bool> _DeleteCadreBy_ID(string IDCadre);
        //Lấy thông tin theo ID cán bộ
        Task<CadreDto> _GetCadreBy_ID(string IDCadre);
        //Lấy ALL
        Task<List<CadreInfoDTO>> _GetListOfCadre();
        //Đặt mật khẩu cán bộ - admin
        Task<bool> _SetCadrePassword(string id,string newPwd);
        //Thay đổi mật khẩu - cán bộ
        Task<int> _ChangeCadrePassword(string id, string oldPwd, string newPwd);
        //Lấy ID người dùng dựa trên ID cán bộ
        Task<string> GetIDUserBaseOnIDCadre(string id, MySqlConnection connection);
        //Lấy thông tin cán bộ kèm theo tài khoản
        Task<List<CadreDto>> _GetListOfCadresAndAccounts();
        //Kiểm tra cán bộ tồn tại
        Task<bool> _CheckCadreExists(string ID, MySqlConnection connection);
        //cán bộ phản hồi
        Task<bool> _CadreSubmitReport(SendReportDto reportDto);
        //Kiểm tra xem ID cán bộ có tồn tại không
        Task<bool> _CheckIfTheCadreCodeExists(string ID_CanBo, MySqlConnection connection);
        //Tạo nơi hoạt động của cán bộ
        //10.thêm danh sách id cán bộ vào kỳ bầu cử
        Task<int>  _AddListCadresToTheElection(CadreListInElectionDto cadreListInElectionDto);
        //11.Thông tin các kỳ bầu cử mà cán bộ đã tham dự để trực
        Task<List<CadreJoinedForElectionDTO>> _getListOfCadreJoinedForElection(string ID_CanBo);
        //Lấy danh sách ID cán bộ theo ngày bầu cử
        Task<List<CadreID_DTO>> _getCadreID_ListBasedOnElection(string ngayBD, MySqlConnection connect);
    }
}