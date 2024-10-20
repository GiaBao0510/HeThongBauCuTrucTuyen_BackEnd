
using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LockController: ControllerBase
    {
        private readonly ILockRepository _lockRepository;
        public LockController(ILockRepository lockRepository) => _lockRepository = lockRepository;
        
        //1.Lấy thông tin khóa công khai theo ngày bắt đầu
        [HttpGet("get-keyInfo-based-on-election-date")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> getLockBasedOnElectionDate([FromQuery] string ngayBD){
            try{
                if(string.IsNullOrEmpty(ngayBD) || string.IsNullOrEmpty(ngayBD))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền ngày bắt đầu."});
                
                var result = await _lockRepository._getLockBasedOnElectionDate(ngayBD);
                if(result == null)
                    return BadRequest(new{Status = "False", Message = "Ngày bắt đầu không tồn tại"});
                
                return Ok(new{Status = true, Message = "", data = result});

            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy thông tin về khóa dựa trên ngày bắt đầu: {ex.Message}"
                });
            }
        }

        //2.Lấy tất cả thông tin khóa
        [HttpGet("get-AllKeyInfo")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> getListKey(){
            try{
                var result = await _lockRepository._getListKey();
                
                return Ok(new{Status = true, Message = "", data = result});
            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy tất cả thông tin về khóa :{ex.Message}"
                });
            }
        }

        //3.Xóa khóa theo ngày bắt đầu
        [HttpDelete("delete-keyInfo-based-on-election-date")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> deleteKeyBasedOnElectionDate([FromQuery] string ngayBD){
            try{
                if(string.IsNullOrEmpty(ngayBD) || string.IsNullOrEmpty(ngayBD))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền ngày bắt đầu."});
                
                var result = await _lockRepository._deleteKeyBasedOnElectionDate(ngayBD);
                if(result == false)
                    return BadRequest(new{Status = "False", Message = "Ngày bắt đầu không tồn tại"});

                return Ok(new{Status = true, Message = "Xóa khóa thành công"});

            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện xóa thông tin về khóa dựa trên ngày bắt đầu: {ex.Message}"
                });
            }
        }

        //4.Lấy khóa thông tin khóa mật theo ngày bắt đầu
        [HttpGet("get-privatekeyInfo-based-on-election-date")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> getPrivateKeyBasedOnElectionDateAndKey([FromQuery] string ngayBD){
            try{
                if(string.IsNullOrEmpty(ngayBD) || string.IsNullOrEmpty(ngayBD))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền ngày bắt đầu."});
                
                var result = await _lockRepository._getPrivateKeyBasedOnElectionDateAndKey(ngayBD);
                if(result == null)
                    return BadRequest(new{Status = "False", Message = "Ngày bắt đầu không tồn tại"});
                
                return Ok(new{Status = true, Message = "", data = result});

            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy thông tin về khóa dựa trên ngày bắt đầu: {ex.Message}"
                });
            }
        }

        [HttpGet("get-list-of-decoded-votes-based-on-election-date")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> getListOfDecodedVotesBasedOnElection([FromQuery] string ngayBD){
            try{
                if(string.IsNullOrEmpty(ngayBD) || string.IsNullOrEmpty(ngayBD))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền ngày bắt đầu."});
                
                var result = await _lockRepository._ListOfDecodedVotesBasedOnElection(ngayBD);
                if(result == null)
                    return BadRequest(new{Status = "False", Message = "Ngày bắt đầu không tồn tại"});
                
                return Ok(new{Status = true, Message = "", data = result});

            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy thông tin về thông tin phiếu đã giải mã dựa trên kỳ bầu cử: {ex.Message}"
                });
            }
        }

    }
}