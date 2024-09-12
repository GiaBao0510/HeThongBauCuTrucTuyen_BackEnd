
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IProfileRepository
    {
        //Lấy ID_user dựa trên hồ sơ người dùng
        Task<string> _GetIDUserByProfile(string MaSo);
        //kiểm tra xem hồ sơ của người dùng đã đăng ký chưa
        Task<bool> _CheckRegistedUserProfile(string ID_user);
        //Kiểm tra xem hồ sơ người dùng có tồn tại không
        Task<bool> _CheckProfileExists(string ID_user, MySqlConnection connection);
        //Thêm hồ sơ người dùng
        Task<bool> _AddProfile(string ID_user, string Status, MySqlConnection connection);
        //Cập nhật hố sơ người dùng - mã người dùng
        Task<bool> _UpdateProfileBy_IdUser(ProfileDto profileDto, string userID);
        //Cập nhật hố sơ người dùng - mã hồ sơ
        Task<bool> _UpdateProfileBy_ProfileCode(ProfileDto profileDto, string ProfileCode);
        //Lấy danh sách người dùng đã đăng ký
        Task<List<ProfileDto>> _GetListRegisteredProfiles();
        //Lấy danh sách người dùng chưa đăng ký
        Task<List<ProfileDto>> _GetListUnregisteredProfiles();
        //Lấy danh sách hồ sơ
        Task<List<ProfileDto>> _GetListProfiles();
        //Lấy danh sách cử tri đã đăng ký tài khoản
        Task<List<ProfileDto>> _GetListRegisteredVoter();
        //Lấy danh sách cử tri chưa đăng ký tài khoản
        Task<List<ProfileDto>> _GetListUnregisteredVoter();
        //Nếu cử tri quét mã thì sẽ tự động cập nhật trạng thái cử tri là đã đăng ký
        Task<bool> _AutomaticallyUpdateUserStatusIfRegistered(string ID_cutri);
        //Xóa hồ sơ dựa trên ID_người dùng
        Task<bool> _DeleteProfileBy_Id_user(string ID_user);
        //Xóa hồ sơ dựa trên Mã hồ sơ
        Task<bool> _DeleteProfileBy_ProfileCode(string ProfileCode);
        //Xóa hồ sơ dựa trên ID_cử tri
        Task<bool> _DeleteProfileBy_ID_cutri(string ID_cutri);
        //Xóa hồ sơ dựa trên ID_Cán bộ
        Task<bool> _DeleteProfileBy_ID_canbo(string ID_canbo);
        //Xóa hồ sơ dựa trên ID_ứng cử viên
        Task<bool> _DeleteProfileBy_ID_ungcuvien(string ID_ungcuvien);
    }
}