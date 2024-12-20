using System.Globalization;
using BackEnd.src.core.Interfaces;
using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.web_api.DTOs;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using MySql.Data.MySqlClient;
using log4net;

namespace BackEnd.src.infrastructure.Hubs
{
    public class NotificationHubs : Hub, IDisposable, INotificationHubs
    {
        private readonly DatabaseContext _context;
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));
        private readonly IHubContext<NotificationHubs> _hubContext;
        private readonly IVoterRepository _voterRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly ICadreRepository _cadreRepository;
        public NotificationHubs(
            DatabaseContext context,
            IHubContext<NotificationHubs> hubContext,
            IVoterRepository voterRepository,
            ICandidateRepository candidateRepository,
            ICadreRepository cadreRepository
        )
        {
            _context = context;
            _hubContext = hubContext;
            _voterRepository = voterRepository;
            _candidateRepository = candidateRepository;
            _cadreRepository = cadreRepository;
        }

        //Hủy
        public new void Dispose() => _context.Dispose(); 

        //0. Lấy ID thông báo cuối cùng trong bảng
        public async Task<int> _getLastestNoticeID(MySqlConnection connect){
            
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connect.State != System.Data.ConnectionState.Open)
                await connect.OpenAsync();

            const string sql = @"SELECT ID_ThongBao FROM thongbao ORDER BY ID_ThongBao DESC LIMIT 1;";
            int ID_thongBao = -1;
            using(var command = new MySqlCommand(sql, connect)){
                
                using var reader = await command.ExecuteReaderAsync();
                if(await reader.ReadAsync())
                    ID_thongBao =  reader.GetInt32(reader.GetOrdinal("ID_ThongBao"));
            }
            return ID_thongBao;
        }

        //0.1 Tạo thông báo (có kiểu trả về)
        public async Task<bool> _createNotice(string content, MySqlConnection connect){
            try{
                //Kiểm tra trạng thái kết nối trước khi mở
                if(connect.State != System.Data.ConnectionState.Open)
                    await connect.OpenAsync();
                
                if(string.IsNullOrEmpty(content)) return false; //Nội dung trống thì báo lỗi

                const string sql = @"
                INSERT INTO thongbao(NoiDungThongBao,ThoiDiem)
                VALUES(@NoiDungThongBao,@ThoiDiem);";
                using(var command = new MySqlCommand(sql, connect)){
                    command.Parameters.AddWithValue("@NoiDungThongBao", content);
                    command.Parameters.AddWithValue("@ThoiDiem", DateTime.Now);
                    
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
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

        //0.3 Lưu thông báo chi tiết
        public async Task SaveNotificationDetail(string tableName, string userIdColumn, string userId, int noticeId, MySqlConnection connection)
        {
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            string sql = $@"
            INSERT INTO {tableName}(ID_ThongBao, {userIdColumn}) 
            VALUES(@ID_ThongBao, @UserId);";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@ID_ThongBao", noticeId);
                command.Parameters.AddWithValue("@UserId", userId);

                await command.ExecuteNonQueryAsync();
            }
        }

        //1.lấy thông tin ngày bầu cử và ID cử tri trong bảng trang thái
        public async Task<List<VoterNoticeDetailsDTO>> _getElectionInformationToVoters(MySqlConnection connect){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connect.State != System.Data.ConnectionState.Open)
                await connect.OpenAsync();
            
            var list = new List<VoterNoticeDetailsDTO>();
            try{
                const string sql = @"SELECT ID_CuTri,ngayBD FROM trangthaibaucu WHERE ID_CuTri IS NOT NULL;";
                using(var command = new MySqlCommand(sql, connect)){
                    using var reader = await command.ExecuteReaderAsync();
                    while(await reader.ReadAsync()){
                        list.Add(new VoterNoticeDetailsDTO{
                            ID_CuTri = reader.GetString(reader.GetOrdinal("ID_CuTri")),
                            ngayBD = reader.GetDateTime(reader.GetOrdinal("ngayBD")).ToString()
                        });
                    }
                }
                return list;
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

        //2.lấy thông tin ngày bầu cử và ID ứng cử viên trong bảng kết quả bầu cử
        public async Task<List<CandidateNoticeDetailsDTO>> _getElectionInformationToCandidates(MySqlConnection connect){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connect.State != System.Data.ConnectionState.Open)
                await connect.OpenAsync();
            
            var list = new List<CandidateNoticeDetailsDTO>();
            try{
                const string sql = @"SELECT ID_ucv,ngayBD FROM ketquabaucu; ";
                using(var command = new MySqlCommand(sql, connect)){
                    using var reader = await command.ExecuteReaderAsync();
                    while(await reader.ReadAsync()){
                        list.Add(new CandidateNoticeDetailsDTO{
                            ID_ucv = reader.GetString(reader.GetOrdinal("ID_ucv")),
                            ngayBD = reader.GetDateTime(reader.GetOrdinal("ngayBD")).ToString(),
                        });
                    }
                }
                return list;
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

        //3.lấy thông tin ngày bầu cử và ID ứng cử viên trong bảng hoạt động
        public async Task<List<CadreNoticeDetailsDTO>> _getElectionInformationToCandres(MySqlConnection connect){
            //Kiểm tra trạng thái kết nối trước khi mở
            if(connect.State != System.Data.ConnectionState.Open)
                await connect.OpenAsync();
            
            var list = new List<CadreNoticeDetailsDTO>();
            try{
                const string sql = @"SELECT ID_canbo,ngayBD FROM hoatdong; ";
                using(var command = new MySqlCommand(sql, connect)){
                    using var reader = await command.ExecuteReaderAsync();
                    while(await reader.ReadAsync()){
                        list.Add(new CadreNoticeDetailsDTO{
                            ID_CanBo = reader.GetString(reader.GetOrdinal("ID_CanBo")),
                            ngayBD = reader.GetDateTime(reader.GetOrdinal("ngayBD")).ToString()
                        });
                    }
                }
                return list;
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

        //4.Dò trong danh sách các ngày bắt đầu bầu cử của cử tri đẫ tham dự sẽ thông báo đó trước một ngày
        public async Task _announceUpcomingElectionDayToVoter(){
            using var connection = await _context.Get_MySqlConnection();
            
            try{
                var listVoterDays = await _getElectionInformationToVoters(connection);
                foreach(var e in listVoterDays){
                    //Lấy thông tin ngày bắt đầu bầu cử. Nếu trước một ngày thì sẽ thông báo
                    CultureInfo culture = new CultureInfo("vi-VN");
                    DateTime upcomingDay = Convert.ToDateTime(e.ngayBD, culture);
                    DateTime currentDay = DateTime.Now;
                    TimeSpan difference = upcomingDay - currentDay;

                    if(difference.Days == 1){
                        _log.Info($">>>>>Thông báo ID cử tri: {e.ID_CuTri} Ngày bắt đầu bầu cử: {e.ngayBD}");
                        //Chuỗi thông báo
                        string message = $"Ngày bầu cử sắp tới của bạn tham dự là {e.ngayBD}.";
                        
                        //Tạo nội dung thông báo
                        bool createAnnouce = await _createNotice(message, connection);

                        //Lấy id cuối cùng thông báo
                        int ID_notice = await _getLastestNoticeID(connection);

                        // Lưu vào bảng thông báo chi tiết cử tri
                        await SaveNotificationDetail("chitietthongbaocutri", "ID_CuTri", e.ID_CuTri, ID_notice, connection);

                        // Gửi thông báo qua SignalR
                        await _hubContext.Clients.All.SendAsync("Kỳ bầu cử sắp diến ra", message);
                    }
                }
            }catch (NullReferenceException nre)
            {
                _log.Error($"Null reference error: {nre.Message}");
                // Xử lý cụ thể cho lỗi null
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

        //5.Dò trong danh sách các ngày bắt đầu bầu cử của ứng cử viên đẫ tham dự sẽ thông báo đó trước một ngày
        public async Task _announceUpcomingElectionDayToCandidate(){
            using var connection = await _context.Get_MySqlConnection();
            
            try{
                var listVoterDays = await _getElectionInformationToCandidates(connection);
                foreach(var e in listVoterDays){
                    
                    //Lấy thông tin ngày bắt đầu bầu cử. Nếu trước một ngày thì sẽ thông báo
                    CultureInfo culture = new CultureInfo("vi-VN");
                    DateTime upcomingDay = Convert.ToDateTime(e.ngayBD, culture);
                    DateTime currentDay = DateTime.Now;
                    TimeSpan difference = upcomingDay - currentDay;

                    if(difference.Days == 1){
                        _log.Info($">>>>>Thông báo ID ứng cử viên: {e.ID_ucv} Ngày bắt đầu bầu cử: {e.ngayBD}");
                        //Chuỗi thông báo
                        string message = $"Ngày bầu cử sắp tới của bạn là {upcomingDay}.";
                        
                        //Tạo nội dung thông báo
                        bool createAnnouce = await _createNotice(message, connection);

                        //Lấy id cuối cùng thông báo
                        int ID_notice = await _getLastestNoticeID(connection);

                        //Lưu vào bảng thông báo chi tiết ứng cử viên
                        await SaveNotificationDetail("chitietthongbaoungcuvien", "ID_ucv", e.ID_ucv, ID_notice, connection);

                     
                        await _hubContext.Clients.All.SendAsync($"Kỳ bầu cử sắp diến ra: {message}");
                    }
                }
            }catch (NullReferenceException nre)
            {
                _log.Error($"Null reference error: {nre.Message}");
                // Xử lý cụ thể cho lỗi null
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

        //6.Dò trong danh sách các ngày bắt đầu bầu cử của cán bộ đẫ tham dự sẽ thông báo đó trước một ngày
        public async Task _announceUpcomingElectionDayToCandre(){
            using var connection = await _context.Get_MySqlConnection();
            
            try{
                var listVoterDays = await _getElectionInformationToCandres(connection);
                foreach(var e in listVoterDays){

                    //Lấy thông tin ngày bắt đầu bầu cử. Nếu trước một ngày thì sẽ thông báo
                    CultureInfo culture = new CultureInfo("vi-VN");
                    DateTime upcomingDay = Convert.ToDateTime(e.ngayBD, culture);
                    DateTime currentDay = DateTime.Now;
                    TimeSpan difference = upcomingDay - currentDay;

                    if(difference.Days == 1){
                        _log.Info($">>>>>Thông báo ID cử tri: {e.ID_CanBo} Ngày bắt đầu bầu cử: {e.ngayBD}");
                        //Chuỗi thông báo
                        string message = $"Ngày bầu cử sắp tới của bạn là {e.ngayBD}.";
                        
                        //Tạo nội dung thông báo
                        bool createAnnouce = await _createNotice(message, connection);

                        //Lấy id cuối cùng thông báo
                        int ID_notice = await _getLastestNoticeID(connection);

                        //Lưu vào bảng thông báo chi tiết ứng cử viên
                        await SaveNotificationDetail("chitietthongbaocanbo", "ID_CanBo", e.ID_CanBo, ID_notice, connection);

                        await _hubContext.Clients.All.SendAsync("Kỳ bầu cử sắp diến ra", message);
                    }
                }
            }catch (NullReferenceException nre)
            {
                _log.Error($"Null reference error: {nre.Message}");
                // Xử lý cụ thể cho lỗi null
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

        //Thông báo kết quả bầu cử cho các đối tượng tham giá dựa trên ngày bắt đầu
        public async Task _announceElectionResult(string ngayBD, MySqlConnection connect){
            try{
                //Kiểm tra trạng thái kết nối trước khi mở
                if(connect.State != System.Data.ConnectionState.Open)
                    await connect.OpenAsync();

                //1.Tạo thông báo kết quả bầu cử
                string message = $"Thông báo kết quả bầu cử tại kỳ: {ngayBD}";
                bool createAnnouce = await _createNotice(message, connect);

                //2. Lấy ID cuối cùng của bảng thông báo
                int ID_notice = await _getLastestNoticeID(connect);

                //Lấy danh sách ID các đối tượng dựa trên ngày BD
                List<VoterID_DTO> listVoterID = await _voterRepository._getVoterID_ListBasedOnElection(ngayBD, connect);
                List<CandidateID_DTO> listCandidateID = await _candidateRepository._getCandidateID_ListBasedOnElection(ngayBD, connect);
                List<CadreID_DTO> listCadreID = await _cadreRepository._getCadreID_ListBasedOnElection(ngayBD, connect); 
                
                //Thông báo cho ứng cử viên
                foreach(var e in listCandidateID){
                    await SaveNotificationDetail("chitietthongbaoungcuvien", "ID_ucv", e.ID_ucv, ID_notice, connect);
                }
                //Thông báo cho cử tri
                foreach(var e in listVoterID){
                    await SaveNotificationDetail("chitietthongbaocutri", "ID_CuTri", e.ID_CuTri, ID_notice, connect);
                }
                //Thông báo cho cán bộ
                foreach(var e in listCadreID){
                    await SaveNotificationDetail("chitietthongbaocanbo", "ID_CanBo", e.ID_CanBo, ID_notice, connect);
                }
                // Gửi thông báo qua SignalR cho tất cả các đối tượng
                await _hubContext.Clients.All.SendAsync("Kết quả bầu cử tại kỳ", message);
            }catch (NullReferenceException nre)
            {
                _log.Error($"Null reference error: {nre.Message}");// Xử lý cụ thể cho lỗi null
                throw new Exception("Null reference error");
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

        //Xem thông báo dựa trên ID đối tượng
        public async Task<List<ViewNotificationBasedOnObjectsDTO>> NotificationViewer(string bangChiTiet, string ID_ObColumn, string ID_ob,  MySqlConnection connect){
            try{
                //Kiểm tra trạng thái kết nối trước khi mở
                if(connect.State != System.Data.ConnectionState.Open)
                    await connect.OpenAsync();
                
                var list = new List<ViewNotificationBasedOnObjectsDTO>();
                string sql = $@"
                SELECT tb.NoiDungThongBao, tb.ThoiDiem
                FROM {bangChiTiet} ct
                JOIN thongbao tb ON ct.ID_ThongBao = tb.ID_ThongBao
                WHERE ct.{ID_ObColumn} = @ID_Object;";

                using(var command = new MySqlCommand(sql, connect)){
                    command.Parameters.AddWithValue("@ID_Object", ID_ob);
                    using var reader = await command.ExecuteReaderAsync();

                    while(await reader.ReadAsync()){
                        list.Add(new ViewNotificationBasedOnObjectsDTO{
                            NoiDungThongBao = reader.GetString(reader.GetOrdinal("NoiDungThongBao")),
                            ThoiDiem =reader.GetDateTime(reader.GetOrdinal("ThoiDiem")),
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

    }
}