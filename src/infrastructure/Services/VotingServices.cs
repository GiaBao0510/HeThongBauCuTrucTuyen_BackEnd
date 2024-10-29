using Microsoft.AspNetCore.Http;
using BackEnd.src.core.Common;
using BackEnd.src.core.Interfaces;
using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Numerics;

namespace BackEnd.src.infrastructure.Services
{
    public class VotingServices :IDisposable, IVotingServices
    {
        private readonly IVoterRepository _voterRepository;
        private readonly IElectionsRepository _electionsRepository;
        private readonly DatabaseContext _context;
        private readonly IVoteRepository _voteRepository;
        private readonly IPaillierServices _paillierServices;
        private readonly IListOfPositionRepository _listOfPositionRepository;
        private readonly IConstituencyRepository _constituencyRepository;
        private readonly ILockRepository _lockRepository;

        public VotingServices(
            DatabaseContext context,
            IVoterRepository voterRepository, 
            IElectionsRepository electionsRepository,
            IPaillierServices paillierServices,
            IConstituencyRepository constituencyRepository,
            IVoteRepository voteRepository,  //Thêm
            IListOfPositionRepository listOfPositionRepository,
            ILockRepository lockRepository
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
                
                Console.WriteLine(" --- Đến bên trong phần kiểm tra --- ");
                //Chuyển đổi dữ liệu
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime votingDay = DateTime.ParseExact(voterVoteDTO.ngayBD,"yyyy-MM-dd HH:mm:ss",provider);
                var timeOfTheElectionDTO = await _electionsRepository._GetTimeOfElection(voterVoteDTO.ngayBD, connect);
                //DateTime pollingStopTime = 

                Console.WriteLine("\t\t --- Thông tin đầu vào --");
                Console.WriteLine($"timeOfTheElectionDTO: {timeOfTheElectionDTO}");
                Console.WriteLine($"Ngày BD: {votingDay}");
                Console.WriteLine($"Ngày HIện tại: {DateTime.Now}");
                Console.WriteLine($"ID_CuTri: {voterVoteDTO.ID_CuTri}");
                Console.WriteLine($"ID_DonViBauCu: {voterVoteDTO.ID_DonViBauCu}");
                Console.WriteLine($"ID_Cap: {voterVoteDTO.ID_Cap}");
                Console.WriteLine($"GiaTriPhieuBau: {voterVoteDTO.GiaTriPhieuBau}");
                
                //17.-1 Kiếm ngày bắt đầu không tồn tại thì báo lỗi
                if(timeOfTheElectionDTO == null)
                    return -10;

                Console.WriteLine($"Ngày KT: {timeOfTheElectionDTO.ngayKT}");
                Console.WriteLine("\t\t --------------------------");

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
                    Console.WriteLine("Kiểm tra xem thời điểm bắt đầu bầu cử có tồn tại không");
                    return -2;
                } 
                
                //17.3 Kiểm tra xem cử tri đã bỏ phiếu chưa ,nếu rồi thì không cho bỏ hiếu
                bool checkVoterHasVoted = await _voterRepository._CheckVoterHasVoted(voterVoteDTO.ID_CuTri, votingDay, connect);
                if(checkVoterHasVoted)
                    return -3;

                //17.4 Kiểm tra giá trị phiếu bầu có hợp lệ không
                int SoLuotBinhChonToiDa = await _electionsRepository._MaximumNumberOfVotes(votingDay, connect);
                int SoLuongToiDaCuTri = await _electionsRepository._MaximumNumberOfVoters(votingDay, connect);
                Console.WriteLine($"SoLuotBinhChonToiDa(S): {SoLuotBinhChonToiDa}");
                Console.WriteLine($"SoLuongToiDaCuTri(N): {SoLuongToiDaCuTri}");
                BigInteger GiaTriPhieuLonNhat = _paillierServices.GiaTriToiDaCuaPhieuBau_M(SoLuongToiDaCuTri+1, SoLuotBinhChonToiDa);
                Console.WriteLine($"GiaTriPhieuLonNhat: {GiaTriPhieuLonNhat} - Check:{GiaTriPhieuLonNhat < GiaTriPhieuBauHienTai}");
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
                Console.WriteLine($"N: {Lock.N}");
                Console.WriteLine($"G: {Lock.G}");
                Console.WriteLine($"GiaTriPhieuBauHienTai: {GiaTriPhieuBauHienTai}");
                    
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
                if(!updateStatusElectionsBy_IDcutri){
                    await transaction.RollbackAsync();
                    return -9;
                }

                await transaction.CommitAsync();
                return 1;

            }catch(MySqlException ex){
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Error Code: {ex.Code}");
                Console.WriteLine($"Error Source: {ex.Source}");
                Console.WriteLine($"Error HResult: {ex.HResult}");
                await transaction.RollbackAsync();
                throw;
            }
            catch(Exception ex){
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Error Source: {ex.Source}");
                Console.WriteLine($"Error StackTrace: {ex.StackTrace}");
                Console.WriteLine($"Error TargetSite: {ex.TargetSite}");
                Console.WriteLine($"Error HResult: {ex.HResult}");
                Console.WriteLine($"Error InnerException: {ex.InnerException}");
                await transaction.RollbackAsync();
                throw;
            }
        }

        //Cập nhật chi tiết phiếu bầu
        public async Task<bool> _AddElectionDetailsBy_IDcutri(ElectionDetailsDTO electionDetailsDTO, MySqlConnection connection){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
            
            try{
                //Kiểm tra mã cử tri có tồn tại không
                bool checkVoterExists = await _voterRepository._CheckVoterExists(electionDetailsDTO.ID_CuTri, connection);
                if(!checkVoterExists) return  false;

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
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Error Code: {ex.Code}");
                Console.WriteLine($"Error Source: {ex.Source}");
                Console.WriteLine($"Error HResult: {ex.HResult}");
                throw;
            }
            catch(Exception ex){
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Error Source: {ex.Source}");
                Console.WriteLine($"Error StackTrace: {ex.StackTrace}");
                Console.WriteLine($"Error TargetSite: {ex.TargetSite}");
                Console.WriteLine($"Error HResult: {ex.HResult}");
                Console.WriteLine($"Error InnerException: {ex.InnerException}");
                throw;
            }
        }

        //Cập nhật lại trạng thái bầu cử dựa trên mã cử tri
        public async Task<bool> _UpdateStatusElectionsBy_IDcutri(string id_cutri, string ngayBD,MySqlConnection connection){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
            
            try{
                //Kiểm tra mã cử tri có tồn tại không
                bool checkVoterExists = await _voterRepository._CheckVoterExists(id_cutri, connection);
                if(!checkVoterExists) return  false;

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
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Error Code: {ex.Code}");
                Console.WriteLine($"Error Source: {ex.Source}");
                Console.WriteLine($"Error HResult: {ex.HResult}");
                throw;
            }
            catch(Exception ex){
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Error Source: {ex.Source}");
                Console.WriteLine($"Error StackTrace: {ex.StackTrace}");
                Console.WriteLine($"Error TargetSite: {ex.TargetSite}");
                Console.WriteLine($"Error HResult: {ex.HResult}");
                Console.WriteLine($"Error InnerException: {ex.InnerException}");
                throw;
            }
        }


    }
}