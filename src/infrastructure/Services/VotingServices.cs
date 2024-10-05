

using BackEnd.src.core.Common;
using BackEnd.src.core.Interfaces;
using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;

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

        public VotingServices(
            DatabaseContext context,
            IVoterRepository voterRepository, 
            IElectionsRepository electionsRepository,
            IPaillierServices paillierServices,
            IConstituencyRepository constituencyRepository,
            IVoteRepository voteRepository,  //Thêm
            IListOfPositionRepository listOfPositionRepository
        )
        {
            _voterRepository = voterRepository;
            _electionsRepository = electionsRepository;
            _context = context;
            _paillierServices = paillierServices;
            _constituencyRepository = constituencyRepository;
            _voteRepository = voteRepository;  //Thêm
            _listOfPositionRepository = listOfPositionRepository;
        }

        //Hủy
        public new void Dispose() => _context.Dispose();

        //17. Cử tri bỏ phiếu
        public async Task<int> _VoterVote(VoterVoteDTO voterVoteDTO){
            using var connect = await _context.Get_MySqlConnection();
            using var transaction = await connect.BeginTransactionAsync();

            try{
                
                //17.0 Kiêm tra xem thời điểm bỏ phiếu hợp lệ không. Nếu không thì trả về
                TimeOfTheElectionDTO timeOfTheElectionDTO = await _electionsRepository._GetTimeOfElection(voterVoteDTO.ngayBD, connect);
                if(DateTime.Now > timeOfTheElectionDTO.ngayKT || DateTime.Now < voterVoteDTO.ngayBD) return 0;

                //17.1 Kiểm tra xem cử tri có tồn tại không
                bool checkVoterExists = await _voterRepository._CheckVoterExists(voterVoteDTO.ID_CuTri, connect);
                if(!checkVoterExists) return -1;

                //17.2 Kiểm tra xem thời điểm bắt đầu bầu cử có tồn tại không
                bool checkElectionExist = await _electionsRepository._CheckIfElectionTimeExists(voterVoteDTO.ngayBD, connect);
                if(!checkElectionExist) return -2;
                
                //17.3 Kiểm tra xem cử tri đã bỏ phiếu chưa ,nếu rồi thì không cho bỏ hiếu
                bool checkVoterHasVoted = await _voterRepository._CheckVoterHasVoted(voterVoteDTO.ID_CuTri, voterVoteDTO.ngayBD, connect);
                if(checkVoterHasVoted) return -3;

                //17.4 Kiểm tra giá trị phiếu bầu có hợp lệ không
                int SoLuotBinhChonToiDa = await _electionsRepository._MaximumNumberOfVotes(voterVoteDTO.ngayBD, connect);
                int SoLuongToiDaCuTri = await _electionsRepository._MaximumNumberOfVoters(voterVoteDTO.ngayBD, connect);
                int GiaTriPhieuLonNhat = _paillierServices.GiaTriToiDaCuaPhieuBau_M(SoLuongToiDaCuTri+1, SoLuotBinhChonToiDa);
                if(GiaTriPhieuLonNhat < voterVoteDTO.GiaTriPhieuBau) return -4;

                //17.5 Kiểm tra ID của danh mục úng cử có tồn tại không
                bool CheckID_ListOfPosition = await _listOfPositionRepository._CheckIfTheCodeIsInTheListOfPosition(voterVoteDTO.ID_Cap,connect);
                if(!CheckID_ListOfPosition) return -5;

                //17.6 Kiểm tra ID của đơn vị bầu cử có tồn tại không'
                bool CheckID_Constituency = await _constituencyRepository._CheckIfConstituencyExists(voterVoteDTO.ID_DonViBauCu.ToString(),connect);
                if(!CheckID_ListOfPosition) return -6;
                
                //17.6 Tự tạo phiếu phầu khi người dùng bỏ phiếu và thêm vào đó luôn
                    //Lấy 2 ký tự ngẫu nhiên
                string randomString = RandomString.ChuoiNgauNhien(2);
                DateTime currentDay = DateTime.Now;
                string ID_Phieu = randomString+$"{currentDay:yyyyMMddHHmmssff}";

                VoteDto phieubau = new VoteDto();
                phieubau.ngayBD = voterVoteDTO.ngayBD.ToString();
                phieubau.ID_cap = voterVoteDTO.ID_Cap;
                phieubau.GiaTriPhieuBau = voterVoteDTO.GiaTriPhieuBau;

                bool addVote = await _voteRepository._AddVote(ID_Phieu, phieubau, connect);
                if(!addVote){
                    await transaction.RollbackAsync();
                    return -7;
                }

                //17.7 Cập nhật trạng thái phiếu bầu là người dùng này đã bầu
                bool updateStatusElectionsBy_IDcutri = await _UpdateStatusElectionsBy_IDcutri(voterVoteDTO.ID_CuTri, connect);
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
        public async Task<bool> _UpdateStatusElectionsBy_IDcutri(string id_cutri, MySqlConnection connection){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
            
            try{
                //Kiểm tra mã cử tri có tồn tại không
                bool checkVoterExists = await _voterRepository._CheckVoterExists(id_cutri, connection);
                if(!checkVoterExists) return  false;

                const string sql = @"UPDATE trangthaibaucu SET GhiNhan = '1' WHERE ID_CuTri = @ID_CuTri;";
                using (var command = new MySqlCommand(sql,connection)){
                    command.Parameters.AddWithValue("@ID_CuTri", id_cutri);
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