
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.web_api.DTOs;
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

        //GIải mã các phiếu dựa trên thời điểm kỳ bầu cử
        [HttpGet("get-list-of-decoded-votes-based-on-election-date")]
        [Authorize(Roles= "1,8")]
        public async Task<IActionResult> getListOfDecodedVotesBasedOnElection([FromQuery] string ngayBD){
            try{
                if(string.IsNullOrEmpty(ngayBD) || string.IsNullOrEmpty(ngayBD))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền ngày bắt đầu."});
                
                var result = await _lockRepository._ListOfDecodedVotesBasedOnElection(ngayBD);
                
                if(result is int int_result ){
                    int status = int_result switch{
                        0 => 400, -1 => 400, _ => 500
                    };
                    string errorMessage = int_result switch{
                        0 => "Không tìm thấy thời điểm bầu cử",
                        -1 => "Không tìm thấy nơi lưu trữ khóa mật",
                       _ =>"Lỗi không xác định"
                    };

                    return StatusCode(status ,new{Status = "False", Message = errorMessage}); 
                }
                    
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

        //Công bố kết quả
        [HttpPut("announce-election-results")]
        [Authorize(Roles= "1,8")]
        public async Task<IActionResult> CaculateAndAnnounceElectionResult([FromBody] CadreResultAnnouncementDTO input){
            try{
                if(string.IsNullOrEmpty(input.ID_CanBo) || string.IsNullOrEmpty(input.ngayBD))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền ngày bắt đầu."});
                
                var result = await _lockRepository._CaculateAndAnnounceElectionResult(input.ngayBD, input.ID_CanBo);
                
                if(result <= 0){
                    int status = result switch{
                        0 => 400, -1 => 400, -2 => 400, -3 => 400, _ => 500
                    };
                    string errorMessage = result switch{
                        0 => "Lỗi không tìm thấy ngày bắt đầu bầu cử",
                        -1 => "Lỗi không tìm thấy nơi lưu trữ khóa mật",
                        -2 => "Lỗi kỳ bầu cử đã công bố rồi",
                       _ =>"Lỗi không xác định"
                    };
                    return StatusCode(status ,new{Status = "False", Message = errorMessage}); 
                }
                    
                return Ok(new{Status = true, Message = $"Đã thực hiện công bố kết quả bầu cử dự trên thời điểm {input.ngayBD}", data =""});

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