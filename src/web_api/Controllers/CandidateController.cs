using Microsoft.AspNetCore.Mvc;
using BackEnd.src.web_api.DTOs;
using BackEnd.src.infrastructure.DataAccess.Repositories;
using Microsoft.AspNetCore.Http;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateController: ControllerBase
    {
        private readonly ICandidateRepository _candidateReposistory;

        //Khởi tạo
        public CandidateController(ICandidateRepository CandidateReposistory) => _candidateReposistory = CandidateReposistory;

        //1. Thêm
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> CreateCandidate([FromForm] CandidateDto Candidate,IFormFile fileAnh){
            try{
                //Kiểm tra đầu vào
                if(Candidate == null || string.IsNullOrEmpty(Candidate.HoTen))
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi khi đầu vào không được rỗng"
                    });
                
                //lấy kết quả thêm vào được hay không
                var result = await _candidateReposistory._AddCandidate(Candidate, fileAnh);
                if(result <= 0){
                    string errorMessage = result switch{
                        0 => "Số điện thoại đã bị trùng",
                        -1 =>"Email thoại đã bị trùng",
                        -2 => "Căn cước công dân thoại đã bị trùng",
                        -3 =>"Vai trò người dùng không tồn tại",
                        -4 =>"Đầu vào không hợp lệ hoặc bị để trống trường nào đó",
                        -5 => "Mã Danh mục ứng cử không tồn tại",
                        -6 => "Ngày bắt bầu cử không tìm thấy",
                        _ => "Lỗi không xác định"
                    };
                    return BadRequest(new {Status = "False", Message = errorMessage});
                }
                    
                return Ok(new{
                    Status = "OK", 
                    Message = "Thêm tài khoản ứng cử viên thành công"
                });
            }catch(Exception ex){
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm tài khoản ứng cử viên: {ex.Message}"
                });
            }
        }

        //2. Lấy all ứng cử viên
        [HttpGet]
        [Route("allCandidate")]
        [Authorize]
        public async Task<IActionResult> GetListOfCandidate(){
            try{
                var result = await _candidateReposistory._GetListOfCandidate();

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = result
                });
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi truy xuất danh sách các ứng cử viên: {ex.Message}"
                });
            }
        }

        //Lấy all ứng cử viên - account
        [HttpGet]
        [Route("allCandidateAndAccount")]
        [Authorize]
        public async Task<IActionResult> GetListOfCandidateAndAccount(){
            try{
                var result = await _candidateReposistory._GetListOfCandidatesAndAccounts();

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = result
                });
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi truy xuất danh sách các ứng cử viên: {ex.Message}"
                });
            }
        }

        //4. Lấy theo ID
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCandidateBy_ID(string id){
            try{
                var District = await _candidateReposistory._GetCandidateBy_ID(id);
                if(District == null)
                    return StatusCode(400, new{
                    Status = "False", 
                    Message = $"Lỗi ID_QH của ứng cử viên không tồn tại"
                });

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = District
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy thông tin ứng cử viên theo ID: {ex.Message}"
                });
            }
        }

        //5.Sửa
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> EditCandidateBy_ID(string id,[FromBody] CandidateDto UngCuVien){
            try{
                if(UngCuVien == null || string.IsNullOrEmpty(UngCuVien.HoTen))
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không được để trống"
                    });

                var result = await _candidateReposistory._EditCandidateBy_ID(id, UngCuVien);
                int status = 200;
                if(result <= 0){
                    string errorMessage = result switch{
                        0 =>"Đã trùng số điện thoại",
                        -1 => "Đã trùng Email",
                        -2 =>"Đã trùng CCCD",
                        -3 => "Vai trò người dùng không tìm thấy",
                        -4 =>"Đầu vào không hợp lệ hoặc bị để trống trường nào đó",
                        -5 => "Không tìm thấy ID_usr từ ứng cử viên",
                        -6 => "Lỗi khi thực hiện cập nhật tài khoản ứng cử viên",
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
                    Data = UngCuVien
                });
            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện sửa thông tin ứng cử viên: {ex.Message}"
                });
            }
        }
        
        //6.xóa
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCandidateBy_ID(string id){
            try{
                var result = await _candidateReposistory._DeleteCandidateBy_ID(id);
                if(result == false)
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi không tìm thấy ID_UngCuVien để xóa"
                    });

                return Ok(new{
                    Status = "Ok",
                    Message = "Xóa thành công"
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện xóa tài khoản ứng cử viên: {ex.Message}"
                });
            }
        }

        //7. Đặt lại mật khẩu - admin
        [HttpPut("SetCandidatePwd/{id}")]
        [Authorize]
        public async Task<IActionResult> SetCandidatePassword(string id,[FromBody] SetPasswordDto setPasswordDto){
            try{
                if(string.IsNullOrEmpty(setPasswordDto.newPwd) )
                    return BadRequest(new {Status = "False", Message = "Mật khẩu không được bỏ trống."});

                var result = await _candidateReposistory._SetCandidatePassword(id,setPasswordDto.newPwd);
                if(result == false)
                    return BadRequest(new{Status = false, Message = "Không tìm thấy ID ứng cử viên để đặt lại mật khẩu mới."});
                
                return Ok(new{Status = true, Message = "Đặt lại mật khẩu mới của ứng cử viên thành công"});
            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện đặt lại mật khẩu ứng cử viên: {ex.Message}"
                });
            }
        }

        //8. Thay đổi mật khẩu - ứng cử viên
        [HttpPut("ChangeCandidatePwd/{id}")]
        [Authorize]
        public async Task<IActionResult> ChangeCandidatePassword(string id,[FromBody] SetPasswordDto setPasswordDto){
            try{
                if(string.IsNullOrEmpty(setPasswordDto.newPwd) || string.IsNullOrEmpty(setPasswordDto.oldPwd) )
                    return BadRequest(new {Status = "False", Message = "Mật khẩu cũ và mật khẩu mới không được bỏ trống."});

                var result = await _candidateReposistory._ChangeCandidatePassword(id,setPasswordDto.oldPwd,setPasswordDto.newPwd);
                int status = 200;
                if(result <= 0){
                    string errorMessage = result switch{
                        0 => "Không tìm thấy ID ứng cử viên",
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

                return Ok(new{Status = false, Message = "Đặt lại mật khẩu mới của ứng cử viên thành công"});
            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện xóa tài khoản ứng cử viên: {ex.Message}"
                });
            }
        }

        //9. Gửi báo cáo
        [HttpPost("sendReport")]
        [Authorize]
        public async Task<IActionResult> VoterSubmitReport([FromBody] SendReportDto sendReportDto){
            try{
                //Kiểm tra đầu vào
                if(string.IsNullOrEmpty(sendReportDto.YKien) || string.IsNullOrEmpty(sendReportDto.IDSender))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền ý kiến và mã người gửi."});

                var result = await _candidateReposistory._CandidateSubmitReport(sendReportDto);
                
                if(result == false)
                    return BadRequest(new{Status = "False", Message = "Không tìm thấy ID ứng cử viên để gửi báo cáo."});

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