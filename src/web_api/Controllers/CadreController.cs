
using BackEnd.src.core.Interfaces;
using BackEnd.src.core.Models;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.web_api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("FixedWindowLimiter")]
    public class CadreController : ControllerBase
    {
        private readonly ICadreRepository _CadreRepository;
                private readonly IVotingServices _votingServices;

        //Khởi tạo
        public CadreController(
            ICadreRepository CadreRepository,
            IVotingServices VotingServices
        ){
            _CadreRepository = CadreRepository;
            _votingServices = VotingServices;
        }


        //1. Thêm
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "1")]
        [EnableRateLimiting("SlidingWindowLimiter")]
        public async Task<IActionResult> CreateCadre([FromForm] CadreDto Cadre,IFormFile fileAnh){
            try{
                //Kiểm tra đầu vào
                if(Cadre == null || string.IsNullOrEmpty(Cadre.HoTen))
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi khi đầu vào không được rỗng"
                    });
                
                //lấy kết quả thêm vào được hay không
                var result = await _CadreRepository._AddCadre(Cadre, fileAnh);
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
                    Message = "Thêm tài khoản cán bộ thành công"
                });
            }catch(Exception ex){
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm tài khoản cán bộ: {ex.Message}"
                });
            }
        }

        //2. Lấy all cán bộ
        [HttpGet]
        [Route("allCadre")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetListOfCadre(){
            try{
                var result = await _CadreRepository._GetListOfCadre();

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = result 
                });
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi truy xuất danh sách các cán bộ: {ex.Message}"
                });
            }
        }

        //Lấy all cán bộ - account
        [HttpGet]
        [Route("allCadreAndAccount")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetListOfCadreAndAccount(){
            try{
                var result = await _CadreRepository._GetListOfCadresAndAccounts();

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = result
                });
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi truy xuất danh sách các cán bộ: {ex.Message}"
                });
            }
        }

        //4. Lấy theo ID
        [HttpGet("{id}")]
        [Authorize(Roles = "1,8,3,4" )]
        public async Task<IActionResult> GetCadreBy_ID(string id){
            try{
                var District = await _CadreRepository._GetCadreBy_ID(id);
                if(District == null)
                    return StatusCode(400, new{
                    Status = "False", 
                    Message = $"Lỗi ID_QH của cán bộ không tồn tại"
                });

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = District
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy thông tin cán bộ theo ID: {ex.Message}"
                });
            }
        }

        //5.Sửa
        [HttpPut("{id}")]
        [Authorize(Roles = "1,8,3,4")]
        public async Task<IActionResult> EditCadreBy_ID(string id,[FromBody] CadreDto UngCuVien){
            try{
                if(UngCuVien == null || string.IsNullOrEmpty(UngCuVien.HoTen))
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không được để trống"
                    });

                var result = await _CadreRepository._EditCadreBy_ID(id, UngCuVien);
                int status = 200;
                if(result <= 0){
                    string errorMessage = result switch{
                        0 =>"Đã trùng số điện thoại",
                        -1 => "Đã trùng Email",
                        -2 =>"Đã trùng CCCD",
                        -3 => "Vai trò người dùng không tìm thấy",
                        -4 =>"Đầu vào không hợp lệ hoặc bị để trống trường nào đó",
                        -5 => "Không tìm thấy ID_usr từ cán bộ",
                        -6 => "Lỗi khi thực hiện cập nhật tài khoản cán bộ",
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
                    Message = $"Lỗi khi thực hiện sửa thông tin cán bộ: {ex.Message}"
                });
            }
        }
        
        //6.xóa
        [HttpDelete("{id}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> DeleteCadreBy_ID(string id){
            try{
                var result = await _CadreRepository._DeleteCadreBy_ID(id);
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
                    Message = $"Lỗi khi thực hiện xóa tài khoản cán bộ: {ex.Message}"
                });
            }
        }

        //7. Đặt lại mật khẩu - admin
        [HttpPut("SetCadrePwd/{id}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> SetCadrePassword(string id,[FromBody] SetPasswordDto setPasswordDto){
            try{
                if(string.IsNullOrEmpty(setPasswordDto.newPwd) )
                    return BadRequest(new {Status = "False", Message = "Mật khẩu không được bỏ trống."});

                var result = await _CadreRepository._SetCadrePassword(id,setPasswordDto.newPwd);
                if(result == false)
                    return BadRequest(new{Status = false, Message = "Không tìm thấy ID cán bộ để đặt lại mật khẩu mới."});
                
                return Ok(new{Status = true, Message = "Đặt lại mật khẩu mới của cán bộ thành công"});
            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện đặt lại mật khẩu cán bộ: {ex.Message}"
                });
            }
        }

        //8. Thay đổi mật khẩu - cán bộ
        [HttpPut("ChangeCadrePwd/{id}")]
        [Authorize(Roles = "1,8,3,4")]
        public async Task<IActionResult> ChangeCadrePassword(string id,[FromBody] ChangePasswordDto setPasswordDto){
            try{
                if(string.IsNullOrEmpty(setPasswordDto.newPwd) || string.IsNullOrEmpty(setPasswordDto.oldPwd) )
                    return BadRequest(new {Status = "False", Message = "Mật khẩu cũ và mật khẩu mới không được bỏ trống."});

                var result = await _CadreRepository._ChangeCadrePassword(id,setPasswordDto.oldPwd,setPasswordDto.newPwd);
                int status = 200;
                if(result <= 0){
                    string errorMessage = result switch{
                        0 => "Không tìm thấy ID cán bộ",
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

                return Ok(new{Status = false, Message = "Đặt lại mật khẩu mới của cán bộ thành công"});
            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện xóa tài khoản cán bộ: {ex.Message}"
                });
            }
        }

        //9. Gửi báo cáo
        [HttpPost("sendReport")]
        [Authorize(Roles = "1,8")]
        public async Task<IActionResult> VoterSubmitReport([FromBody] SendReportDto sendReportDto){
            try{
                //Kiểm tra đầu vào
                if(string.IsNullOrEmpty(sendReportDto.YKien) || string.IsNullOrEmpty(sendReportDto.IDSender))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền ý kiến và mã người gửi."});

                var result = await _CadreRepository._CadreSubmitReport(sendReportDto);
                
                if(result == false)
                    return BadRequest(new{Status = "False", Message = "Không tìm thấy ID cán bộ để gửi báo cáo."});

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

        //10.Thêm danh sách các cán bộ vào trực tại kỳ bầu cử
        [HttpPost("add-cadre-list-to-election")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> AddListCadresToTheElection([FromBody] CadreListInElectionDto cadreListInElectionDto){
            try{
                //Kiểm tra đầu vào
                if(
                   cadreListInElectionDto.ListID_canbo.Count == 0 ||
                   string.IsNullOrEmpty(cadreListInElectionDto.ngayBD.ToString())
                ) 
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền đầy đủ thông tin."});
                
                var result = await _CadreRepository._AddListCadresToTheElection(cadreListInElectionDto);
                if(result <=0){
                    int status = result switch{
                        0 => 400, -1 => 400, -2 => 500, -3 => 400, -4 => 500, -5 => 400, -6=>400,_ => 500
                    };
                    string errorMessage = result switch{
                        0 => "Không thấy cán bộ nào hợp lệ để thêm vào",
                        -1 => "Không tìm thấy ban",
                        -2 => "Không tìm thấy thời điểm bắt đầu cuộc bầu cử",
                        _ =>"Lỗi không xác định"
                    };

                    return StatusCode(status ,new{Status = "False", Message = errorMessage}); 
                }

                return Ok(new ApiRespons{
                    Success = true,
                    Message = $"Thêm danh sách cán bộ vào cuộc bầu cử thành công. (Số lượng cán bộ hợp lệ là {result})"
                });
            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm danh sách cán bộ vào kỳ bầu cử: {ex.Message}"
                });
            }
        }

        //13. Lấy danh sách các kỳ bầu cử mà cán bộ đã tham dự
        [HttpGet]
        [Route("get-list-of-cadre-joined-for-election")]
        [Authorize(Roles = "1,8")]
        [EnableRateLimiting("SlidingWindowLimiter")]
        public async Task<IActionResult> getListOfCadreJoinedForElection([FromQuery] string ID_CanBo){
            try{
                if(string.IsNullOrEmpty(ID_CanBo) )
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền mã cán bộ."});

                var result = await _CadreRepository._getListOfCadreJoinedForElection(ID_CanBo);
                if(result == null)
                    return BadRequest(new{Status = "False", Message = "Không tìm thấy mã cán bộ"});

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = result
                });
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi lấy danh sách kỳ bầu cử mã cán bộ đang trực thuộc: {ex.Message}"
                });
            }
        }

        //14. cán bộ bỏ phiếu
        [HttpPost("cadre-vote")]
        [Authorize(Roles= "1,8")]
        public async Task<IActionResult> CadreVote([FromBody] CadreVoteDTO cadreVoteDTO){
            try{
                // Kiểm tra đầu vào
                if(cadreVoteDTO == null || string.IsNullOrEmpty(cadreVoteDTO.ID_CanBo))
                    return StatusCode(400,new{
                        Status = "false",
                        Message="Lỗi khi đầu vào không được rỗng"
                    });
        
                // Lấy kết quả thêm vào được hay không
                int result = await _votingServices._CadreVote(cadreVoteDTO);
                if(result <= 0){
                    string errorMessage = result switch{
                        0 => "Lỗi ngày bỏ phiếu không hợp lệ",
                        -1 =>"Lỗi cán bộ không tồn tại",
                        -2 => "Lỗi không tìm thấy kỳ bầu cử",
                        -3 =>"cán bộ đã bỏ phiếu rồi",
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