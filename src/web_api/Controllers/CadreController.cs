
using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.web_api.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CadreController : ControllerBase
    {
        private readonly ICadreRepository _CadreRepository;

        //Khởi tạo
        public CadreController(ICadreRepository CadreRepository) => _CadreRepository = CadreRepository;

        //1. Thêm
        [HttpPost]
        [Consumes("multipart/form-data")]
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
        public async Task<IActionResult> ChangeCadrePassword(string id,[FromBody] SetPasswordDto setPasswordDto){
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
    }
}