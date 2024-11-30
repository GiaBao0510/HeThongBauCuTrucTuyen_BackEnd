using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using BackEnd.src.web_api.DTOs;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using BackEnd.src.core.Entities;
using BackEnd.src.core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("FixedWindowLimiter")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;
        private readonly IMemoryCache _cache;

        //Khởi tạo
        public UserController(
            IUserRepository userRepository,
            IMapper mapper, ILogger<UserController> logger, IMemoryCache cache
        ){
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        //Thêm
        [HttpPost]
        [Route("add-user")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> CreateUser([FromForm] UserDto User, IFormFile fileAnh){
            try{
                if(!ModelState.IsValid)
                    return BadRequest(ModelState);
                
                //lấy kết quả thêm vào được hay không
                var result = await _userRepository._AddUser(User, fileAnh);
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
                    Message = "Thêm tài khoản người dùng thành công"
                });
            }catch(Exception ex){
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Error Data: {ex.Data}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine($"Error InnerException: {ex.InnerException}");
                return StatusCode(500, new ApiRespons{
                    Success = false, 
                    Message = $"Lỗi khi thực hiện thêm tài khoản Người dùng: {ex.Message}"
                });
            }
        }

        //Lấy all người dùng
        [HttpGet]
        [Route("AllUser")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> GetListOfUsers(){
            try{
                if(!_cache.TryGetValue("AllUsers", out List<Users> result)){
                    result = await _userRepository._GetListOfUsers();
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5)); //Lưu trong bộ nhớ đệm
                    _cache.Set("AllUsers", result, cacheEntryOptions);
                }
                return Ok(new{
                    Status="ok",
                    Message="null",
                    Data = result
                });
            }catch(Exception ex){
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Error Data: {ex.Data}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine($"Error InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "false",
                    Message=$"Error Message: {ex.Message}"
                });
            }
        }

        //Lấy all người dùng - tài khoản
        [HttpGet]
        [Route("AllUserAndAccount")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> GetListOfUsersAndAccounts(){
            try{
                if(!_cache.TryGetValue("AllUsersAndAccounts", out List<UserDto> result)){
                    result = await _userRepository._GetListOfUsersAndAccounts();
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5)); //Lưu trong bộ nhớ đệm
                    _cache.Set("AllUsersAndAccounts", result, cacheEntryOptions);
                }
                return Ok(new{
                    Status="ok",
                    Message="null",
                    Data = result
                });
            }catch(Exception ex){
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Error Data: {ex.Data}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine($"Error InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "false",
                    Message=$"Error Message: {ex.Message}"
                });
            }
        }

        //Lấy all người dùng theo role - admin, cử tri, cán bộ
        [HttpGet("ListUserRole/{id}")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> GetListOfUsersWithRole(string id){
            try{
                var result = await _userRepository._GetListOfUsersWithRole(int.Parse(id));
                return Ok(new{
                    Status="ok",
                    Message="null",
                    Data = result
                });
            }catch(Exception ex){
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Error Data: {ex.Data}");
                Console.WriteLine($"Error InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "false",
                    Message=$"Error Message: {ex.Message}"
                });
            }
        }

        //Lấy thông tin người dùng theo ID -admin, user
        [HttpGet("user{id}")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> GetVouterBy_ID(string id){
            try{
                var District = await _userRepository._GetUserBy_ID(id);
                if(District == null)
                    return StatusCode(400, new{
                    Status = "False", 
                    Message = $"Lỗi ID_user của người dùng không tồn tại"
                });

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = District
                });
            }catch(Exception ex){
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Error Data: {ex.Data}");
                Console.WriteLine($"Error InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy thông tin người dùng theo ID: {ex.Message}"
                });
            }
        }

        //Sửa - admin
        [HttpPut("EditUserForAdmin/{id}")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> EditUserBy_ID_ForAdmin(string id,[FromBody] UserDto user){
            try{
                if(user == null || string.IsNullOrEmpty(user.HoTen))
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không được để trống"
                    });

                var result = await _userRepository._EditUserBy_ID_ForAdmin(id, user);
                if(result <= 0){
                    string errorMessage = result switch{
                        0 =>"Đã trùng số điện thoại",
                        -1 => "Đã trùng Email",
                        -2 =>" Đã trùng CCCD",
                        -3 => "Vai trò người dùng không tìm thấy",
                        -4 =>"Đầu vào không hợp lệ hoặc bị để trống trường nào đó",
                        -5 => "Lỗi khi thực hiện cập nhật thông tin người dùng",
                        -6 => "Lỗi khi thực hiện cập nhật tài khoản người dùng",
                        _ => "Lỗi không xác định"
                    };
                    return BadRequest(new {Status = "False", Message = errorMessage});
                }

                return Ok(new{
                    Status = "Ok",
                    Message = "Cập nhật thành công"
                });
            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện sửa thông tin người dùng: {ex.Message}"
                });
            }
        }

        //Cập nhật Pwd người dùng dựa trên ID người dùng - admin
        [HttpPut("SetUserPwd/{id}")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> SetUserPassword(string id,[FromBody] SetPasswordDto setPasswordDto){
            try{
                if(string.IsNullOrEmpty(setPasswordDto.newPwd) )
                    return BadRequest(new {Status = "False", Message = "Mật khẩu không được bỏ trống."});

                var result = await _userRepository._SetUserPassword(id, setPasswordDto.newPwd);
                if(result == false)
                    return BadRequest(new {Status = "False", Message = "Lỗi không tìm thấy ID người dùng"});
                
                return Ok(new{
                    Status = "Ok",
                    Message = "Thay đổi mật khẩu thành công"
                });
            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi đặt lại mật khẩu người dùng: {ex.Message}"
                });
            }
        }

        //Cập nhật Pwd người dùng dựa trên ID người dùng - người dùng: cử tri, cán bộ, ứng cử viên
        [HttpPut("ChangeUserPwd/{id}")]
        [Authorize(Roles= "1,2,5,8")]
        public async Task<IActionResult> ChangeUserPassword(string id,[FromBody] ChangePasswordDto ChangePasswordDto){
            try{
                if(string.IsNullOrEmpty(ChangePasswordDto.newPwd) || string.IsNullOrEmpty(ChangePasswordDto.oldPwd))
                    return BadRequest(new {Status = "False", Message = "Thông tin đầu vào không được bỏ trống."});

                var result = await _userRepository._ChangeUserPassword(id, ChangePasswordDto.oldPwd,ChangePasswordDto.newPwd);
                int status = 200;
                if(result <= 0){
                    string errorMessage = result switch{
                        0 => "Không tìm thấy tài khoản người dùng",
                        -1 => "Không tìm thấy mật khẩu cũ người dùng",
                        -2 => "Xác minh mật khẩu không đúng",
                        _ => "Lỗi không xác định"
                    };
                    status = result switch{
                        0 => 404, 
                        -1 => 500,
                        -2 => 400,
                        _ => 500
                    };

                    return StatusCode( status,new {Status = "False", Message = errorMessage});
                }
                    
                return Ok(new{
                    Status = "Ok",
                    Message = "Thay đổi mật khẩu thành công"
                });
            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi đặt lại mật khẩu người dùng: {ex.Message}"
                });
            }
        }

        //Xóa người dùng theo ID người dùng
        [HttpDelete("{id}")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> DeleteUserBy_ID(string id){
            try{
                var result = await _userRepository._DeleteUserBy_ID(id);
                if(result == false)
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi không tìm thấy ID_user để xóa"
                    });

                return Ok(new{
                    Status = "Ok",
                    Message = "Xóa thành công"
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện xóa tài khoản người dùng: {ex.Message}"
                });
            }
        }

        //Chỉnh sửa ảnh của người dùng thông qua ID_user
        [HttpPut("ChangeUserImage/{id}")] 
        [Consumes("multipart/form-data")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> EditUserImageByID(string id,[FromForm] IFormFile fileAnh){
            try{
                if (fileAnh == null || fileAnh.Length == 0)
                {
                    return BadRequest(new { Status = false, Message = "File ảnh không hợp lệ" });
                }

                var result = await _userRepository._EditUserImageByID(id, fileAnh);
                return result switch{
                    true => Ok(new { Status = true, Message = "Thay đổi ảnh thành công." }),
                    false => StatusCode(404, new { Status = false, Message = "Không tìm thấy ID người dùng" }),
                };
            }catch(Exception ex){
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thay đổi ảnh người dùng: {ex.Message}"
                });
            }
        }

        //Người dùng thay đổi mật dựa trên email người dùng
        [HttpPut("set-pwd-based-on-email")]
        [EnableRateLimiting("SlidingWindowLimiter")]

        public async Task<IActionResult> SetPwdBasedOnUserEmail([FromQuery]string email ,[FromBody] SetPasswordDto setPasswordByEmailDto){
            try{
                if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(setPasswordByEmailDto.newPwd))
                    return BadRequest(new {Status = "False", Message = "Thông tin đầu vào không được bỏ trống."});

                var result = await _userRepository._SetPwdBasedOnUserEmail(email, setPasswordByEmailDto.newPwd);
                if(result < 1)
                    return BadRequest(new {Status = "False", Message = "Email người dùng không tồn tại."});

                return Ok(new{
                    Status = true,
                    Message ="Thay mật khẩu thành công."
                });
            }catch(Exception ex){
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thay đổi ảnh người dùng: {ex.Message}"
                });
            }
        }

        //Người dùng thay đổi mật dựa trên email người dùng
        [HttpGet("get-personal-information")]
        [EnableRateLimiting("SlidingWindowLimiter")]
        [Authorize(Roles= "1,2,3,4,5,8")]

        public async Task<IActionResult> GetPersonnalInfomationByEmail([FromQuery]string email){
            try{
                var result = await _userRepository._GetPersonnalInfomationByEmail(email);
                if(result == null)
                    return BadRequest(new {Status = "False", Message = "Email người dùng không tồn tại."});

                return Ok(new{
                    Status = true,
                    Message ="",
                    Data = result
                });
            }catch(Exception ex){
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy thông tin người dùng: {ex.Message}"
                });
            }
        }

        //14. Hiển thị danh sách cử tri đã tham gia vào các ky bầu cử  
        [HttpGet("list-of-elections-voters-have-paticipated")]
        [Authorize(Roles= "1,5")]
        public async Task<IActionResult> GetListOfElectionsByUserPhone([FromQuery]string sdt){
            try{
                if(string.IsNullOrEmpty(sdt))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền số điện thoại cử tri."});

                var result = await _userRepository._getListOfElectionsByUserPhone(sdt);
                if(result == null)
                    return BadRequest(new{Status = "False", Message = "Không tìm thấy số điện thoại cử tri."});

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

        //15. Người dùng tự cập nhật thoong tin cá nhân dựa trên ID người dùng
        [HttpPut("user-update-info")]
        [Authorize(Roles= "1,2,3,4,5,8")]
        public async Task<IActionResult> UpdatePersonalInfomation([FromBody]PersonalInformationDTO personalInfo){
            try{
                if(string.IsNullOrEmpty(personalInfo.HoTen))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền đầy đủ thông tin"});

                var result = await _userRepository._UpdatePersonalInfomation(personalInfo);
                if(result <= 0){
                    string errorMessage = result switch{
                        0 => "Không tìm thấy số điện thoại trước đó",
                        -1 => "Đã trùng số điện thoại",
                        -2 => "Đã trùng email",
                        _ => "Lỗi không xác định"
                    };
                    int status = result switch{
                        0 => 400, 
                        -1 => 400,
                        -2 => 400,
                        _ => 500
                    };

                    return StatusCode( status,new {Status = "False", Message = errorMessage});
                }

                return Ok(new ApiRespons{
                    Success = true,
                    Message = "Cập nhật thông tin thành công",
                    Data = result
                });
            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện cập nhật thông tin người dùng: {ex.Message}"
                });
            }
        }

        //16. Hiển thị danh sách người dùng chưa bỏ phiếu theo kỳ bầu cử
        [HttpGet("get-list-of-users-who-have-not-voted-by-election")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> GetListOfUsersWhoHaveNotVotedByElection([FromQuery] string ngayBD){
            try{
                if(string.IsNullOrEmpty(ngayBD))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền ngày bắt đầu của kỳ bầu cử."});

                var result = await _userRepository._getListOfUsersWhoHaveNotVotedByElection(ngayBD);
                return Ok(new{
                    Status="ok",
                    Message="null",
                    Data = result
                });
            }catch(Exception ex){
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Error Data: {ex.Data}");
                Console.WriteLine($"Error InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "false",
                    Message=$"Error Message: {ex.Message}"
                });
            }
        }

    }
}