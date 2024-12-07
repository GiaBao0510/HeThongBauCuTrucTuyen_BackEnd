using System.Diagnostics;
using BackEnd.src.core.Common;
using BackEnd.src.core.Interfaces;
using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;
using System.Globalization;
using log4net;
using System.Numerics;

namespace BackEnd.src.infrastructure.Services
{
    public class VotingServices :IDisposable, IVotingServices
    {
        private readonly IVoterRepository _voterRepository;
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program)); 
        private readonly IElectionsRepository _electionsRepository;
        private readonly DatabaseContext _context;
        private readonly IVoteRepository _voteRepository;
        private readonly IPaillierServices _paillierServices;
        private readonly IListOfPositionRepository _listOfPositionRepository;
        private readonly IConstituencyRepository _constituencyRepository;
        private readonly ILockRepository _lockRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly ICadreRepository _cadreRepository;

        public VotingServices(
            DatabaseContext context,
            IVoterRepository voterRepository, 
            IElectionsRepository electionsRepository,
            IPaillierServices paillierServices,
            IConstituencyRepository constituencyRepository,
            IVoteRepository voteRepository,  //Thêm
            IListOfPositionRepository listOfPositionRepository,
            ILockRepository lockRepository,
            ICandidateRepository candidateRepository,
            ICadreRepository cadreRepository
        )
        {
            _voterRepository = voterRepository;
            _electionsRepository = electionsRepository;
            _context = context;
            _paillierServices = paillierServices;
            _constituencyRepository = constituencyRepository;
            _voteRepository = voteRepository;  //Thêm
            _listOfPositionRepository = listOfPositionRepository;
            _lockRepository = lockRepository;
            _candidateRepository = candidateRepository;
            _cadreRepository = cadreRepository;
        }

        //Hủy
        public new void Dispose() => _context.Dispose();

        //17.0 Kiêm tra xem thời điểm bỏ phiếu hợp lệ không. Nếu không thì trả về
        // if(DateTime.Now > timeOfTheElectionDTO.ngayKT || DateTime.Now < votingDay){
        //    return 0;
        // } 

        // Loại người bỏ phiếu
        private enum VoterType
        {
            Voter,
            Candidate,
            Cadre
        }

        // Phương thức bỏ phiếu chung
        private async Task<int> ProcessVoteAsync(
            string ngayBD,
            string giaTriPhieuBauStr,
            int idCap,
            int idDonViBauCu,
            string idNguoiBau,
            VoterType voterType)
        {
            //Bắt đầu đo thời gian bỏ phiếu
            Stopwatch stopwatch = Stopwatch.StartNew();

            using var connect = await _context.Get_MySqlConnection();
            if (connect.State != System.Data.ConnectionState.Open)
                await connect.OpenAsync();

            using var transaction = await connect.BeginTransactionAsync();

            try
            {
                //_log.Info(" --- Bắt đầu quá trình bỏ phiếu --- ");

                // Chuyển đổi dữ liệu
                if (!DateTime.TryParseExact(ngayBD, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime votingDay))
                {
                    _log.Error("Định dạng ngày bắt đầu không hợp lệ.");
                    return -10;
                }

                var electionInfo = await _electionsRepository._GetTimeOfElection(ngayBD, connect);
                if (electionInfo == null)
                {
                    _log.Error("Không tìm thấy thông tin bầu cử.");
                    return -10;
                }

                // _log.Info("Thông tin đầu vào:");
                // _log.Info($"Ngày bầu cử: {votingDay}");
                // _log.Info($"ID Người bầu: {idNguoiBau}");
                // _log.Info($"ID Đơn vị bầu cử: {idDonViBauCu}");
                // _log.Info($"ID Cấp: {idCap}");
                // _log.Info($"Giá trị phiếu bầu: {giaTriPhieuBauStr}");

                if (!BigInteger.TryParse(giaTriPhieuBauStr, out BigInteger giaTriPhieuBau))
                {
                    _log.Error("Giá trị phiếu bầu không hợp lệ.");
                    return -4;
                }

                // Kiểm tra người bầu có tồn tại
                bool isVoterExists = voterType switch
                {
                    VoterType.Voter => await _voterRepository._CheckVoterExists(idNguoiBau, connect),
                    VoterType.Candidate => await _candidateRepository._CheckCandidateExists(idNguoiBau, connect),
                    VoterType.Cadre => await _cadreRepository._CheckCadreExists(idNguoiBau, connect),
                    _ => false
                };

                if (!isVoterExists)
                {
                    _log.Error("Người bầu không tồn tại.");
                    return -1;
                }

                // Kiểm tra người bầu đã bỏ phiếu chưa
                bool hasVoted = voterType switch
                {
                    VoterType.Voter => await _voterRepository._CheckVoterHasVoted(idNguoiBau, votingDay, connect),
                    VoterType.Candidate => await _candidateRepository._CheckCandidateHasVoted(idNguoiBau, votingDay, connect),
                    VoterType.Cadre => await _cadreRepository._CheckCadreHasVoted(idNguoiBau, votingDay, connect),
                    _ => false
                };

                if (hasVoted)
                {
                    _log.Error("Người bầu đã bỏ phiếu.");
                    return -3;
                }

                // Kiểm tra giá trị phiếu bầu hợp lệ
                int maxVotes = await _electionsRepository._MaximumNumberOfVotes(votingDay, connect);
                int maxVoters = await _electionsRepository._MaximumNumberOfVoters(votingDay, connect);

                BigInteger maxVoteValue = _paillierServices.GiaTriToiDaCuaPhieuBau_M(maxVoters + 1, maxVotes);

                if (giaTriPhieuBau > maxVoteValue)
                {
                    _log.Error("Giá trị phiếu bầu vượt quá giới hạn.");
                    return -4;
                }

                // Kiểm tra danh mục ứng cử
                bool isValidListOfPosition = await _listOfPositionRepository._CheckTheListOgCandidatesWithTheVotingDateTogether(votingDay, idCap, connect);
                if (!isValidListOfPosition)
                {
                    _log.Error("Danh mục ứng cử không hợp lệ.");
                    return -5;
                }

                // Kiểm tra đơn vị bầu cử
                bool isValidConstituency = voterType switch{
                    VoterType.Voter => await _constituencyRepository._CheckVoterID_ConsituencyID_andPollingDateTogether(idDonViBauCu.ToString(), idNguoiBau, votingDay, connect),
                    VoterType.Candidate => await _constituencyRepository._CheckCandidateID_ConsituencyID_andPollingDateTogether(idDonViBauCu.ToString(), idNguoiBau, votingDay, connect),
                    VoterType.Cadre => await _constituencyRepository._CheckCadreID_ConsituencyID_andPollingDateTogether(idDonViBauCu.ToString(), idNguoiBau, votingDay, connect),
                    _ => false 
                };
                if (!isValidConstituency){ 
                    _log.Error("Đơn vị bầu cử không hợp lệ.");
                    return -6;
                }

                // Tạo phiếu bầu
                string idPhieu = GenerateVoteId();
                var lockInfo = await _lockRepository._getLockBasedOnElectionDate(ngayBD, connect);
                BigInteger encryptedVoteValue = _paillierServices.Encryption(lockInfo.G, lockInfo.N, giaTriPhieuBau);

                var voteDto = new VoteDto
                {
                    ngayBD = ngayBD,
                    ID_cap = idCap,
                    GiaTriPhieuBau = encryptedVoteValue
                };

                bool isVoteAdded = await _voteRepository._AddVote(idPhieu, voteDto, connect);
                if (!isVoteAdded)
                {
                    _log.Error("Thêm phiếu bầu thất bại.");
                    await transaction.RollbackAsync();
                    return -7;
                }

                // Cập nhật trạng thái bỏ phiếu
                bool isStatusUpdated = voterType switch
                {
                    VoterType.Voter => await _UpdateStatusElectionsBy_IDcutri(idNguoiBau, ngayBD, connect),
                    VoterType.Candidate => await _UpdateStatusElectionsBy_IDucv(idNguoiBau, ngayBD, connect),
                    VoterType.Cadre => await _UpdateStatusElectionsBy_IDcanbo(idNguoiBau, ngayBD, connect),
                    _ => false
                };

                if (!isStatusUpdated)
                {
                    _log.Error("Cập nhật trạng thái bầu cử thất bại.");
                    await transaction.RollbackAsync();
                    return -8;
                }

                // Thêm chi tiết phiếu bầu
                var electionDetails = new ElectionDetailsDTO
                {
                    ID_CuTri = voterType == VoterType.Voter ? idNguoiBau : null,
                    ID_ucv = voterType == VoterType.Candidate ? idNguoiBau : null,
                    ID_CanBo = voterType == VoterType.Cadre ? idNguoiBau : null,
                    ID_Phieu = idPhieu,
                    ThoiDiem = DateTime.Now
                };

                bool isElectionDetailAdded = voterType switch
                {
                    VoterType.Voter => await _AddElectionDetailsBy_IDcutri(electionDetails, connect),
                    VoterType.Candidate => await _AddElectionDetailsBy_IDucv(electionDetails, connect),
                    VoterType.Cadre => await _AddElectionDetailsBy_IDcanbo(electionDetails, connect),
                    _ => false
                };

                if (!isElectionDetailAdded)
                {
                    _log.Error("Thêm chi tiết phiếu bầu thất bại.");
                    await transaction.RollbackAsync();
                    return -9;
                }

                await transaction.CommitAsync();
                _log.Info("Bỏ phiếu thành công.");

                //Kết thúc đo thời gian bỏ phiếu
                stopwatch.Stop();
                _log.Info($"Thời gian bỏ phiếu: {stopwatch.ElapsedMilliseconds} ms");

                return 1;
            }
            catch (MySqlException ex)
            {
                _log.Error($"Lỗi MySQL: {ex.Message}", ex);
                await transaction.RollbackAsync();
                return -100;
            }
            catch (Exception ex)
            {
                _log.Error($"Lỗi hệ thống: {ex.Message}", ex);
                await transaction.RollbackAsync();
                return -100;
            }
        }

        // Phương thức bỏ phiếu cho cử tri
        public async Task<int> _VoterVote(VoterVoteDTO voterVoteDTO)
        {
            _log.Info(" >>> Bắt đầu cửu tri bỏ phiếu");
            return await ProcessVoteAsync(
                voterVoteDTO.ngayBD,
                voterVoteDTO.GiaTriPhieuBau,
                voterVoteDTO.ID_Cap,
                voterVoteDTO.ID_DonViBauCu,
                voterVoteDTO.ID_CuTri,
                VoterType.Voter);
        }

        // Phương thức bỏ phiếu cho ứng cử viên
        public async Task<int> _CandidateVote(CandidateVoteDTO candidateVoteDTO)
        {
            _log.Info(" >>> Bắt đầu ứng cử viên bỏ phiếu");
            return await ProcessVoteAsync(
                candidateVoteDTO.ngayBD,
                candidateVoteDTO.GiaTriPhieuBau,
                candidateVoteDTO.ID_Cap,
                candidateVoteDTO.ID_DonViBauCu,
                candidateVoteDTO.ID_ucv,
                VoterType.Candidate);
        }

        // Phương thức bỏ phiếu cho cán bộ
        public async Task<int> _CadreVote(CadreVoteDTO cadreVoteDTO)
        {
            _log.Info(" >>> Bắt đầu cán bộ bỏ phiếu");
            return await ProcessVoteAsync(
                cadreVoteDTO.ngayBD,
                cadreVoteDTO.GiaTriPhieuBau,
                cadreVoteDTO.ID_Cap,
                cadreVoteDTO.ID_DonViBauCu,
                cadreVoteDTO.ID_CanBo,
                VoterType.Cadre);
        }

        // Tạo ID phiếu ngẫu nhiên
        private string GenerateVoteId()
        {
            string randomString = RandomString.ChuoiNgauNhien(2);
            DateTime currentDay = DateTime.Now;
            return $"{randomString}{currentDay:yyyyMMddHHmmssff}";
        }

        //Cập nhật chi tiết phiếu bầu cử tri
        public async Task<bool> _AddElectionDetailsBy_IDcutri(ElectionDetailsDTO electionDetailsDTO, MySqlConnection connection){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
            
            try{
                const string sql = @"
                INSERT INTO chitietbaucu(ID_CuTri,ID_Phieu,ThoiDiem) 
                VALUES(@ID_CuTri,@ID_Phieu,@ThoiDiem);";
                using (var command = new MySqlCommand(sql,connection)){
                    command.Parameters.AddWithValue("@ID_CuTri", electionDetailsDTO.ID_CuTri);
                    command.Parameters.AddWithValue("@ID_Phieu", electionDetailsDTO.ID_Phieu);
                    command.Parameters.AddWithValue("@ThoiDiem", DateTime.Now);
                    await command.ExecuteNonQueryAsync();
                }

                return true;
            }catch(MySqlException ex){
                _log.Info($"Error message: {ex.Message}");
                _log.Info($"Error Code: {ex.Code}");
                _log.Info($"Error Source: {ex.Source}");
                _log.Info($"Error HResult: {ex.HResult}");
                throw;
            }
            catch(Exception ex){
                _log.Info($"Error message: {ex.Message}");
                _log.Info($"Error Source: {ex.Source}");
                _log.Info($"Error StackTrace: {ex.StackTrace}");
                _log.Info($"Error TargetSite: {ex.TargetSite}");
                _log.Info($"Error HResult: {ex.HResult}");
                _log.Info($"Error InnerException: {ex.InnerException}");
                throw;
            }
        }

        //Cập nhật chi tiết phiếu bầu ứng cử viên
        public async Task<bool> _AddElectionDetailsBy_IDucv(ElectionDetailsDTO electionDetailsDTO, MySqlConnection connection){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
            
            try{
                const string sql = @"
                INSERT INTO chitietbaucu(ID_ucv,ID_Phieu,ThoiDiem) 
                VALUES(@ID_ucv,@ID_Phieu,@ThoiDiem);";
                using (var command = new MySqlCommand(sql,connection)){
                    command.Parameters.AddWithValue("@ID_ucv", electionDetailsDTO.ID_ucv);
                    command.Parameters.AddWithValue("@ID_Phieu", electionDetailsDTO.ID_Phieu);
                    command.Parameters.AddWithValue("@ThoiDiem", DateTime.Now);
                    await command.ExecuteNonQueryAsync();
                }

                return true;
            }catch(MySqlException ex){
                _log.Info($"Error message: {ex.Message}");
                _log.Info($"Error Code: {ex.Code}");
                _log.Info($"Error Source: {ex.Source}");
                _log.Info($"Error HResult: {ex.HResult}");
                throw;
            }
            catch(Exception ex){
                _log.Info($"Error message: {ex.Message}");
                _log.Info($"Error Source: {ex.Source}");
                _log.Info($"Error StackTrace: {ex.StackTrace}");
                _log.Info($"Error TargetSite: {ex.TargetSite}");
                _log.Info($"Error HResult: {ex.HResult}");
                _log.Info($"Error InnerException: {ex.InnerException}");
                throw;
            }
        }

        //Cập nhật chi tiết phiếu bầu cán bộ
        public async Task<bool> _AddElectionDetailsBy_IDcanbo(ElectionDetailsDTO electionDetailsDTO, MySqlConnection connection){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
            
            try{
                const string sql = @"
                INSERT INTO chitietbaucu(ID_CanBo,ID_Phieu,ThoiDiem) 
                VALUES(@ID_CanBo,@ID_Phieu,@ThoiDiem);";
                using (var command = new MySqlCommand(sql,connection)){
                    command.Parameters.AddWithValue("@ID_CanBo", electionDetailsDTO.ID_CanBo);
                    command.Parameters.AddWithValue("@ID_Phieu", electionDetailsDTO.ID_Phieu);
                    command.Parameters.AddWithValue("@ThoiDiem", DateTime.Now);
                    await command.ExecuteNonQueryAsync();
                }

                return true;
            }catch(MySqlException ex){
                _log.Info($"Error message: {ex.Message}");
                _log.Info($"Error Code: {ex.Code}");
                _log.Info($"Error Source: {ex.Source}");
                _log.Info($"Error HResult: {ex.HResult}");
                throw;
            }
            catch(Exception ex){
                _log.Info($"Error message: {ex.Message}");
                _log.Info($"Error Source: {ex.Source}");
                _log.Info($"Error StackTrace: {ex.StackTrace}");
                _log.Info($"Error TargetSite: {ex.TargetSite}");
                _log.Info($"Error HResult: {ex.HResult}");
                _log.Info($"Error InnerException: {ex.InnerException}");
                throw;
            }
        }

        //Cập nhật lại trạng thái bầu cử dựa trên mã cử tri
        public async Task<bool> _UpdateStatusElectionsBy_IDcutri(string id_cutri, string ngayBD,MySqlConnection connection){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
            
            try{
                const string sql = @"
                UPDATE trangthaibaucu 
                SET GhiNhan = '1' 
                WHERE ID_CuTri = @ID_CuTri And ngayBD = @ngayBD;";

                using (var command = new MySqlCommand(sql,connection)){
                    command.Parameters.AddWithValue("@ID_CuTri", id_cutri);
                    command.Parameters.AddWithValue("@ngayBD", ngayBD);
                    await command.ExecuteNonQueryAsync();
                }

                return true;
            }catch(MySqlException ex){
                _log.Error($"Error message: {ex.Message}");
                _log.Error($"Error Code: {ex.Code}");
                _log.Error($"Error Source: {ex.Source}");
                _log.Error($"Error HResult: {ex.HResult}");
                throw;
            }
            catch(Exception ex){
                _log.Error($"Error message: {ex.Message}");
                _log.Error($"Error Source: {ex.Source}");
                _log.Error($"Error StackTrace: {ex.StackTrace}");
                _log.Error($"Error TargetSite: {ex.TargetSite}");
                _log.Error($"Error HResult: {ex.HResult}");
                _log.Error($"Error InnerException: {ex.InnerException}");
                throw;
            }
        }

        //Cập nhật lại trạng thái bầu cử dựa trên mã ứng cử viên
        public async Task<bool> _UpdateStatusElectionsBy_IDucv(string ID_ucv, string ngayBD,MySqlConnection connection){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
            
            try{
                const string sql = @"
                UPDATE trangthaibaucu 
                SET GhiNhan = '1' 
                WHERE ID_ucv = @ID_ucv And ngayBD = @ngayBD;";

                using (var command = new MySqlCommand(sql,connection)){
                    command.Parameters.AddWithValue("@ID_ucv", ID_ucv);
                    command.Parameters.AddWithValue("@ngayBD", ngayBD);
                    await command.ExecuteNonQueryAsync();
                }
                _log.Info("Đã cập nhật trạng thái bầu cử ứng cử viên");

                return true;
            }catch(MySqlException ex){
                _log.Error($"Error message: {ex.Message}");
                _log.Error($"Error Code: {ex.Code}");
                _log.Error($"Error Source: {ex.Source}");
                _log.Error($"Error HResult: {ex.HResult}");
                throw;
            }
            catch(Exception ex){
                _log.Error($"Error message: {ex.Message}");
                _log.Error($"Error Source: {ex.Source}");
                _log.Error($"Error StackTrace: {ex.StackTrace}");
                _log.Error($"Error TargetSite: {ex.TargetSite}");
                _log.Error($"Error HResult: {ex.HResult}");
                _log.Error($"Error InnerException: {ex.InnerException}");
                throw;
            }
        }

        //Cập nhật lại trạng thái bầu cử dựa trên mã cán bộ
        public async Task<bool> _UpdateStatusElectionsBy_IDcanbo(string ID_CanBo, string ngayBD,MySqlConnection connection){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
            
            try{
                const string sql = @"
                UPDATE trangthaibaucu 
                SET GhiNhan = '1' 
                WHERE ID_CanBo = @ID_CanBo And ngayBD = @ngayBD;";

                using (var command = new MySqlCommand(sql,connection)){
                    command.Parameters.AddWithValue("@ID_CanBo", ID_CanBo);
                    command.Parameters.AddWithValue("@ngayBD", ngayBD);
                    await command.ExecuteNonQueryAsync();
                }
                _log.Info("Đã cập nhật trạng thái bầu cử ứng cử viên");

                return true;
            }catch(MySqlException ex){
                _log.Error($"Error message: {ex.Message}");
                _log.Error($"Error Code: {ex.Code}");
                _log.Error($"Error Source: {ex.Source}");
                _log.Error($"Error HResult: {ex.HResult}");
                throw;
            }
            catch(Exception ex){
                _log.Error($"Error message: {ex.Message}");
                _log.Error($"Error Source: {ex.Source}");
                _log.Error($"Error StackTrace: {ex.StackTrace}");
                _log.Error($"Error TargetSite: {ex.TargetSite}");
                _log.Error($"Error HResult: {ex.HResult}");
                _log.Error($"Error InnerException: {ex.InnerException}");
                throw;
            }
        }

    }
}