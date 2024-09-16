using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using BackEnd.src.web_api.DTOs;
using BackEnd.src.infrastructure.Services;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using BackEnd.src.core.Entities;
using BackEnd.src.core.Models;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
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
                return StatusCode(500, new ApiRespons{
                    Success = false, 
                    Message = $"Lỗi khi thực hiện thêm tài khoản Người dùng: {ex.Message}"
                });
            }
        }

        //Lấy all người dùng
        [HttpGet]
        [Route("AllUser")]
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
        public async Task<IActionResult> ChangeUserPassword(string id,[FromBody] SetPasswordDto setPasswordDto){
            try{
                if(string.IsNullOrEmpty(setPasswordDto.newPwd) || string.IsNullOrEmpty(setPasswordDto.oldPwd))
                    return BadRequest(new {Status = "False", Message = "Thông tin đầu vào không được bỏ trống."});

                var result = await _userRepository._ChangeUserPassword(id, setPasswordDto.oldPwd,setPasswordDto.newPwd);
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
        public async Task<IActionResult> EditUserImageByID(string id,[FromForm]  IFormFile fileAnh){
            try{
                var result = await _userRepository._EditUserImageByID(id, fileAnh);
                if( result == false)
                    return StatusCode(404, new{
                        Status = false,
                        Message ="Không tìm thấy ID người dùng"
                    });
                return Ok(new{
                    Status = true,
                    Message ="Thay đổi ảnh thành công."
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

    }
}