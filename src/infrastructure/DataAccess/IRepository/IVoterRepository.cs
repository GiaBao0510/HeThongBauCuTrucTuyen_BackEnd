using BackEnd.core.Entities;
using BackEnd.src.core.Entities;
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
        Task<string> GetIDUserBaseOnIDCuTri(string id,  MySqlConnection connection);
        //Lấy thông tin cử tri kèm theo tài khoản
        Task<List<VoterDto>> _GetListOfVotersAndAccounts();
        //Cử tri phản hồi
        //Cử tri bỏ phiếu
        //Cử tri muốn xác thực lại phiếu đã bỏ trong thời gian bỏ, nhưng không
    }
}