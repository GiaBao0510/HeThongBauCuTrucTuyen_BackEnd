using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.src.infrastructure.DataAccess.Repositories
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetailNoticeController: ControllerBase
    {
        private readonly IDetailNoticeRepository _detailNoticeRepository;
        public DetailNoticeController(IDetailNoticeRepository detailNoticeRepository) => _detailNoticeRepository = detailNoticeRepository;

        //Lấy danh sách thông báo cho cử tri
        [HttpGet("get-voter-notification-list")]
        [Authorize(Roles= "1,5")]
        public async Task<IActionResult> GetVoterNotificationList([FromQuery] string ID_CuTri){
            try{
                if(string.IsNullOrEmpty(ID_CuTri) || string.IsNullOrEmpty(ID_CuTri))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền ID cử tri."});
                
                var result = await _detailNoticeRepository._getVoterNotificationList(ID_CuTri);
                if(result == null)
                    return BadRequest(new{Status = "False", Message = "ID cử tri không tồn tại"});
                
                return Ok(new{Status = true, Message = "", data = result});

            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy danh sách thông báo về cử tri: {ex.Message}"
                });
            }
        }

        //Lấy danh sách thông báo cho ứng cử viên
        [HttpGet("get-candidate-notification-list")]
        [Authorize(Roles= "1,2")]
        public async Task<IActionResult> GetCandidateNotificationList([FromQuery] string ID_ucv){
            try{
                if(string.IsNullOrEmpty(ID_ucv) || string.IsNullOrEmpty(ID_ucv))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền ID ứng cử viên."});
                
                var result = await _detailNoticeRepository._getCandidateNotificationList(ID_ucv);
                if(result == null)
                    return BadRequest(new{Status = "False", Message = "ID ứng cử viên không tồn tại"});
                
                return Ok(new{Status = true, Message = "", data = result});

            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy danh sách thông báo về ứng cử viên: {ex.Message}"
                });
            }
        }

        //Lấy danh sách thông báo cho cán bộ
        [HttpGet("get-cadre-notification-list")]
        [Authorize(Roles= "1,8")]
        public async Task<IActionResult> GetCadreNotificationList([FromQuery] string ID_CanBo){
            try{
                if(string.IsNullOrEmpty(ID_CanBo) || string.IsNullOrEmpty(ID_CanBo))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền ID cán bộ."});
                
                var result = await _detailNoticeRepository._getCadreNotificationList(ID_CanBo);
                if(result == null)
                    return BadRequest(new{Status = "False", Message = "ID cán bộ không tồn tại"});
                
                return Ok(new{Status = true, Message = "", data = result});

            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy danh sách thông báo về cán bộ: {ex.Message}"
                });
            }
        }
    }
}