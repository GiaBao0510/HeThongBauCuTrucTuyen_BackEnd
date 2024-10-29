using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.infrastructure.Hubs;
using BackEnd.src.web_api.DTOs;
using MySql.Data.MySqlClient;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    public class DetailNoticeRepository : IDisposable, IDetailNoticeRepository
    {
        private readonly DatabaseContext _context;
        private readonly NotificationHubs _notificationHubs;
        private readonly IVoterRepository _voterRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly ICadreRepository _cadreRepository;
        
        //Khởi tạo
        public DetailNoticeRepository(
            DatabaseContext context,
            NotificationHubs notificationHubs,
            IVoterRepository voterRepository,
            ICandidateRepository candidateRepository,
            ICadreRepository cadreRepository
        ){
            _context = context;
            _notificationHubs = notificationHubs;
            _voterRepository = voterRepository;
            _candidateRepository = candidateRepository;
            _cadreRepository = cadreRepository;
        }
        
        //Hàm hủy
        public void Dispose() => _context.Dispose();

        //1. Lấy danh sách thông báo cho cử tri
        public async Task<List<ViewNotificationBasedOnObjectsDTO>> _getVoterNotificationList(string ID_CuTri){
            try{

                using var connection = await _context.Get_MySqlConnection();
                //Tìm xem cử tri có tồn tại không
                bool checkVoterExit = await _voterRepository._CheckVoterExists(ID_CuTri,connection);
                if(!checkVoterExit)
                    return null;

                return await _notificationHubs.NotificationViewer("chitietthongbaocutri","ID_CuTri",ID_CuTri,connection);
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

        //2. Lấy danh sách thông báo cho cán bộ
        public async Task<List<ViewNotificationBasedOnObjectsDTO>> _getCadreNotificationList(string ID_CanBo){
            try{

                using var connection = await _context.Get_MySqlConnection();
                //Tìm xem cử tri có tồn tại không
                bool checkVoterExit = await _cadreRepository._CheckCadreExists(ID_CanBo,connection);
                if(!checkVoterExit)
                    return null;

                return await _notificationHubs.NotificationViewer("chitietthongbaocanbo","ID_CanBo",ID_CanBo,connection);
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

        //3. Lấy danh sách thông báo cho ứng cử viên
        public async Task<List<ViewNotificationBasedOnObjectsDTO>> _getCandidateNotificationList(string ID_ucv){
            try{

                using var connection = await _context.Get_MySqlConnection();
                //Tìm xem cử tri có tồn tại không
                bool checkVoterExit = await _candidateRepository._CheckCandidateExists(ID_ucv,connection);
                if(!checkVoterExit)
                    return null;

                return await _notificationHubs.NotificationViewer("chitietthongbaoungcuvien","ID_ucv",ID_ucv,connection);
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