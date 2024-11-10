using Microsoft.AspNetCore.Mvc;
using BackEnd.src.infrastructure.DataAccess.Repositories;
using BackEnd.src.web_api.DTOs;
using Microsoft.AspNetCore.Http;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.AspNetCore.Authorization;
using BackEnd.src.core.Models;
using BackEnd.src.core.Interfaces;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoterController: ControllerBase
    {
        private readonly IVoterRepository _voterReposistory;
        private readonly IVotingServices _votingServices;

        //Khởi tạo
        public VoterController(
            IVoterRepository vouterReposistory,
            IVotingServices votingServices
        ){ 
            _voterReposistory = vouterReposistory;
            _votingServices = votingServices;
        }

        //1.Thêm
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> CreateVouter([FromForm] VoterDto vouter,  IFormFile fileAnh){
            try{
                Console.WriteLine($"Họ tên: {vouter.HoTen}");
                Console.WriteLine($"Giới tính: {vouter.GioiTinh}");
                Console.WriteLine($"Ngày sinh: {vouter.NgaySinh}");
                Console.WriteLine($"Địa chỉ: {vouter.DiaChiLienLac}");
                Console.WriteLine($"Số điện thoại: {vouter.SDT}");
                Console.WriteLine($"Email: {vouter.Email}");
                Console.WriteLine($"ID_ChucVu: {vouter.ID_ChucVu}");
                //Kiểm tra đầu vào
                if(string.IsNullOrEmpty(vouter.HoTen))
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
                        -5 => "Lỗi khi tạo hồ sơ cho cử tri",
                        -6 => "Lỗi ID chức vụ không tồn tại",
                        _ => "Lỗi không xác định"
                    };
                    int status = result switch{
                        0 => 400, -1 => 400, -2 => 400, -3 =>400, -4 => 400,
                        -5=> 500, -6 => 400, _ => 500
                    };
                    return StatusCode(status,new {Status = "False", Message = errorMessage});
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
        [Authorize(Roles= "1")]
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
        [Authorize(Roles= "1")]
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
        [Authorize(Roles= "1,5")]
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
        [Authorize(Roles= "1,5")]
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
        [Authorize(Roles= "1")]
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
        [Authorize(Roles= "1")]
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
        [Authorize(Roles= "1,5")]
        public async Task<IActionResult> ChangeVoterPassword(string id,[FromBody] ChangePasswordDto setPasswordDto){
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
        [Authorize(Roles= "5")]
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

        //10. Thay đổi chức vụ của cử tri
        [HttpPut("change-of-voter-position")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> ChangeOfVoterPosition([FromQuery] string id_cutri, [FromQuery] string id_chucvu){
            try{
                if(string.IsNullOrEmpty(id_cutri) || string.IsNullOrEmpty(id_chucvu))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền mã cử tri và mã chức vụ."});
                
                var result = await _voterReposistory._ChangeOfVoterPosition(id_cutri, Convert.ToInt32(id_chucvu));
                if(result == false)
                    return BadRequest(new{Status = "False", Message = "Mã chức vụ hoặc mã cử tri không tồn tại"});
                
                return Ok(new{Status = true, Message = "Thay đổi chức vụ cử tri thành công"});

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

        //11.HIển thị thông tin cử tri sau khi quét mã QR
        [HttpGet("show-information-before-registration")]
        public async Task<IActionResult> DisplayUserInformationAfterScanningTheCode([FromQuery] string id_cutri){
            try{ 
                if(string.IsNullOrEmpty(id_cutri))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền mã cử tri."});

                var result = await _voterReposistory._DisplayUserInformationAfterScanningTheCode(id_cutri);
                if(result == null)
                    return BadRequest(new{Status = "False", Message = "Không tìm thấy thông tin cử tri hoặc cử tri đã đăng ký rồi"});

                return Ok(new ApiRespons{
                    Success = true,
                    Message = "Thông tin cử tri",
                    Data = result
                });
            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy thông tin cử tri trước khi đăng ký: {ex.Message}"
                });
            }
        }

        //12. đặt mật khẩu, CCCD của cử tri trước khi đăng ký
        [HttpPost("register-and-set-password")]
        public async Task<IActionResult> RegisterAndSetPassword([FromQuery] string id_cutri,[FromBody] RegisterDto registerDto){
            try{
                // Kiểm tra đầu vào
                if(string.IsNullOrEmpty(registerDto.pwd) || string.IsNullOrEmpty(registerDto.CCCD))
                    return BadRequest(new{Status = "False", Message = "Mật khẩu và số căn cước công dân không được bỏ trống."});
                
                var result = await _voterReposistory._SetVoterCCCD_SetVoterPwd(id_cutri, registerDto.CCCD, registerDto.pwd);
                if(result ==false)
                    return BadRequest(new{Status = "False", Message = "Mã cử tri không tồn tại"});

                return Ok(new{Status = true, Message = "Đăng ký thành công. Mật khẩu đã được đặt."});

            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy thông tin cử tri trước khi đăng ký: {ex.Message}"
                });
            }
        }

        //13.Thêm danh sách cử tri vào cuộc bầu cử nào đó
        [HttpPost("add-vote-list-to-election")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> AddListVotersToTheElection([FromBody]VoterListInElectionDto voterListInElectionDto){
            try{
                //Kiểm tra đầu vào
                if(
                   voterListInElectionDto.listIDVoter.Count == 0 ||
                   string.IsNullOrEmpty(voterListInElectionDto.ngayBD.ToString()) ||
                     string.IsNullOrEmpty(voterListInElectionDto.ID_DonViBauCu)
                )
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền đầy đủ thông tin."});
                
                var result = await _voterReposistory._AddListVotersToTheElection(voterListInElectionDto);
                if(result <=0){
                    int status = result switch{
                        0 => 400, -1 => 400, -2 => 500, -3 => 400, -4 => 500, _ => 500
                    };
                    string errorMessage = result switch{
                        0 => "Không tìm thấy được ngày tổ chức cuộc bầu cử",
                        -1 => "Không tìm thấy ID đơn vị bầu cử",
                        -2 => "Lỗi khi thực hiện lấy số lượng cử tri tối đa",
                        -3 => "Lỗi, số lượng cử tri tham gia vào kỳ bầu cử không lớn hơn số lượng quy định trước đó",
                        -4 => "Lỗi khi lấy số lượng cử tri hiện tại",
                        _ =>"Lỗi không xác định"
                    };

                    Console.WriteLine($"result: {result}");

                    return StatusCode(status ,new{Status = "False", Message = errorMessage}); 
                }

                return Ok(new ApiRespons{
                    Success = true,
                    Message = "Thêm danh sách cử tri vào cuộc bầu cử thành công"
                });

            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy thông tin cử tri trước khi đăng ký: {ex.Message}"
                });
            }
        } 

        //14. Hiển thị danh sách cử tri đã tham gia vào các k�� bầu cử  
        [HttpGet("list-pf-elections-voters-have-paticipated")]
        [Authorize(Roles= "1,5")]
        public async Task<IActionResult> ListElectionsVotersHavePaticipated([FromQuery]string ID_cutri){
            try{
                if(string.IsNullOrEmpty(ID_cutri))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền mã cử tri."});

                var result = await _voterReposistory._ListElectionsVotersHavePaticipated(ID_cutri);
                if(result == null)
                    return BadRequest(new{Status = "False", Message = "Không tìm thấy ID cử tri."});

                return Ok(new ApiRespons{
                    Success = true,
                    Message = "Danh sách các kỳ bầu cử mà cử tri đã tham gia",
                    Data = result
                });
            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy thông tin cử tri trước khi đăng ký: {ex.Message}"
                });
            }
        }

        //15. Thêm phiếu bầu
        [HttpPost("voter-vote")]
        [Authorize(Roles= "1,5")]
        public async Task<IActionResult> VoterVote([FromBody] VoterVoteDTO voterVoteDTO){
            try{
                // Kiểm tra đầu vào
                if(voterVoteDTO == null || string.IsNullOrEmpty(voterVoteDTO.ID_CuTri))
                    return StatusCode(400,new{
                        Status = "false",
                        Message="Lỗi khi đầu vào không được rỗng"
                    });
        
                // Lấy kết quả thêm vào được hay không
                int result = await _votingServices._VoterVote(voterVoteDTO);
                if(result <= 0){
                    string errorMessage = result switch{
                        0 => "Lỗi ngày bỏ phiếu không hợp lệ",
                        -1 =>"Lỗi cử tri không tồn tại",
                        -2 => "Lỗi không tìm thấy kỳ bầu cử",
                        -3 =>"Cử tri đã bỏ phiếu rồi",
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

        //16. Đếm số lượng các kỳ bầu cử mà cử tri đã tham gia
        [HttpGet("count-the-number-of-elections-voters-participated")]
        [Authorize(Roles= "1,5")]
        public async Task<IActionResult> CountTheNumberOfElections_VotersParticipated([FromQuery]string ID_cutri){
            try{
                if(string.IsNullOrEmpty(ID_cutri))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền mã cử tri."});

                var result = await _voterReposistory._countTheNumberOfElections_VotersParticipated(ID_cutri);
                if(result == -1)
                    return BadRequest(new{Status = "False", Message = "Không tìm thấy ID cử tri."});

                return Ok(new ApiRespons{
                    Success = true,
                    Message = "Số lượng các kỳ bầu cử mà cử tri đã tham gia",
                    Data = new {
                        count = result,
                    },
                });
            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Error TargetSite: {ex.TargetSite}");
                Console.WriteLine($"Error Source: {ex.Source}");
                Console.WriteLine($"Error HResult: {ex.HResult}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy thông tin cử tri trước khi đăng ký: {ex.Message}"
                });
            }
        }

        //17. Đếm số lượng các kỳ bầu cử mà cử tri sắp bỏ phiếu trong tương lai
        [HttpGet("count-the-number-of-elections-voter-will-vote-future")]
        [Authorize(Roles= "1,5")]
        public async Task<IActionResult> CountTheNumberOfElections_VoterWillVote_Future([FromQuery]string ID_cutri){
            try{
                if(string.IsNullOrEmpty(ID_cutri))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền mã cử tri."});

                var result = await _voterReposistory._countTheNumberOfElections_VoterWillVote_Future(ID_cutri);
                if(result == -1)
                    return BadRequest(new{Status = "False", Message = "Không tìm thấy ID cử tri."});

                return Ok(new ApiRespons{
                    Success = true,
                    Message = "Số lượng các kỳ bầu cử mà cử tri đã tham gia",
                    Data = new {
                        count = result,
                    },
                });
            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Error TargetSite: {ex.TargetSite}");
                Console.WriteLine($"Error Source: {ex.Source}");
                Console.WriteLine($"Error HResult: {ex.HResult}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy thông tin cử tri trước khi đăng ký: {ex.Message}"
                });
            }
        }

    }
}
