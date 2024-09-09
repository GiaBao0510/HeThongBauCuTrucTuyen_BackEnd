using BackEnd.src.infrastructure.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using BackEnd.core.Entities;
using BackEnd.src.infrastructure.DataAccess.IRepository;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationRepository _notificationsReposistory;

        //Khởi tạo
        public NotificationsController(INotificationRepository notificationsReposistory) => _notificationsReposistory = notificationsReposistory;

        //Liệt kê
        [HttpGet]
        public async Task<IActionResult> GetListOfNotifications(){
            try{
                var result = await _notificationsReposistory._GetListOfNotifications();

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = result
                });
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi truy xuất danh sách các thông báo: {ex.Message}"
                });
            }
        }

        //Thêm
        [HttpPost]
        public async Task<IActionResult> CreateNotifications([FromBody] Notifications Notifications){
            try{
                //Kiểm tra đầu vào
                if(Notifications == null || string.IsNullOrEmpty(Notifications.NoiDungThongBao))
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi khi đầu vào không được rỗng"
                    });
                
                //lấy kết quả thêm vào được hay không
                var result = await _notificationsReposistory._AddNotifications(Notifications);
                if(result == false)
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi ID_ThongBao thông báo đã bị trùng"
                    });
                
                return Ok(new{
                    Status = "OK", 
                    Message = "Thêm thông báo thành công"
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm thông báo: {ex.Message}"
                });
            }
        }
    
        //Lấy theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotificationsBy_ID(string id){
            try{
                var Notifications = await _notificationsReposistory._GetNotificationsBy_ID(id);
                if(Notifications == null)
                    return StatusCode(400, new{
                    Status = "False", 
                    Message = $"Lỗi ID_ThongBao của thông báo không tồn tại"
                });

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = Notifications
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm thông báo: {ex.Message}"
                });
            }
        }

        //Sửa
        [HttpPut("{id}")]
        public async Task<IActionResult> EditNotificationsBy_ID(string id, Notifications Notifications){
            try{
                if(Notifications == null || string.IsNullOrEmpty(Notifications.NoiDungThongBao))
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không được để trống"
                    });

                var result = await _notificationsReposistory._EditNotificationsBy_ID(id, Notifications);
                if(result == false)
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không hợp lệ"
                    });

                return Ok(new{
                    Status = "Ok",
                    Message = "Cập nhật thành công"
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện sửa thông báo: {ex.Message}"
                });
            }
        }

        //xóa
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotificationsBy_ID(string id){
            try{
                var result = await _notificationsReposistory._DeleteNotificationsBy_ID(id);
                if(result == false)
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không hợp lệ"
                    });

                return Ok(new{
                    Status = "Ok",
                    Message = "Xóa thành công"
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện xóa thông báo: {ex.Message}"
                });
            }
        }

    }
}