using Microsoft.AspNetCore.Http;
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

        //17. Cử tri bỏ phiếu
        public async Task<int> _VoterVote(VoterVoteDTO voterVoteDTO){
            using var connect = await _context.Get_MySqlConnection();
            //Kiểm tra kết nối
            if(connect.State != System.Data.ConnectionState.Open )
                await connect.OpenAsync();
            
            using var transaction = await connect.BeginTransactionAsync();

            try{
                _log.Info(" --- Đến bên trong phần kiểm tra --- ");
                //Chuyển đổi dữ liệu
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime votingDay = DateTime.ParseExact(voterVoteDTO.ngayBD,"yyyy-MM-dd HH:mm:ss",provider);
                var timeOfTheElectionDTO = await _electionsRepository._GetTimeOfElection(voterVoteDTO.ngayBD, connect);
                //DateTime pollingStopTime = 

                _log.Info("\t\t --- Thông tin đầu vào --");
                _log.Info($"timeOfTheElectionDTO: {timeOfTheElectionDTO}");
                _log.Info($"Ngày BD: {votingDay}");
                _log.Info($"Ngày HIện tại: {DateTime.Now}");
                _log.Info($"ID_CuTri: {voterVoteDTO.ID_CuTri}");
                _log.Info($"ID_DonViBauCu: {voterVoteDTO.ID_DonViBauCu}");
                _log.Info($"ID_Cap: {voterVoteDTO.ID_Cap}");
                _log.Info($"GiaTriPhieuBau: {voterVoteDTO.GiaTriPhieuBau}");
                
                //17.-1 Kiếm ngày bắt đầu không tồn tại thì báo lỗi
                if(timeOfTheElectionDTO == null)
                    return -10;

                _log.Info($"Ngày KT: {timeOfTheElectionDTO.ngayKT}");
                _log.Info("\t\t --------------------------");

                //Chuyển đổi giá trị phiếu
                BigInteger GiaTriPhieuBauHienTai = BigInteger.Parse(voterVoteDTO.GiaTriPhieuBau);

                //17.0 Kiêm tra xem thời điểm bỏ phiếu hợp lệ không. Nếu không thì trả về
                // if(DateTime.Now > timeOfTheElectionDTO.ngayKT || DateTime.Now < votingDay){
                //    return 0;
                // } 

                //17.1 Kiểm tra xem cử tri có tồn tại không
                bool checkVoterExists = await _voterRepository._CheckVoterExists(voterVoteDTO.ID_CuTri, connect);
                if(!checkVoterExists)
                    return -1;

                //17.2 Kiểm tra xem thời điểm bắt đầu bầu cử có tồn tại không
                bool checkElectionExist = await _electionsRepository._CheckIfElectionTimeExists(votingDay, connect);
                if(!checkElectionExist){
                    _log.Info("Kiểm tra xem thời điểm bắt đầu bầu cử có tồn tại không");
                    return -2;
                } 
                
                //17.3 Kiểm tra xem cử tri đã bỏ phiếu chưa ,nếu rồi thì không cho bỏ hiếu
                bool checkVoterHasVoted = await _voterRepository._CheckVoterHasVoted(voterVoteDTO.ID_CuTri, votingDay, connect);
                if(checkVoterHasVoted)
                    return -3;

                //17.4 Kiểm tra giá trị phiếu bầu có hợp lệ không
                int SoLuotBinhChonToiDa = await _electionsRepository._MaximumNumberOfVotes(votingDay, connect);
                int SoLuongToiDaCuTri = await _electionsRepository._MaximumNumberOfVoters(votingDay, connect);
                _log.Info($"SoLuotBinhChonToiDa(S): {SoLuotBinhChonToiDa}");
                _log.Info($"SoLuongToiDaCuTri(N): {SoLuongToiDaCuTri}");
                
                BigInteger GiaTriPhieuLonNhat = _paillierServices.GiaTriToiDaCuaPhieuBau_M(SoLuongToiDaCuTri+1, SoLuotBinhChonToiDa);
                _log.Info($"GiaTriPhieuLonNhat: {GiaTriPhieuLonNhat} - Check:{GiaTriPhieuLonNhat < GiaTriPhieuBauHienTai}");
                
                if(GiaTriPhieuLonNhat < GiaTriPhieuBauHienTai)
                    return -4;

                //17.5 Kiểm tra ID của danh mục úng cử & ngày bầu cử có tồn tại trong kỳ bầu cử không không
                bool CheckID_ListOfPosition = await _listOfPositionRepository._CheckTheListOgCandidatesWithTheVotingDateTogether(votingDay,voterVoteDTO.ID_Cap,connect);
                if(!CheckID_ListOfPosition) return -5;

                //17.6 Kiểm tra ID của đơn vị bầu, mã cử tri và ngày bắt đầu có cùng tồn tại không
                bool CheckID_Constituency = await _constituencyRepository._CheckVoterID_ConsituencyID_andPollingDateTogether(voterVoteDTO.ID_DonViBauCu.ToString(),voterVoteDTO.ID_CuTri,votingDay ,connect);
                if(!CheckID_Constituency) return -6;
                
                //17.7 Tự tạo phiếu phầu khi người dùng bỏ phiếu và thêm vào đó luôn
                    //Lấy 2 ký tự ngẫu nhiên
                string randomString = RandomString.ChuoiNgauNhien(2);
                DateTime currentDay = DateTime.Now;
                string ID_Phieu = randomString+$"{currentDay:yyyyMMddHHmmssff}";

                    //Lấy N và G dựa trên ngày bầu cử
                LockDTO Lock = await  _lockRepository._getLockBasedOnElectionDate(voterVoteDTO.ngayBD,connect);
                _log.Info($"N: {Lock.N}"); 
                _log.Info($"G: {Lock.G}");
                _log.Info($"GiaTriPhieuBauHienTai: {GiaTriPhieuBauHienTai}");
                    
                    //Mã hóa phiếu trước khi lưu    
                BigInteger GiaTriPhieuBauHienTai_MaHoa = _paillierServices.Encryption(Lock.G, Lock.N, GiaTriPhieuBauHienTai);

                VoteDto phieubau = new VoteDto();
                phieubau.ngayBD = voterVoteDTO.ngayBD.ToString();
                phieubau.ID_cap = voterVoteDTO.ID_Cap;
                phieubau.GiaTriPhieuBau = GiaTriPhieuBauHienTai_MaHoa;

                bool addVote = await _voteRepository._AddVote(ID_Phieu, phieubau, connect);
                if(!addVote){
                    await transaction.RollbackAsync();
                    return -7;
                }
                _log.Info($"Đã thêm phiếu bầu (Mã phiếu: {ID_Phieu})");

                //17.7 Cập nhật trạng thái phiếu bầu là người dùng này đã bầu
                bool updateStatusElectionsBy_IDcutri = await _UpdateStatusElectionsBy_IDcutri(voterVoteDTO.ID_CuTri, voterVoteDTO.ngayBD,connect);
                if(!updateStatusElectionsBy_IDcutri){
                    await transaction.RollbackAsync();
                    return -8;
                }

                //17.8 Cập nhật chi tiết phiếu bầu , là cho biết cử tri đã bầu bằng phiếu nào và thời điểm nào
                ElectionDetailsDTO electionDetailsDTO = new ElectionDetailsDTO{
                    ID_CuTri = voterVoteDTO.ID_CuTri,
                    ID_Phieu = ID_Phieu,
                    ThoiDiem = DateTime.Now
                };
                bool addElectionDetail = await _AddElectionDetailsBy_IDcutri(electionDetailsDTO,connect);
                if(!addElectionDetail){
                    await transaction.RollbackAsync();
                    return -9;
                }

                await transaction.CommitAsync();
                _log.Info($"Đã ghi nhận bỏ phiếu");
                return 1;

            }catch(MySqlException ex){
                _log.Info($"Error message: {ex.Message}");
                _log.Info($"Error Code: {ex.Code}");
                _log.Info($"Error Source: {ex.Source}");
                _log.Info($"Error HResult: {ex.HResult}");
                await transaction.RollbackAsync();
                throw;
            }
            catch(Exception ex){
                _log.Info($"Error message: {ex.Message}");
                _log.Info($"Error Source: {ex.Source}");
                _log.Info($"Error StackTrace: {ex.StackTrace}");
                _log.Info($"Error TargetSite: {ex.TargetSite}");
                _log.Info($"Error HResult: {ex.HResult}");
                _log.Info($"Error InnerException: {ex.InnerException}");
                await transaction.RollbackAsync();
                throw;
            }
        }

        //18. uứng cử viên bỏ phiếu
        public async Task<int> _CandidateVote(CandidateVoteDTO candidateVoteDTO){
            using var connect = await _context.Get_MySqlConnection();
            //Kiểm tra kết nối
            if(connect.State != System.Data.ConnectionState.Open )
                await connect.OpenAsync();
            
            using var transaction = await connect.BeginTransactionAsync();

            try{
                _log.Info(" --- Đến bên trong phần kiểm tra --- ");
                //Chuyển đổi dữ liệu
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime votingDay = DateTime.ParseExact(candidateVoteDTO.ngayBD,"yyyy-MM-dd HH:mm:ss",provider);
                var timeOfTheElectionDTO = await _electionsRepository._GetTimeOfElection(candidateVoteDTO.ngayBD, connect);
                //DateTime pollingStopTime = 

                _log.Info("\t\t --- Thông tin đầu vào --");
                _log.Info($"timeOfTheElectionDTO: {timeOfTheElectionDTO}");
                _log.Info($"Ngày BD: {votingDay}");
                _log.Info($"Ngày HIện tại: {DateTime.Now}");
                _log.Info($"ID_ucv: {candidateVoteDTO.ID_ucv}");
                _log.Info($"ID_DonViBauCu: {candidateVoteDTO.ID_DonViBauCu}");
                _log.Info($"ID_Cap: {candidateVoteDTO.ID_Cap}");
                _log.Info($"GiaTriPhieuBau: {candidateVoteDTO.GiaTriPhieuBau}");
                
                //18.-1 Kiếm ngày bắt đầu không tồn tại thì báo lỗi
                if(timeOfTheElectionDTO == null)
                    return -10;

                _log.Info($"Ngày KT: {timeOfTheElectionDTO.ngayKT}");
                _log.Info("\t\t --------------------------");

                //Chuyển đổi giá trị phiếu
                BigInteger GiaTriPhieuBauHienTai = BigInteger.Parse(candidateVoteDTO.GiaTriPhieuBau);

                //18.0 Kiêm tra xem thời điểm bỏ phiếu hợp lệ không. Nếu không thì trả về
                // if(DateTime.Now > timeOfTheElectionDTO.ngayKT || DateTime.Now < votingDay){
                //    return 0;
                // } 

                //18.1 Kiểm tra xem ứng cử viên có tồn tại không
                bool checkVoterExists = await _candidateRepository._CheckCandidateExists(candidateVoteDTO.ID_ucv, connect);
                if(!checkVoterExists)
                    return -1;

                //18.2 Kiểm tra xem thời điểm bắt đầu bầu cử có tồn tại không
                bool checkElectionExist = await _electionsRepository._CheckIfElectionTimeExists(votingDay, connect);
                if(!checkElectionExist){
                    _log.Info("Kiểm tra xem thời điểm bắt đầu bầu cử có tồn tại không");
                    return -2;
                } 
                
                //18.3 Kiểm tra xem ứng cử viên đã bỏ phiếu chưa ,nếu rồi thì không cho bỏ hiếu
                bool checkVoterHasVoted = await _candidateRepository._CheckCandidateHasVoted(candidateVoteDTO.ID_ucv, votingDay, connect);
                if(checkVoterHasVoted)
                    return -3;

                //18.4 Kiểm tra giá trị phiếu bầu có hợp lệ không
                int SoLuotBinhChonToiDa = await _electionsRepository._MaximumNumberOfVotes(votingDay, connect);
                int SoLuongToiDaCuTri = await _electionsRepository._MaximumNumberOfVoters(votingDay, connect);
                _log.Info($"SoLuotBinhChonToiDa(S): {SoLuotBinhChonToiDa}");
                _log.Info($"SoLuongToiDaCuTri(N): {SoLuongToiDaCuTri}");
                
                BigInteger GiaTriPhieuLonNhat = _paillierServices.GiaTriToiDaCuaPhieuBau_M(SoLuongToiDaCuTri+1, SoLuotBinhChonToiDa);
                _log.Info($"GiaTriPhieuLonNhat: {GiaTriPhieuLonNhat} - Check:{GiaTriPhieuLonNhat < GiaTriPhieuBauHienTai}");
                
                if(GiaTriPhieuLonNhat < GiaTriPhieuBauHienTai)
                    return -4;

                //18.5 Kiểm tra ID của danh mục úng cử & ngày bầu cử có tồn tại trong kỳ bầu cử không không
                bool CheckID_ListOfPosition = await _listOfPositionRepository._CheckTheListOgCandidatesWithTheVotingDateTogether(votingDay,candidateVoteDTO.ID_Cap,connect);
                if(!CheckID_ListOfPosition) return -5;

                //18.6 Kiểm tra ID của đơn vị bầu, mã cử tri và ngày bắt đầu có cùng tồn tại không
                bool CheckID_Constituency = await _constituencyRepository._CheckCandidateID_ConsituencyID_andPollingDateTogether(candidateVoteDTO.ID_DonViBauCu.ToString(),candidateVoteDTO.ID_ucv,votingDay ,connect);
                if(!CheckID_Constituency) return -6;
                
                //18.7 Tự tạo phiếu phầu khi người dùng bỏ phiếu và thêm vào đó luôn
                    //Lấy 2 ký tự ngẫu nhiên
                string randomString = RandomString.ChuoiNgauNhien(2);
                DateTime currentDay = DateTime.Now;
                string ID_Phieu = randomString+$"{currentDay:yyyyMMddHHmmssff}";

                    //Lấy N và G dựa trên ngày bầu cử
                LockDTO Lock = await  _lockRepository._getLockBasedOnElectionDate(candidateVoteDTO.ngayBD,connect);
                _log.Info($"N: {Lock.N}"); 
                _log.Info($"G: {Lock.G}");
                _log.Info($"GiaTriPhieuBauHienTai: {GiaTriPhieuBauHienTai}");
                    
                    //Mã hóa phiếu trước khi lưu    
                BigInteger GiaTriPhieuBauHienTai_MaHoa = _paillierServices.Encryption(Lock.G, Lock.N, GiaTriPhieuBauHienTai);

                VoteDto phieubau = new VoteDto();
                phieubau.ngayBD = candidateVoteDTO.ngayBD.ToString();
                phieubau.ID_cap = candidateVoteDTO.ID_Cap;
                phieubau.GiaTriPhieuBau = GiaTriPhieuBauHienTai_MaHoa;

                bool addVote = await _voteRepository._AddVote(ID_Phieu, phieubau, connect);
                if(!addVote){
                    await transaction.RollbackAsync();
                    return -7;
                }
                _log.Info($"Đã thêm phiếu bầu (Mã phiếu: {ID_Phieu})");

                //18.7 Cập nhật trạng thái phiếu bầu là người dùng này đã bầu
                bool updateStatusElections= await _UpdateStatusElectionsBy_IDucv(candidateVoteDTO.ID_ucv, candidateVoteDTO.ngayBD,connect);
                if(updateStatusElections != true){
                    await transaction.RollbackAsync();
                    return -8;
                }

                //18.8 Cập nhật chi tiết phiếu bầu , là cho biết cử tri đã bầu bằng phiếu nào và thời điểm nào
                ElectionDetailsDTO electionDetailsDTO = new ElectionDetailsDTO{
                    ID_ucv = candidateVoteDTO.ID_ucv,
                    ID_Phieu = ID_Phieu,
                    ThoiDiem = DateTime.Now
                };
                bool addElectionDetail = await _AddElectionDetailsBy_IDucv(electionDetailsDTO,connect);
                if(!addElectionDetail){
                    await transaction.RollbackAsync();
                    return -9;
                }

                await transaction.CommitAsync();
                _log.Info($"Đã ghi nhận bỏ phiếu");
                return 1;

            }catch(MySqlException ex){
                _log.Info($"Error message: {ex.Message}");
                _log.Info($"Error Code: {ex.Code}");
                _log.Info($"Error Source: {ex.Source}");
                _log.Info($"Error HResult: {ex.HResult}");
                await transaction.RollbackAsync();
                throw;
            }
            catch(Exception ex){
                _log.Info($"Error message: {ex.Message}");
                _log.Info($"Error Source: {ex.Source}");
                _log.Info($"Error StackTrace: {ex.StackTrace}");
                _log.Info($"Error TargetSite: {ex.TargetSite}");
                _log.Info($"Error HResult: {ex.HResult}");
                _log.Info($"Error InnerException: {ex.InnerException}");
                await transaction.RollbackAsync();
                throw;
            }
        }

        //19. cán bộ bỏ phiếu
        public async Task<int> _CadreVote(CadreVoteDTO cadreVoteDTO){
            using var connect = await _context.Get_MySqlConnection();
            //Kiểm tra kết nối
            if(connect.State != System.Data.ConnectionState.Open )
                await connect.OpenAsync();
            
            using var transaction = await connect.BeginTransactionAsync();

            try{
                _log.Info(" --- Đến bên trong phần kiểm tra --- ");
                //Chuyển đổi dữ liệu
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime votingDay = DateTime.ParseExact(cadreVoteDTO.ngayBD,"yyyy-MM-dd HH:mm:ss",provider);
                var timeOfTheElectionDTO = await _electionsRepository._GetTimeOfElection(cadreVoteDTO.ngayBD, connect);
                //DateTime pollingStopTime = 

                _log.Info("\t\t --- Thông tin đầu vào --");
                _log.Info($"timeOfTheElectionDTO: {timeOfTheElectionDTO}");
                _log.Info($"Ngày BD: {votingDay}");
                _log.Info($"Ngày HIện tại: {DateTime.Now}");
                _log.Info($"ID_ucv: {cadreVoteDTO.ID_CanBo}");
                _log.Info($"ID_DonViBauCu: {cadreVoteDTO.ID_DonViBauCu}");
                _log.Info($"ID_Cap: {cadreVoteDTO.ID_Cap}");
                _log.Info($"GiaTriPhieuBau: {cadreVoteDTO.GiaTriPhieuBau}");
                
                //19.-1 Kiếm ngày bắt đầu không tồn tại thì báo lỗi
                if(timeOfTheElectionDTO == null)
                    return -10;

                _log.Info($"Ngày KT: {timeOfTheElectionDTO.ngayKT}");
                _log.Info("\t\t --------------------------");

                //Chuyển đổi giá trị phiếu
                BigInteger GiaTriPhieuBauHienTai = BigInteger.Parse(cadreVoteDTO.GiaTriPhieuBau);

                //19.0 Kiêm tra xem thời điểm bỏ phiếu hợp lệ không. Nếu không thì trả về
                // if(DateTime.Now > timeOfTheElectionDTO.ngayKT || DateTime.Now < votingDay){
                //    return 0;
                // } 

                //19.1 Kiểm tra xem ứng cử viên có tồn tại không
                bool checkVoterExists = await _cadreRepository._CheckCadreExists(cadreVoteDTO.ID_CanBo, connect);
                if(!checkVoterExists)
                    return -1;

                //19.2 Kiểm tra xem thời điểm bắt đầu bầu cử có tồn tại không
                bool checkElectionExist = await _electionsRepository._CheckIfElectionTimeExists(votingDay, connect);
                if(!checkElectionExist){
                    _log.Info("Kiểm tra xem thời điểm bắt đầu bầu cử có tồn tại không");
                    return -2;
                } 
                
                //19.3 Kiểm tra xem ứng cử viên đã bỏ phiếu chưa ,nếu rồi thì không cho bỏ hiếu
                bool checkVoterHasVoted = await _cadreRepository._CheckCadreHasVoted(cadreVoteDTO.ID_CanBo, votingDay, connect);
                if(checkVoterHasVoted)
                    return -3;

                //19.4 Kiểm tra giá trị phiếu bầu có hợp lệ không
                int SoLuotBinhChonToiDa = await _electionsRepository._MaximumNumberOfVotes(votingDay, connect);
                int SoLuongToiDaCuTri = await _electionsRepository._MaximumNumberOfVoters(votingDay, connect);
                _log.Info($"SoLuotBinhChonToiDa(S): {SoLuotBinhChonToiDa}");
                _log.Info($"SoLuongToiDaCuTri(N): {SoLuongToiDaCuTri}");
                
                BigInteger GiaTriPhieuLonNhat = _paillierServices.GiaTriToiDaCuaPhieuBau_M(SoLuongToiDaCuTri+1, SoLuotBinhChonToiDa);
                _log.Info($"GiaTriPhieuLonNhat: {GiaTriPhieuLonNhat} - Check:{GiaTriPhieuLonNhat < GiaTriPhieuBauHienTai}");
                
                if(GiaTriPhieuLonNhat < GiaTriPhieuBauHienTai)
                    return -4;

                //19.5 Kiểm tra ID của danh mục úng cử & ngày bầu cử có tồn tại trong kỳ bầu cử không không
                bool CheckID_ListOfPosition = await _listOfPositionRepository._CheckTheListOgCandidatesWithTheVotingDateTogether(votingDay,cadreVoteDTO.ID_Cap,connect);
                if(!CheckID_ListOfPosition) return -5;

                //19.6 Kiểm tra ID của đơn vị bầu, mã cử tri và ngày bắt đầu có cùng tồn tại không
                bool CheckID_Constituency = await _constituencyRepository._CheckCadreID_ConsituencyID_andPollingDateTogether(cadreVoteDTO.ID_DonViBauCu.ToString(),cadreVoteDTO.ID_CanBo,votingDay ,connect);
                if(!CheckID_Constituency) return -6;
                
                //19.7 Tự tạo phiếu phầu khi người dùng bỏ phiếu và thêm vào đó luôn
                    //Lấy 2 ký tự ngẫu nhiên
                string randomString = RandomString.ChuoiNgauNhien(2);
                DateTime currentDay = DateTime.Now;
                string ID_Phieu = randomString+$"{currentDay:yyyyMMddHHmmssff}";

                    //Lấy N và G dựa trên ngày bầu cử
                LockDTO Lock = await  _lockRepository._getLockBasedOnElectionDate(cadreVoteDTO.ngayBD,connect);
                _log.Info($"N: {Lock.N}"); 
                _log.Info($"G: {Lock.G}");
                _log.Info($"GiaTriPhieuBauHienTai: {GiaTriPhieuBauHienTai}");
                    
                    //Mã hóa phiếu trước khi lưu    
                BigInteger GiaTriPhieuBauHienTai_MaHoa = _paillierServices.Encryption(Lock.G, Lock.N, GiaTriPhieuBauHienTai);

                VoteDto phieubau = new VoteDto();
                phieubau.ngayBD = cadreVoteDTO.ngayBD.ToString();
                phieubau.ID_cap = cadreVoteDTO.ID_Cap;
                phieubau.GiaTriPhieuBau = GiaTriPhieuBauHienTai_MaHoa;

                bool addVote = await _voteRepository._AddVote(ID_Phieu, phieubau, connect);
                if(!addVote){
                    await transaction.RollbackAsync();
                    return -7;
                }
                _log.Info($"Đã thêm phiếu bầu (Mã phiếu: {ID_Phieu})");

                //19.7 Cập nhật trạng thái phiếu bầu là người dùng này đã bầu
                bool updateStatusElections= await _UpdateStatusElectionsBy_IDcanbo(cadreVoteDTO.ID_CanBo, cadreVoteDTO.ngayBD,connect);
                if(updateStatusElections != true){
                    await transaction.RollbackAsync();
                    return -8;
                }

                //19.8 Cập nhật chi tiết phiếu bầu , là cho biết cử tri đã bầu bằng phiếu nào và thời điểm nào
                ElectionDetailsDTO electionDetailsDTO = new ElectionDetailsDTO{
                    ID_CanBo = cadreVoteDTO.ID_CanBo,
                    ID_Phieu = ID_Phieu,
                    ThoiDiem = DateTime.Now
                };
                bool addElectionDetail = await _AddElectionDetailsBy_IDcanbo(electionDetailsDTO,connect);
                if(!addElectionDetail){
                    await transaction.RollbackAsync();
                    return -9;
                }

                await transaction.CommitAsync();
                _log.Info($"Đã ghi nhận bỏ phiếu");
                return 1;

            }catch(MySqlException ex){
                _log.Info($"Error message: {ex.Message}");
                _log.Info($"Error Code: {ex.Code}");
                _log.Info($"Error Source: {ex.Source}");
                _log.Info($"Error HResult: {ex.HResult}");
                await transaction.RollbackAsync();
                throw;
            }
            catch(Exception ex){
                _log.Info($"Error message: {ex.Message}");
                _log.Info($"Error Source: {ex.Source}");
                _log.Info($"Error StackTrace: {ex.StackTrace}");
                _log.Info($"Error TargetSite: {ex.TargetSite}");
                _log.Info($"Error HResult: {ex.HResult}");
                _log.Info($"Error InnerException: {ex.InnerException}");
                await transaction.RollbackAsync();
                throw;
            }
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