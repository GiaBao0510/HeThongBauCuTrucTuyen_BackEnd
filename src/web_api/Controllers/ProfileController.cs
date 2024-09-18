

using BackEnd.src.infrastructure.DataAccess.IRepository;
using BackEnd.src.web_api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileRepository _profileRepository;
        public ProfileController(IProfileRepository profileRepository){
            _profileRepository = profileRepository;
        }

        //1.Cập nhật hồ sơ người dùng
        [HttpPut("UpdateProfileByIdUser/{id}")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> UpdateProfileBy_IdUser([FromBody]ProfileDto profileDto, string id){
            try{
                var result = await _profileRepository._UpdateProfileBy_IdUser(profileDto,id);
                if(result == false)
                    return BadRequest(new {Status = "False", Message = "Lỗii không tìm thấy ID người dùng"});
                
                return Ok(new{
                    Status = "Ok",
                    Message = "Cập nhật hồ sơ thành công"
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

        //2. Cập nhật hố sơ người dùng - mã hồ sơ
        [HttpPut("UpdateProfileByProfileCode/{id}")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> UpdateProfileBy_ProfileCode([FromBody]ProfileDto profileDto, string id){
            try{
                var result = await _profileRepository._UpdateProfileBy_ProfileCode(profileDto,id);
                if(result == false)
                    return BadRequest(new {Status = "False", Message = "Lỗii không tìm thấy ID hồ sơ"});
                
                return Ok(new{
                    Status = "Ok",
                    Message = "Cập nhật hồ sơ thành công"
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

        //3.Lấy danh sách người dùng đã đăng ký
        [HttpGet("ListRegisteredProfiles")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> GetListRegisteredProfiles(){
            try{
                var result = await _profileRepository._GetListRegisteredProfiles();
                
                return Ok(new{
                    Status = "Ok",
                    Message = "",
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

        //4.Lấy danh sách người dùng chưa đăng ký
        [HttpGet("ListUnregisteredProfiles")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> GetListUnregisteredProfiles(){
            try{
                var result = await _profileRepository._GetListUnregisteredProfiles();
                
                return Ok(new{
                    Status = "Ok",
                    Message = "",
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

        //5.Lấy danh sách hồ sơ
        [HttpGet("ListProfiles")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> GetListProfiles(){
            try{
                var result = await _profileRepository._GetListProfiles();
                
                return Ok(new{
                    Status = "Ok",
                    Message = "",
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

        //6.Lấy danh sách cử tri đã đăng ký tài khoản
        [HttpGet("ListRegisteredVoters")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> GetListRegisteredVoter(){
            try{
                var result = await _profileRepository._GetListRegisteredVoter();
                
                return Ok(new{
                    Status = "Ok",
                    Message = "",
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

        //7.Lấy danh sách cử tri chưa đăng ký tài khoản
        [HttpGet("ListUnregisteredVoters")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> GetListUnregisteredVoter(){
            try{
                var result = await _profileRepository._GetListUnregisteredVoter();
                
                return Ok(new{
                    Status = "Ok",
                    Message = "",
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

        //8. Xóa hồ sơ dựa trên ID_người dùng
        [HttpDelete("DeleteProfileByUserId/{id}")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> DeleteProfileBy_Id_user(string id){
            try{
                var result = await _profileRepository._DeleteProfileBy_Id_user(id);
                if(result == false)
                    return BadRequest(new {Status = "False", Message = "Lỗi không tìm thấy ID người dùng"});
                
                return Ok(new{
                    Status = "Ok",
                    Message = "Xóa hồ sơ thành công"
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

        //9. Xóa hồ sơ dựa trên Mã hồ sơ
        [HttpDelete("DeleteProfileByProfileCode/{id}")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> DeleteProfileBy_ProfileCode(string id){
            try{
                var result = await _profileRepository._DeleteProfileBy_ProfileCode(id);
                if(result == false)
                    return BadRequest(new {Status = "False", Message = "Lỗi không tìm thấy ID hồ sơ"});
                
                return Ok(new{
                    Status = "Ok",
                    Message = "Xóa hồ sơ thành công"
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

        //10.  Xóa hồ sơ dựa trên ID_cử tri
        [HttpDelete("DeleteProfileByIdVoter/{id}")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> DeleteProfileBy_ID_cutri(string id){
            try{
                var result = await _profileRepository._DeleteProfileBy_ID_cutri(id);
                if(result == false)
                    return BadRequest(new {Status = "False", Message = "Lỗi không tìm thấy ID cử tri"});
                
                return Ok(new{
                    Status = "Ok",
                    Message = "Xóa hồ sơ thành công"
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

        //11. Xóa hồ sơ dựa trên ID_Cán bộ
        [HttpDelete("DeleteProfileByIdCadre/{id}")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> DeleteProfileBy_ID_canbo(string id){
            try{
                var result = await _profileRepository._DeleteProfileBy_ID_canbo(id);
                if(result == false)
                    return BadRequest(new {Status = "False", Message = "Lỗi không tìm thấy ID cán bộ"});
                
                return Ok(new{
                    Status = "Ok",
                    Message = "Xóa hồ sơ thành công"
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

        //12. Xóa hồ sơ dựa trên ID_ứng cử viên
        [HttpDelete("DeleteProfileByIdCandidate/{id}")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> DeleteProfileBy_ID_ungcuvien(string id){
            try{
                var result = await _profileRepository._DeleteProfileBy_ID_ungcuvien(id);
                if(result == false)
                    return BadRequest(new {Status = "False", Message = "Lỗi không tìm thấy ID ứng cử viên"});
                
                return Ok(new{
                    Status = "Ok",
                    Message = "Xóa hồ sơ thành công"
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










    }
}