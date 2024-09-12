using Microsoft.AspNetCore.Mvc;
using BackEnd.src.infrastructure.DataAccess.Repositories;
using BackEnd.src.web_api.DTOs;
using Microsoft.AspNetCore.Http;
using BackEnd.src.infrastructure.DataAccess.IRepository;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoterController: ControllerBase
    {
        private readonly IVoterRepository _voterReposistory;

        //Khởi tạo
        public VoterController(IVoterRepository vouterReposistory) => _voterReposistory = vouterReposistory;

        //1.Thêm
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateVouter([FromForm] VoterDto vouter,  IFormFile fileAnh){
            try{
                //Kiểm tra đầu vào
                if(vouter == null || string.IsNullOrEmpty(vouter.HoTen))
                    return StatusCode(400,new{
                        Status = "false",
                        Message="Lỗi khi đầu vào không được rỗng"
                    });
                
                //lấy kết quả thêm vào được hay không
                var result = await _voterReposistory._AddVoter(vouter, fileAnh);
                if(result <= 0){
                    string errorMessage = result switch{
                        0 => "Số điện thoại đã bị trùng",
                        -1 =>"Email thoại đã bị trùng",
                        -2 => "Căn cước công dân thoại đã bị trùng",
                        -3 =>"Vai trò người dùng không tồn tại",
                        -4 =>"Đầu vào không hợp lệ hoặc bị để trống trường nào đó",
                        _ => "Lỗi không xác định"
                    };
                    return BadRequest(new {Status = "False", Message = errorMessage});
                }
                
                return Ok(new{
                    Status = "OK", 
                    Message = "Thêm tài khoản cử tri thành công"
                });
            }catch(Exception ex){
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm tài khoản cử tri: {ex.Message}"
                });
            }
        }

        //2.Lấy all cử tri
        [HttpGet]
        [Route("allVoter")]
        public async Task<IActionResult> GetListOfVouter(){
            try{
                var result = await _voterReposistory._GetListOfVoter();

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = result
                });
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi truy xuất danh sách các cử tri: {ex.Message}"
                });
            }
        }

        //3.Lấy all cử tri - account
        [HttpGet]
        [Route("allVoterAndAccount")]
        public async Task<IActionResult> GetListOfVotersAndAccounts(){
            try{
                var result = await _voterReposistory._GetListOfVotersAndAccounts();

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = result
                });
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi truy xuất danh sách các cử tri - tài khoản: {ex.Message}"
                });
            }
        }

        //4.Lấy theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVoterBy_ID(string id){
            try{
                var District = await _voterReposistory._GetVoterBy_ID(id);
                if(District == null)
                    return StatusCode(400, new{
                    Status = "False", 
                    Message = $"Lỗi ID của cử tri không tồn tại"
                });

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = District
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy thông tin cử tri theo ID: {ex.Message}"
                });
            }
        }

        //5.Sửa
        [HttpPut("{id}")]
        public async Task<IActionResult> EditVoterBy_ID(string id,[FromBody] VoterDto cutri){
            try{
                if(cutri == null || string.IsNullOrEmpty(cutri.HoTen))
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không được để trống"
                    });

                var result = await _voterReposistory._EditVoterBy_ID(id, cutri);
                int status = 200;
                if(result <= 0){
                    string errorMessage = result switch{
                        0 =>"Đã trùng số điện thoại",
                        -1 => "Đã trùng Email",
                        -2 =>"Đã trùng CCCD",
                        -3 => "Vai trò người dùng không tìm thấy",
                        -4 =>"Đầu vào không hợp lệ hoặc bị để trống trường nào đó",
                        -5 => "Không tìm thấy ID_usr từ cử tri",
                        -6 => "Lỗi khi thực hiện cập nhật tài khoản cử tri",
                        -7 => "Mã lỗi trùng lặp",
                        -8 => "Mã lỗi SQL khác",
                        -9 => "Mã lỗi cho các exception",
                        _ => "Lỗi không xác định"
                    };

                    status = result switch{
                        0 =>400, -1 => 400, -2 =>400, -3 => 400,
                        -4 =>400, -5 => 500, -6 => 500, -7 => 400,
                        -8 => 500, -9 => 500,_ => 500
                    };

                    return StatusCode(status ,new {Status = "False", Message = errorMessage});
                }

                return Ok(new{
                    Status = "Ok",
                    Message = "Cập nhật thành công",
                    Data = cutri
                });
            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện sửa thông tin cử tri: {ex.Message}"
                });
            }
        }

        //6.xóa
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVoterBy_ID(string id){
            try{
                var result = await _voterReposistory._DeleteVoterBy_ID(id);
                if(result == false)
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi không tìm thấy ID_CuTri để xóa"
                    });

                return Ok(new{
                    Status = "Ok",
                    Message = "Xóa thành công"
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện xóa tài khoản cử tri: {ex.Message}"
                });
            }
        }

        //7. Đặt lại mật khẩu admin
        [HttpPut("SetVoterPwd/{id}")]
        public async Task<IActionResult> SetVoterPassword(string id,[FromBody] SetPasswordDto setPasswordDto){
            try{
                if(string.IsNullOrEmpty(setPasswordDto.newPwd) )
                    return BadRequest(new {Status = "False", Message = "Mật khẩu không được bỏ trống."});

                var result = await _voterReposistory._SetVoterPassword(id,setPasswordDto.newPwd);
                if(result == false)
                    return BadRequest(new{Status = false, Message = "Không tìm thấy ID cử tri để đặt lại mật khẩu mới."});
                
                return Ok(new{Status = true, Message = "Đặt lại mật khẩu mới của cử tri thành công"});
            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện đặt lại mật khẩu cử tri: {ex.Message}"
                });
            }
        }

        //8. thay đổi mật khẩu cử tri
        [HttpPut("ChangeVoterPwd/{id}")]
        public async Task<IActionResult> ChangeVoterPassword(string id,[FromBody] SetPasswordDto setPasswordDto){
            try{
                if(string.IsNullOrEmpty(setPasswordDto.newPwd) || string.IsNullOrEmpty(setPasswordDto.oldPwd) )
                    return BadRequest(new {Status = "False", Message = "Mật khẩu cũ và mật khẩu mới không được bỏ trống."});

                var result = await _voterReposistory._ChangeVoterPassword(id,setPasswordDto.oldPwd,setPasswordDto.newPwd);
                int status = 200;
                if(result <= 0){
                    string errorMessage = result switch{
                        0 => "Không tìm thấy ID cử tri",
                        -1 => "Hai mật khẩu không khớp nhau",
                        -2 => "Lỗi khi thay đổi mật khẩu",
                        _ => "Lỗi không xác định"
                    };
                    status = result switch{
                        0 => 400, -1 => 400,
                        -2 => 500, _ => 500
                    };
                    return StatusCode(status, new{ Status = "False", Message = errorMessage });
                }

                return Ok(new{Status = false, Message = "Đặt lại mật khẩu mới của cử tri thành công"});
            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện xóa tài khoản cử tri: {ex.Message}"
                });
            }
        }

        //9. Gửi báo cáo
        [HttpPost("sendReport")]
        public async Task<IActionResult> VoterSubmitReport([FromBody] SendReportDto sendReportDto){
            try{
                //Kiểm tra đầu vào
                if(string.IsNullOrEmpty(sendReportDto.YKien) || string.IsNullOrEmpty(sendReportDto.IDSender))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền ý kiến và mã người gửi."});

                var result = await _voterReposistory._VoterSubmitReport(sendReportDto);
                
                if(result == false)
                    return BadRequest(new{Status = "False", Message = "Không tìm thấy ID cử tri để gửi báo cáo."});

                return Ok(new{Status = true, Message = "Gửi báo cáo thành công"});

            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện gửi phản hồi: {ex.Message}"
                });
            }
        }

    }
}