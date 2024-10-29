using BackEnd.src.web_api.DTOs;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IVoterRepository
    {
        //Thêm
        Task<int> _AddVoter(VoterDto voter, IFormFile fileAnh);
        //Sửa
        Task<int> _EditVoterBy_ID(string IDvoter ,VoterDto voter);
        //Xóa
        Task<bool> _DeleteVoterBy_ID(string IDvoter);
        //Lấy thông tin theo ID cử tri
        Task<VoterDto> _GetVoterBy_ID(string IDvoter);
        //Lấy ALL
        Task<List<VoterDto>> _GetListOfVoter();
        //Đặt mật khẩu cử tri - admin
        Task<bool> _SetVoterPassword(string id,string newPwd);
        //Thay đổi mật khẩu - cử tri
        Task<int> _ChangeVoterPassword(string id, string oldPwd, string newPwd);
        //Lấy ID người dùng dựa trên ID cử tri
        Task<string> GetIDUserBaseOnIDCuTri(string id, MySqlConnection connection);
        //Lấy thông tin cử tri kèm theo tài khoản
        Task<List<VoterDto>> _GetListOfVotersAndAccounts();
        //Kiểm tra ID cử tri có tồn tại không
        Task<bool> _CheckVoterExists(string ID, MySqlConnection connection);
        //Cử tri phản hồi
        Task<bool> _VoterSubmitReport(SendReportDto reportDto);
        //Cử tri đăng ký khi quét mã(Hiển thị thông tin đăng ký của cử tri và cử tri phải điền mật khẩu)
        Task<VoterDto> _DisplayUserInformationAfterScanningTheCode(string ID);
        //Kiểm tra xem tài khoản cử tri đã đăng ký hay chưa
        Task<int> _CheckRegisteredVoter(string ID, MySqlConnection connection); 
        //Cử tri đặt mật khẩu, CCCD khi đăng ký
        Task<bool> _SetVoterCCCD_SetVoterPwd(string id, string newCCCD, string pwd);
        //Cử tri đảm nhận chức vụ gì
        Task<int> _VoterTakePosition(string ID_voter, int ID_ChucVu, MySqlConnection connection, MySqlTransaction transaction);
        //Thay đổi chức vụ của cử tri
        Task<bool> _ChangeOfVoterPosition(string ID_voter, int ID_ChucVu);
        //Thêm danh sách cử tri vào cuộc bầu cử (Cử tri có thể tham gia vào những cuộc bầu cử nào)
        Task<int> _AddListVotersToTheElection(VoterListInElectionDto voterListInElectionDto);
        //Kiểm tra xem cử tri có tồn tại trong kỳ bầu cử không
        Task<bool> _VoterCheckInElection(string ID_cutri, DateTime ngayBD,  MySqlConnection connection);
        //Danh sách các kỳ bầu cử mà cử tri có thể tham gia
        //19. Kiểm tra xem cử tri đã bỏ phiếu trong kỳ bầu cử chưa
        Task<bool> _CheckVoterHasVoted(string ID_cutri, DateTime ngayBD, MySqlConnection connect);
        //Cử tri muốn xác thực lại phiếu đã bỏ trong thời gian bỏ, nhưng không
        Task<List<ElectionsDto>> _ListElectionsVotersHavePaticipated(string ID_cutri);
        //Lấy danh sách ID cử tri theo ngày bầu cử
        Task<List<VoterID_DTO>> _getVoterID_ListBasedOnElection(string ngayBD, MySqlConnection connect);
    }
}