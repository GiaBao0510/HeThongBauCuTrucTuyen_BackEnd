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
        Task<List<CandidateInfoDTO>> _GetListOfCandidate();
        //Đặt mật khẩu ứng cử viên - admin
        Task<bool> _SetCandidatePassword(string id,string newPwd);
        //Thay đổi mật khẩu - ứng cử viên
        Task<int> _ChangeCandidatePassword(string id, string oldPwd, string newPwd);
        //Lấy ID người dùng dựa trên ID ứng cử viên
        Task<string> GetIDUserBaseOnIDUngCuVien(string id,  MySqlConnection connection);
        //Lấy thông tin ứng cử viên kèm theo tài khoản
        Task<List<CandidateDto>> _GetListOfCandidatesAndAccounts();
        //Kiểm tra ứng cử viên tồn tại
        Task<bool> _CheckCandidateExists(string ID, MySqlConnection connection);
        //Ứng cử viên phản hồi
        Task<bool> _CandidateSubmitReport(SendReportDto reportDto);
        //Kiểm tra thông tin đầu vào trước khi lưu vào bảng Kết quả 
        Task<int> _CheckInformationBeforeEnterInTableElectionResults(CandidateDto Candidate, MySqlConnection connection);
        Task<bool> _CheckCandidatForA_ParicularElection(string ID_cutri, DateTime ngayBD ,MySqlConnection connection);
         //11.Thêm danh sách ứng cử viên vào cuộc bầu cử
        Task<int> _AddListCandidatesToTheElection(CandidateListInElectionDto CandidateListInElectionDto);
        //12. Xóa ứng cử viên khỏi kỳ bầu cử cụ thể
        Task<int> _RemoveCandidateOfElection(string Id_ucv, DateTime ngayBD);
        //Lấy danh sách và thông tin ứng cử viên dựa trên kỳ bầu cử
        Task<List<ListCandidateOnElectionDateDTO>> _GetCandidateListBasedOnElectionDate(DateTime ngayBD);
        //14. Lấy danh sách các kỳ bầu cử mà ứng cử viên đã tham gia
        Task<List<CandidateRegistedForElectionsDTO>> _getListOfRegisteredCandidate(string ID_ucv);
        //Lấy danh sách ID ứng cử viên theo ngày bầu cử
        Task<List<CandidateID_DTO>> _getCandidateID_ListBasedOnElection(string ngayBD, MySqlConnection connect);
        //Kiểm tra xem cán bộ đã bỏ phiếu hay chưa
        Task<bool> _CheckCandidateHasVoted(string ID_ucv, DateTime ngayBD, MySqlConnection connect);
    }
}