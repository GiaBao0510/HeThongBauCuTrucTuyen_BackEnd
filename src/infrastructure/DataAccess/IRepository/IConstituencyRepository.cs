using BackEnd.core.Entities;
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IConstituencyRepository
    {
        Task<List<ConstituencyDto>> _GetListOfConstituency();
        Task<bool> _AddConstituency(Constituency donvibaucu);
        Task<bool> _EditConstituencyBy_ID(string ID, ConstituencyDto Constituency);
        Task<Constituency> _GetConstituencyBy_ID(string id);
        Task<bool> _DeleteConstituencyBy_ID(string ID);
        Task<bool> _CheckIfConstituencyExists(string ID, MySqlConnection connection);
        Task<bool> _CheckVoterID_ConsituencyID_andPollingDateTogether(string ID_DonViBauCu, string IDcutri, DateTime ngayBD, MySqlConnection connection);
        //Kiểm tra ID của đơn vị bầu, mã cán bộ và ngày bắt đầu có cùng tồn tại không
        Task<bool> _CheckCadreID_ConsituencyID_andPollingDateTogether(string ID_DonViBauCu, string ID_CanBo, DateTime ngayBD, MySqlConnection connection);
        //Kiểm tra ID của đơn vị bầu, mã ững củ viên và ngày bắt đầu có cùng tồn tại không
        Task<bool> _CheckCandidateID_ConsituencyID_andPollingDateTogether(string ID_DonViBauCu, string ID_ucv, DateTime ngayBD, MySqlConnection connection);
    
    }
}