using BackEnd.src.core.Entities;
using BackEnd.src.web_api.DTOs;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IUserRepository
    {
        Task<List<Users>> _GetListOfUsers(); //Liệt kê all
        //Task<List<UserDto>> _GetListOfUserAndAccount(); //Liệt kê danh sách người dùng và tài khoản
        Task<List<Users>> _GetListOfUsersWithRole(int roleID);   //Lấy người dùng theo Role
        Task<int> _AddUser(UserDto user, IFormFile fileAnh );
        Task<object> _AddUserWithConnect(UserDto user ,IFormFile fileAnh ,MySqlConnection connection, MySqlTransaction transaction); //Thêm người dùng
        Task<Users> _GetUserBy_ID(string id);   //Lấy người dùng theo ID
        Task<int> _EditUserBy_ID_ForAdmin(string ID, UserDto user); //Sửa người dùng theo ID
        Task<bool> _DeleteUserBy_ID(string ID);   //Xóa người dùng theo ID
        Task<bool> _SetUserPassword(string id, string newPassword); //Đổi mật khẩu người dùng dự trên ID
        Task<int> _ChangeUserPassword(string id, string oldPwd, string newPwd);  //Người dùng đổi mật khẩu
        Task<int> CheckEmailAlreadyExits(string email, MySqlConnection connection);
        Task<int> CheckCitizenIdentificationAlreadyExits(string cccd, MySqlConnection connection);
        Task<int> CheckPhoneNumberAlreadyExits(string sdt, MySqlConnection connection);
        Task<bool> CheckRoleAlreadyExits(int role, MySqlConnection connection);
        Task<int> CheckForDuplicateUserInformation(int N,UserDto user, MySqlConnection connection);
        bool CheckUserInformationIsNotEmpty(UserDto user);
        Task<bool> _EditUserImageByID(string ID, IFormFile file);
        Task<bool> _CheckUserExists(string ID, MySqlConnection connection);
        Task<string> _GetUserProperties(string ID_user, string attribute, MySqlConnection connection);
        Task<bool> _DeleteUserBy_ID_withConnection(string ID, MySqlConnection connection);  //Xóa người dùng với connection
        Task<List<UserDto>> _GetListOfUsersAndAccounts(); //Lấy thông tin người dùng kèm theo tài khoản người dùng
        //Kiểm tra thông tin cử tri trước kh thêm vào
        bool _CheckVoterInformationIsNotEmpty(UserDto user);
        //Kiểm tra chức vụ có tồn tại không
        Task<bool> _CheckPositionExist(int roleID, MySqlConnection connection, MySqlTransaction transaction);
        Task<bool> _CheckPositionExist(int roleID, MySqlConnection connection);
        //Kiểm tra xem tài khoản người dùng đã đăng ký chưa
        Task<int> _CheckRegisteredUser(string ID);
        Task<int> _SetPwdBasedOnUserEmail(string Email, string password);
        //Lấy thông tin người dùng dựa trên Email
        Task<PersonalInformationDTO> _GetPersonnalInfomationByEmail(string email);
        //Lấy danh sách kỳ bầu cử mà người dùng có thể tham dự dựa trên sđt người dùng
        Task<List<ElectionsDto>> _getListOfElectionsByUserPhone(string sdt); 
        //Người dùng tự cập nhật thong tin cá nhân
        Task<int> _UpdatePersonalInfomation(PersonalInformationDTO personalInfo);
        //11. Xóa thông tin người dùng theo ID
        Task<bool> _DeleteUserBy_ID(string ID, MySqlConnection connection);
    }
}