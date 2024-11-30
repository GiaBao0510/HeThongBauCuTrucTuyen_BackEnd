using Microsoft.AspNetCore.Mvc;
using BackEnd.src.web_api.DTOs;
using BackEnd.src.infrastructure.DataAccess.Repositories;
using Microsoft.AspNetCore.Http;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.AspNetCore.Authorization;
using BackEnd.src.core.Models;
using Microsoft.AspNetCore.RateLimiting;
using BackEnd.src.core.Interfaces;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("FixedWindowLimiter")]
    public class CandidateController: ControllerBase
    {
        private readonly ICandidateRepository _candidateReposistory;
        private readonly IVotingServices _votingServices;
        //Khởi tạo
        public CandidateController(
            ICandidateRepository CandidateReposistory,
            IVotingServices VotingServices    
        ){
            _candidateReposistory = CandidateReposistory;
            _votingServices = VotingServices;
        }

        //1. Thêm
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "1")]
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
                        -7 => "Đã vượt quá số lượng ứng cử viên đã quy định trong kỳ này.",
                        -8 => "Đây không phải là thời điểm thích hợp để ứng cử cho kỳ bầu cử này",
                        -9 => "Lỗi ID trình độ học vấn không tồn tại",
                        _ => "Lỗi không xác định"
                    };
                    int statusCode = result switch{
                        0 => 400, -1 =>400, -2 => 400,
                        -3 =>400, -4 =>400, -5 => 400,
                        -6 => 400, -7 => 400 ,-8 =>400,
                        -9 =>400, _ => 500
                    };
                    Console.WriteLine($"Kết quả: {result}");
                    return StatusCode(statusCode ,new {Status = "False", Message = errorMessage});
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
        [Authorize(Roles = "1")]
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
        [Authorize(Roles = "1")]
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
        [Authorize(Roles = "1,2")]
        [EnableRateLimiting("SlidingWindowLimiter")]
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
        [Authorize(Roles = "1,2")]
        [EnableRateLimiting("SlidingWindowLimiter")]
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
        [Authorize(Roles = "1")]
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
        [Authorize(Roles = "1")]
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
        [EnableRateLimiting("SlidingWindowLimiter")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> ChangeCandidatePassword(string id,[FromBody] ChangePasswordDto setPasswordDto){
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
        [Authorize(Roles = "2")]
        [EnableRateLimiting("SlidingWindowLimiter")]
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

        //10.Thêm danh sách các ứng cử viên vào kỳ bầu cử
        [HttpPost("add-candidate-list-to-election")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> AddListCandidatesToTheElection([FromBody] CandidateListInElectionDto candidateListInElectionDto){
            try{
                //Kiểm tra đầu vào
                if(
                   candidateListInElectionDto.listIDCandidate.Count == 0 ||
                   string.IsNullOrEmpty(candidateListInElectionDto.ngayBD.ToString()) ||
                     string.IsNullOrEmpty(candidateListInElectionDto.ID_Cap.ToString())
                ) 
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền đầy đủ thông tin."});
                
                var result = await _candidateReposistory._AddListCandidatesToTheElection(candidateListInElectionDto);
                if(result <=0){
                    int status = result switch{
                        0 => 400, -1 => 400, -2 => 500, -3 => 400, -4 => 500, -5 => 400, -6=>400,_ => 500
                    };
                    string errorMessage = result switch{
                        0 => "Không tìm thấy được ngày tổ chức cuộc bầu cử",
                        -1 => "Không tìm thấy ID vị trí ứng cử",
                        -2 => "Lỗi khi thực hiện lấy số lượng ứng cử viên tối đa",
                        -3 => "Lỗi, số lượng ứng cử viên tham gia vào kỳ bầu cử không lớn hơn số lượng quy định trước đó",
                        -4 => "Lỗi khi lấy số lượng ứng cử viên hiện tại",
                        -5 => "Lỗi ngày đăng ký ứng cử đã kết thúc",
                        -6 => "Lỗi không tìm thấy bất kỳ mã ứng cử viên nào hợp lệ",
                        _ =>"Lỗi không xác định"
                    };

                    return StatusCode(status ,new{Status = "False", Message = errorMessage}); 
                }

                return Ok(new ApiRespons{
                    Success = true,
                    Message = $"Thêm danh sách ứng cử viên vào cuộc bầu cử thành công. (Số lượng ứng cử viên hợp lệ là {result})"
                });
            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm danh sách ứng cử viên vào kỳ bầu cử: {ex.Message}"
                });
            }
        }

        //11. Xóa ứng cử viên ra khỏi kỳ bầu cử theo ID
        [HttpDelete("remove-candidate-from-election")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> RemoveCandidateFromSpecificElection([FromQuery] string ID_ucv, [FromQuery] DateTime ngayBD){
            try{
                if(string.IsNullOrEmpty(ID_ucv) || string.IsNullOrEmpty(ngayBD.ToString()))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền đầy đủ thông tin."});
            
                var result = await _candidateReposistory._RemoveCandidateOfElection(ID_ucv, ngayBD);
                if(result <=0){
                    int status = result switch{
                        0 => 400, -1 => 400, -2 => 500, _ => 500
                    };
                    string errorMessage = result switch{
                        0 => "Không tìm thấy ID ứng cử viên",
                        -1 => "Không tìm thấy thời điểm của kỳ bầu cử cụ thể",
                        -2 => "Lỗi khi thực hiện xóa ứng cử viên ra khỏi kỳ bầu cử",
                       _ =>"Lỗi không xác định"
                    };

                    return StatusCode(status ,new{Status = "False", Message = errorMessage}); 
                }
                
                return Ok(new ApiRespons{
                    Success = true,
                    Message = "Xóa ứng cử viên ra khỏi kỳ bầu cử thành công"
                });

            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện xóa ứng cử viên khỏi kỳ bầu cử cụ thể: {ex.Message}"
                });
            }
        }

        //12. Lấydanh sách ứng cử viên dựa trên thời điểm bỏ phiếu
        [HttpGet]
        [Route("get-candidate-list-based-on-election-date")]
        [Authorize(Roles = "1,2,5,8")]
        [EnableRateLimiting("SlidingWindowLimiter")]
        public async Task<IActionResult> GetCandidateListBasedOnElectionDate([FromQuery] DateTime ngayBD){
            try{
                Console.WriteLine($"ngay BD:{ngayBD}");
                if(string.IsNullOrEmpty(ngayBD.ToString()) || ngayBD == DateTime.MinValue )
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền thời điểm bầu cử."});

                var result = await _candidateReposistory._GetCandidateListBasedOnElectionDate(ngayBD);
                if(result == null)
                    return BadRequest(new{Status = "False", Message = "Không tìm thấy thời điểm bầu cử"});

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = result
                });
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi lấy danh sách ứng cử viên dựa trên thời điểm bầu cử: {ex.Message}"
                });
            }
        }

        //13. Lấy các kỳ bầu cử mà ứng cử viên đã ghi danh
        [HttpGet]
        [Route("get-list-of-registered-candidate")]
        [Authorize(Roles = "1,2,5")]
        [EnableRateLimiting("SlidingWindowLimiter")]
        public async Task<IActionResult> getListOfRegisteredCandidate([FromQuery] string ID_ucv){
            try{
                if(string.IsNullOrEmpty(ID_ucv) )
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền mã ứng cử viên."});

                var result = await _candidateReposistory._getListOfRegisteredCandidate(ID_ucv);
                if(result == null)
                    return BadRequest(new{Status = "False", Message = "Không tìm thấy mã ứng cử viên"});

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = result
                });
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi lấy danh sách kỳ bầu cử mã ứng cử viên đã đăng ký: {ex.Message}"
                });
            }
        }

        //14. ứng cử viên bỏ phiếu
        [HttpPost("candidate-vote")]
        [Authorize(Roles= "1,2")]
        public async Task<IActionResult> VoterVote([FromBody] CandidateVoteDTO candidateVoteDTO){
            try{
                // Kiểm tra đầu vào
                if(candidateVoteDTO == null || string.IsNullOrEmpty(candidateVoteDTO.ID_ucv))
                    return StatusCode(400,new{
                        Status = "false",
                        Message="Lỗi khi đầu vào không được rỗng"
                    });
        
                // Lấy kết quả thêm vào được hay không
                int result = await _votingServices._CandidateVote(candidateVoteDTO);
                if(result <= 0){
                    string errorMessage = result switch{
                        0 => "Lỗi ngày bỏ phiếu không hợp lệ",
                        -1 =>"Lỗi Ứng cử viên không tồn tại",
                        -2 => "Lỗi không tìm thấy kỳ bầu cử",
                        -3 =>"Ứng cử viên đã bỏ phiếu rồi",
                        -4 =>"Lỗi giá trị phiếu bầu không hợp lệ",
                        -5 => "Lỗi vị trí ứng tuyển để bầu cử không tồn tại",
                        -6 => "Lỗi không tìm thấy đơn vị bầu cử",
                        -7 => "Lỗi khi khởi tại và thêm phiếu bầu",
                        -8 => "Lỗi khi cập nhật trạng thái bầu cử của người dùng",
                        -9 => "Lỗi khi thêm thông tin chi tiết của người dùng",
                        -10 => "Lỗi ngày bắt đầu của kỳ bầu cử không tồn tại",
                        _ => "Lỗi không xác định"
                    };
                    int status = result switch{
                        0 => 400, -1 => 400, -2 => 400, -3 =>400, -4 => 400,
                        -5=> 400, -6 => 400, -7=>500, -8=>500 , -9=> 500, -10 => 400,
                        _ => 500
                    };
                    return StatusCode(status,new {Status = "False", Message = errorMessage});
                }
                
                return Ok(new{
                    Status = "OK", 
                    Message = "Bỏ phiếu bình chọn thành công"
                });
            }catch(Exception ex){
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Error TargetSite: {ex.TargetSite}");
                Console.WriteLine($"Error Source: {ex.Source}");
                Console.WriteLine($"Error HResult: {ex.HResult}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện bỏ phiếu: {ex.Message}"
                });
            }
        }

    }
}