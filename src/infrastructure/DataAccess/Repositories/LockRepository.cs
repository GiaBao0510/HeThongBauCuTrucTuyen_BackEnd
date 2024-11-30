using BackEnd.src.infrastructure.Hubs;
using System.Globalization;
using System.Numerics;
using BackEnd.src.core.Interfaces;
using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.web_api.DTOs;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using log4net;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class LockRepository :IDisposable, ILockRepository
    {
        private readonly DatabaseContext _context;
        private readonly IElectionsRepository _electionsRepository;
        private readonly IPaillierServices _paillierServices;
        private readonly NotificationHubs _notificationHubs;
        private readonly IResultsAnnouncementDetailsRepository _resultsAnnouncementDetails;
        public IConfiguration Configuration{get;}
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program)); 

        //khởi tạo
        public LockRepository(
            DatabaseContext context, 
            IElectionsRepository electionsRepository, 
            IConfiguration configuration,
            IPaillierServices paillierServices,
            IResultsAnnouncementDetailsRepository resultsAnnouncementDetails,
            NotificationHubs notificationHubs
        ){
            _context = context;
            _electionsRepository = electionsRepository;
            Configuration = configuration;
            _paillierServices = paillierServices;
            _resultsAnnouncementDetails = resultsAnnouncementDetails;
            _notificationHubs = notificationHubs;
        }

        //Hàm hủy
        public void Dispose() => _context.Dispose();

        //1.Lấy thông tin khóa công khai theo ngày bắt đầu
        public async Task<LockDTO> _getLockBasedOnElectionDate(string ngayBD){
            using var connection = await _context.Get_MySqlConnection();
            try{ 

                //Kiểm tra ngày bắt đầu bầu cử có tồn tại không
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime votingDay = DateTime.ParseExact(ngayBD,"yyyy-MM-dd HH:mm:ss",provider);
                bool check_ngayBD = await _electionsRepository._CheckIfElectionTimeExists(votingDay, connection);
                if(!check_ngayBD) return null;

                //Lấy thông tin trả về kết quả
                const string sql = @"
                SELECT *
                FROM khoa WHERE ngayBD = @ngayBD;";

                using (var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ngayBD",ngayBD);
                    using var reader = await command.ExecuteReaderAsync();
                    if(await reader.ReadAsync()){
                        return new LockDTO{
                            ID_Khoa = reader.GetInt32(reader.GetOrdinal("ID_Khoa")),
                            NgayTao = reader.GetDateTime(reader.GetOrdinal("NgayTao")),
                            N = new BigInteger(reader.GetInt64(reader.GetOrdinal("N"))),
                            G = new BigInteger(reader.GetInt64(reader.GetOrdinal("G"))),
                            path_PK = reader.GetString(reader.GetOrdinal("path_PK")),
                        };
                    }
                }

                return null;
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

        //1.1 Lấy thông tin khóa công khai theo ngày bắt đầu
        public async Task<LockDTO> _getLockBasedOnElectionDate(string ngayBD, MySqlConnection connection){
            try{
                    //Kiểm tra trạng thái kết nối trước khi mở
                if(connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                //Kiểm tra ngày bắt đầu bầu cử có tồn tại không
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime votingDay = DateTime.ParseExact(ngayBD,"yyyy-MM-dd HH:mm:ss",provider);
                bool check_ngayBD = await _electionsRepository._CheckIfElectionTimeExists(votingDay, connection);
                if(!check_ngayBD) return null;

                //Lấy thông tin trả về kết quả
                const string sql = @"
                SELECT *
                FROM khoa WHERE ngayBD = @ngayBD;";

                using (var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ngayBD",ngayBD);
                    using var reader = await command.ExecuteReaderAsync();
                    if(await reader.ReadAsync()){
                        
                        return new LockDTO{
                            ID_Khoa = reader.GetInt32(reader.GetOrdinal("ID_Khoa")),
                            NgayTao = reader.GetDateTime(reader.GetOrdinal("NgayTao")),
                            N = BigInteger.Parse(reader.GetString(reader.GetOrdinal("N"))),
                            G = BigInteger.Parse(reader.GetString(reader.GetOrdinal("G"))),
                            path_PK = reader.GetString(reader.GetOrdinal("path_PK")),
                        };
                    }
                }

                return null;
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

        //2.Lấy tất cả thông tin khóa
        public async Task<List<LockDTO>> _getListKey(){
            using var connection = await _context.Get_MySqlConnection();
            var list = new List<LockDTO>();
            try{
                _log.Info($"Đường dẫn khóa mật: {Configuration["AppSettings:PrivateKeyPath"]}");
                //Lấy thông tin trả về kết quả
                const string sql = @"
                SELECT *
                FROM khoa;";

                using (var command = new MySqlCommand(sql, connection)){
                    using var reader = await command.ExecuteReaderAsync();
                        while(await reader.ReadAsync()){
                            list.Add(new LockDTO{
                                ID_Khoa = reader.GetInt32(reader.GetOrdinal("ID_Khoa")),
                                NgayTao = reader.GetDateTime(reader.GetOrdinal("NgayTao")),
                                N = BigInteger.Parse(reader.GetString(reader.GetOrdinal("N"))),
                                G = BigInteger.Parse(reader.GetString(reader.GetOrdinal("G"))),
                                path_PK = reader.GetString(reader.GetOrdinal("path_PK")),
                            });
                        }

                    return list;
                }
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

        //3.Xóa khóa theo ngày bắt đầu
        public async Task<bool> _deleteKeyBasedOnElectionDate(string ngayBD){
            using var connection = await _context.Get_MySqlConnection();
            try{

                //Kiểm tra ngày bắt đầu bầu cử có tồn tại không
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime votingDay = DateTime.ParseExact(ngayBD,"yyyy-MM-dd HH:mm:ss",provider);
                bool check_ngayBD = await _electionsRepository._CheckIfElectionTimeExists(votingDay, connection);
                if(!check_ngayBD) return false;

                //Lấy thông tin trả về kết quả
                const string sql = @"
                DELETE FROM khoa WHERE ngayBD = @ngayBD;";

                using (var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ngayBD",ngayBD);
                    await command.ExecuteNonQueryAsync();
                }

                //Thiết phần xóa tệp tin dựa trên ngày BD

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

        //4.Lấy khóa thông tin khóa mật theo ngày bắt đầu
        public async Task<PrivateKeyDTO> _getPrivateKeyBasedOnElectionDateAndKey(string ngayBD){
            using var connection = await _context.Get_MySqlConnection();
            try{
                 //Kiểm tra ngày bắt đầu bầu cử có tồn tại không
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime votingDay = DateTime.ParseExact(ngayBD,"yyyy-MM-dd HH:mm:ss",provider);
                bool check_ngayBD = await _electionsRepository._CheckIfElectionTimeExists(votingDay, connection);
                if(!check_ngayBD) return null;

                //Lấy thông tin trả về kết quả
                string path = null;
                const string sql = @"
                SELECT path_PK
                FROM khoa WHERE ngayBD = @ngayBD;";

                using (var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ngayBD",ngayBD);
                    using var reader = await command.ExecuteReaderAsync();
                    if(await reader.ReadAsync()){
                        path = reader.GetString(reader.GetOrdinal("path_PK"));
                    }
                }

                //Nếu đường dẫn tồn tại thì đọc
                if(File.Exists(path)){
                    //Đọc toàn bộ nội dung tệp
                    string content = File.ReadAllText(path);

                    //Tách các giá trị trong tệp tin dựa vào dấu ','
                    string[] values = content.Split(',');

                    if(values.Length == 2){
                        //Chuyển đổi giá trị đọc được thành BigInteger
                        return new PrivateKeyDTO{
                            lamda = BigInteger.Parse(values[0].Trim()),
                            muy = BigInteger.Parse(values[1].Trim())
                        };
                    }
                }else{
                    _log.Info("File khong tồn tại");
                }
                

                return null;
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

        //5.Lấy private key path dựa trên ngày bắt đầu
        public async Task<string> _getPrivateKeyPathBasedOnElectionDate(string ngayBD, MySqlConnection connection){
            try{
                //Kiểm tra xem ngày bắt đầu có tồn tại không
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime votingDay = DateTime.ParseExact(ngayBD,"yyyy-MM-dd HH:mm:ss",provider);
                bool check_ngayBD = await _electionsRepository._CheckIfElectionTimeExists(votingDay, connection);
                if(!check_ngayBD) return null;

                const string sql = @"
                SELECT path_PK
                FROM khoa 
                WHERE ngayBD = @ngayBD;";
                using (var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ngayBD",ngayBD);
                    using var reader = await command.ExecuteReaderAsync();

                    if(await reader.ReadAsync()){
                        return reader.GetString(reader.GetOrdinal("path_PK"));
                    }
                    return null;
                }
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

        //6.Giải mã các giá trị phiếu bầu dựa trên kỳ bầu cử
        public async Task<dynamic> _ListOfDecodedVotesBasedOnElection(string ngayBD){
            using var connection = await _context.Get_MySqlConnection();

            try{
                var list = new List<DecodingTheBallotsDTO>();
                var pbKey = await _getLockBasedOnElectionDate(ngayBD, connection);
                if(pbKey == null){
                    _log.Info("ngày bắt đầu không tồn tại");
                    return 0;
                }

                const string sql = @"
                SELECT pb.ID_Phieu, pb.GiaTriPhieuBau, 
                   ctbc.ThoiDiem, nd.ID_user, nd.HoTen
                FROM phieubau pb
                JOIN chitietbaucu ctbc ON ctbc.ID_Phieu = pb.ID_Phieu
                LEFT JOIN ungcuvien ucv ON ucv.ID_ucv = ctbc.ID_ucv
                LEFT JOIN canbo cb ON cb.ID_CanBo = ctbc.ID_CanBo
                LEFT JOIN cutri ct ON ct.ID_CuTri = ctbc.ID_CuTri
                JOIN nguoidung nd ON nd.ID_user = COALESCE(ucv.ID_user, cb.ID_user, ct.ID_user)
                WHERE pb.ngayBD =  @ngayBD;";

                //Kiểm tra đường dẫn tồn tại không 
                if(!File.Exists(pbKey.path_PK)){
                    _log.Info($"File khóa mật phiếu bầu không tồn tại:{pbKey.path_PK}");
                    return -1;
                }

                //Truy xuất các giá trị phiếu đã mã hóa và ngày bầu cử (Giá trị phiếu, ID phiếu , N, Path mã hóa)
                using(var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ngayBD",ngayBD);
                    using var reader = await command.ExecuteReaderAsync();
                    
                    while(await reader.ReadAsync()){
                        
                        //Đọc giá trị từng phiếu bầu
                        var vote = new DecodingTheBallotsDTO{
                            ID_Phieu = reader.GetString(reader.GetOrdinal("ID_Phieu")),
                            GiaTriPhieuBau = BigInteger.Parse(reader.GetString(reader.GetOrdinal("GiaTriPhieuBau"))),
                            ThoiDiem = reader.GetDateTime(reader.GetOrdinal("ThoiDiem")),
                            ID_user = reader.GetString(reader.GetOrdinal("ID_user")),
                            HoTen = reader.GetString(reader.GetOrdinal("HoTen"))
                        };

                        //Lấy đường dẫn khóa mật
                        string content = File.ReadAllText(pbKey.path_PK);

                        //Tách các giá trị trong tệp tin dựa vào dấu ','
                        string[] values = content.Split(',');

                        var pvKey = new PrivateKeyDTO();

                        if(values.Length == 2){
                            //Chuyển đổi giá trị đọc được thành BigInteger
                            pvKey.lamda = BigInteger.Parse(values[0].Trim());
                            pvKey.muy = BigInteger.Parse(values[1].Trim());

                            //Giải mã
                            var decodedValue = _paillierServices.Decryption(vote.GiaTriPhieuBau, pbKey.N, pvKey.lamda, pvKey.muy);
                            vote.GiaTriPhieuBau = decodedValue;
                            list.Add(vote);
                        }
                    }
                    return list;
                }
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

        //6.1 Giải mã các giá trị phiếu bầu dựa trên kỳ bầu cử - MySQLConnection
        public async Task<dynamic> _ListOfDecodedVotesBasedOnElection(string ngayBD, MySqlConnection connection){

            try{
                var list = new List<VoteDto>();
                var pbKey = await _getLockBasedOnElectionDate(ngayBD, connection);
                if(pbKey == null){
                    _log.Info("ngày bắt đầu không tồn tại");
                    return 0;
                }

                const string sql = @"
                SELECT pb.ID_Phieu, pb.GiaTriPhieuBau,pb.ID_cap
                FROM phieubau pb 
                JOIN khoa k ON pb.ngayBD = k.ngayBD
                WHERE k.ngayBD = @ngayBD;";

                //Kiểm tra đường dẫn tồn tại không 
                if(!File.Exists(pbKey.path_PK)){
                    _log.Info($"File khóa mật phiếu bầu không tồn tại:{pbKey.path_PK}");
                    return -1;
                }

                //Truy xuất các giá trị phiếu đã mã hóa và ngày bầu cử (Giá trị phiếu, ID phiếu , N, Path mã hóa)
                using(var command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@ngayBD",ngayBD);
                    using var reader = await command.ExecuteReaderAsync();
                    
                    while(await reader.ReadAsync()){
                        
                        //Đọc giá trị từng phiếu bầu
                        var vote = new VoteDto{
                            ID_Phieu = reader.GetString(reader.GetOrdinal("ID_Phieu")),
                            GiaTriPhieuBau = BigInteger.Parse(reader.GetString(reader.GetOrdinal("GiaTriPhieuBau"))),
                            ID_cap = reader.GetInt32(reader.GetOrdinal("ID_cap")),
                            ngayBD = ngayBD
                        };

                        //Lấy đường dẫn khóa mật
                        string content = File.ReadAllText(pbKey.path_PK);

                        //Tách các giá trị trong tệp tin dựa vào dấu ','
                        string[] values = content.Split(',');

                        var pvKey = new PrivateKeyDTO();

                        if(values.Length == 2){
                            //Chuyển đổi giá trị đọc được thành BigInteger
                            pvKey.lamda = BigInteger.Parse(values[0].Trim());
                            pvKey.muy = BigInteger.Parse(values[1].Trim());

                            //Giải mã
                            var decodedValue = _paillierServices.Decryption(vote.GiaTriPhieuBau, pbKey.N, pvKey.lamda, pvKey.muy);
                            vote.GiaTriPhieuBau = decodedValue;
                            list.Add(vote);
                        }
                    }
                    return list;
                }
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

        //7.Công bố kết quả bầu cử dựa trên ngày bầu cử
        public async Task<int> _CaculateAndAnnounceElectionResult(string ngayBD, string ID_CanBo){
            using var connection = await _context.Get_MySqlConnection();
            using var transaction = await connection.BeginTransactionAsync();
            _log.Info("\t\t == Chuẩn bị cập nhật công bố kết quả bầu cử ==");

            try{
                //Đưa ngày bắt đầu về dạng DateTime
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime startTime = DateTime.ParseExact(ngayBD,"yyyy-MM-dd HH:mm:ss",provider);

                //lấy danh sách các giá trị phiếu bầu đã được giải mã
                var listOfDecodedVotesBasedOnElection = await _ListOfDecodedVotesBasedOnElection(ngayBD, connection);

                //lấy danh sách ứng cử viên dựa trên ngày bắt đầu trong bảng kết quả bầu cử
                var listCandidateBasedOnElections = await _electionsRepository._GetListCandidateNamesBasedOnElections_OtherID_ucv(startTime, connection);
                
                //Trả về kết quả nếu gặp lối
                if(listOfDecodedVotesBasedOnElection is int int_result){
                    await transaction.RollbackAsync();
                    return int_result;
                }
                
                //Kiểm tra ngày bắt đầu của kỳ bầu cử công bố chưa. Nếu công bố rồi thì hủy
                bool checkResultAnnouncement = await _electionsRepository._checkResultAnnouncement(ngayBD, connection);
                if(checkResultAnnouncement){
                    await transaction.RollbackAsync();
                    return -2;
                }

                //Lấy số lượng cử tri tối đã dựa trên ngày bắt đầu
                int n = await _electionsRepository._MaximumNumberOfVoters(startTime,connection);

                //Lấy số lượng ứng cử viên tối đã dựa trên ngày bắt đầu
                int k = await _electionsRepository._MaximumNumberOfCandidates(startTime, connection);

                //Lấy số lượng bầu chọn tối đã dựa trên ngày bắt đầu
                int s = await _electionsRepository._MaximumNumberOfVotes(startTime, connection);

                //Tính B-phân
                int b = n+1;

                //Tạo danh sách số nguyên với dựa trên số lượng ứng cử viên
                List<int> BallotPaper = Enumerable.Repeat(0, k).ToList();

                //Tính toán từng phiếu bầu để tìm số lượt bình chọn cho từng ứng củ viên
                foreach(var vote in listOfDecodedVotesBasedOnElection){
                    //_log.Error($"Giá trị phiếu bầu: {vote.GiaTriPhieuBau}");
                    BigInteger vote_mi = (BigInteger)vote.GiaTriPhieuBau;
                    int s_temp = s;

                    //Dựa trên từng số lần bỏ phiếu, tìm ứng viên được bầu
                    while(s_temp >=0){

                        BigInteger power_b_s = BigInteger.Pow(b,s_temp); //Math.Pow(b, s_temp);  // Tính toán b^s_temp một lần
                        if(vote_mi >= power_b_s){
                            vote_mi -= (BigInteger)power_b_s;      // Trừ giá trị vote
                            BallotPaper[s_temp]++;          // Tăng số lần bình chọn cho ứng viên

                            // Nếu giá trị còn lại là 1, thì cộng vào vị trí đầu tiên
                            if(vote_mi == 1){
                                BallotPaper[0]++;
                                break;
                            }
                        } 
                        s_temp--;
                    }
                }
                //Tổng các giá trị phiếu
                int TotalVotes = BallotPaper.Sum(item => (int)item);

                //Tính tỉ lệ phần trăm
                float scale = 100.0f/(float)TotalVotes;
                List<float> VotingScale = new List<float>();
                foreach(var e in BallotPaper){
                    float temp = (float)scale*e;
                    VotingScale.Add(temp);
                }
 
                //Cập nhật thông tin số lượt bình chọn và tỷ lệ bình chọn cho từng ứng cử viên dựa trên ngày bầu cử
                const string sql_updateResultElection = @"
                UPDATE ketquabaucu
                SET SoLuotBinhChon =@SoLuotBinhChon ,TyLeBinhChon =@TyLeBinhChon 
                WHERE ngayBD =@ngayBD AND ID_ucv =@ID_ucv;";

                //Tìm kiếm ứng cử viên có lượt bình chọn nhiều nhất
                int SoLuotBinhChonCaoNhat = -1;
                String MaUngCuVienDacCu = "";

                foreach (var item in listCandidateBasedOnElections)
                {
                    string ID_ucv = item.ID_ucv;
                    int SoLuotBinhChon = BallotPaper[listCandidateBasedOnElections.IndexOf(item)];
                    float TyLeBinhChon = VotingScale[listCandidateBasedOnElections.IndexOf(item)];

                    //Tìm ứng cử viên có số lượt bình chọn cao nhất
                    if(SoLuotBinhChonCaoNhat <= SoLuotBinhChon){
                        SoLuotBinhChonCaoNhat = SoLuotBinhChon;
                        MaUngCuVienDacCu = ID_ucv;
                    }

                    //Cập nhật số lượt bình chọn và tỷ lệ bình chọn cho từng ứng cử viên
                    using(var command = new MySqlCommand(sql_updateResultElection, connection)){
                        command.Parameters.AddWithValue("@SoLuotBinhChon", SoLuotBinhChon);
                        command.Parameters.AddWithValue("@TyLeBinhChon", TyLeBinhChon);
                        command.Parameters.AddWithValue("@ngayBD", ngayBD);
                        command.Parameters.AddWithValue("@ID_ucv", ID_ucv);
                        
                        await command.ExecuteNonQueryAsync();
                    }
                } 

                //Cập nhật công bố kỳ bầu cử đã công bố
                bool checkUpdateResultAnnouncement = await _electionsRepository._UpdateResultAnnouncementElectionBasedOnElectionDate(startTime, connection);
                if(!checkUpdateResultAnnouncement){
                    await transaction.RollbackAsync();
                    return -3;
                }

                //Lưu thông tin cán bộ công bố kết quả bầu cử
                bool checkAddCadrePublicizations = await _resultsAnnouncementDetails._CadrePublicizeResult(ID_CanBo, MaUngCuVienDacCu, ngayBD, connection);
                if(!checkAddCadrePublicizations){
                    await transaction.RollbackAsync();
                    return -4;
                }
                
                //Thông báo kết quả bầu cử
                await _notificationHubs._announceElectionResult(ngayBD, connection);

                //Gửi thông báo kết quả liên quan đến người dùng qua email
                
                
                await transaction.CommitAsync();
                _log.Info("\t\t ====> công bố thành công");
                return 1;
            }catch(MySqlException ex){
                _log.Error($"Error message: {ex.Message}");
                _log.Error($"Error Code: {ex.Code}");
                _log.Error($"Error Source: {ex.Source}");
                _log.Error($"Error HResult: {ex.HResult}");
                await transaction.RollbackAsync();
                return -98;
            }
            catch(Exception ex){
                _log.Error($"Error message: {ex.Message}");
                _log.Error($"Error Source: {ex.Source}");
                _log.Error($"Error StackTrace: {ex.StackTrace}");
                _log.Error($"Error TargetSite: {ex.TargetSite}");
                _log.Error($"Error HResult: {ex.HResult}");
                _log.Error($"Error InnerException: {ex.InnerException}");
                await transaction.RollbackAsync();
                return -99;
            }
        }
    }
}