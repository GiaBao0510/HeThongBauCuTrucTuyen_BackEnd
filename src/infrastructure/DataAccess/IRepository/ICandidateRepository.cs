using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.src.web_api.DTOs;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface ICandidateRepository
    {
        //Thêm
        Task<int> _AddCandidate(CandidateDto Candidate, IFormFile fileAnh);
        //Sửa
        Task<int> _EditCandidateBy_ID(string IDCandidate ,CandidateDto Candidate);
        //Xóa
        Task<bool> _DeleteCandidateBy_ID(string IDCandidate);
        //Lấy thông tin theo ID ứng cử viên
        Task<CandidateDto> _GetCandidateBy_ID(string IDCandidate);
        //Lấy ALL
        Task<List<CandidateDto>> _GetListOfCandidate();
        //Đặt mật khẩu ứng cử viên - admin
        Task<bool> _SetCandidatePassword(string id,string newPwd);
        //Thay đổi mật khẩu - ứng cử viên
        Task<int> _ChangeCandidatePassword(string id, string oldPwd, string newPwd);
        //Lấy ID người dùng dựa trên ID ứng cử viên
        Task<string> GetIDUserBaseOnIDCuTri(string id,  MySqlConnection connection);
        //Lấy thông tin ứng cử viên kèm theo tài khoản
        Task<List<CandidateDto>> _GetListOfCandidatesAndAccounts();
        
    }
}