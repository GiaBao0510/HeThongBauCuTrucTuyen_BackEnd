using Microsoft.AspNetCore.Mvc;
using BackEnd.core.Entities;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController:ControllerBase
    {
        private readonly IRoleRepository _roleResposistory;
        public RoleController(IRoleRepository roleReposistory){
            _roleResposistory = roleReposistory;
        }

        //Liêt kê
        [HttpGet]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetListOfRole(){
            try{
                var roles = await _roleResposistory._GetListOfRoles();
                return Ok( new{
                        Status = "Ok",
                        Message = "null",
                        Data = roles
                    }
                );
            }
            catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi truy xuất danh sách các vai trò: {ex.Message}"
                });
            }
        }

        //Thêm
        [HttpPost]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> CreateRole([FromBody] Roles role){
            try{
                //Kiểm tra đầu vào
                if(role == null || string.IsNullOrEmpty(role.TenVaiTro)){
                    return BadRequest("Dữ liệu đầu vào không hợp lệ");
                }
                await _roleResposistory._AddRole(role);
                return Ok(new{
                    Status = "OK", 
                    Message = "",
                    Data = role
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm vai trò: {ex.Message}"
                });
            }
        }

        //lấy theo ID
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetRoleBy_ID(string id){
            try{
                var role = await _roleResposistory._GetRoleBy_ID(id);

                //Nếu độ dài == 0 thì id không tồn tại => Báo lỗi
                if(role.ToList().Count == 0)
                    return StatusCode(404,new{
                        Status = "false",
                        Message = $"Lỗi ID: {id} không tồn tại."
                    });    
                
                return Ok(role);
            }catch(Exception ex){
                return StatusCode(
                    500, new{
                        Status = "false",
                        Message = $"Lỗi khi thực hiện tìm kiếm vai trò theo ID: {ex.Message}"
                    }
                );
            }
        }

        //Sửa
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> EditRoleBy_ID(string id, [FromBody] Roles role){
            try{
                if(role == null || string.IsNullOrEmpty(role.TenVaiTro))
                    return StatusCode(400,new{
                        Status = "False",
                        Message = $"Lỗi đầu vào không hợp lệ."
                    }); 

                var result = await _roleResposistory._EditRoleBy_ID(id,role);

                if(result == false)
                    return NotFound(new{
                        Status = "False",
                        Message = $"Lỗi ID: {id} không tồn tại."
                    });

                return Ok(role); 
            }catch(Exception ex){
                return StatusCode(500,new {
                    Status = "False",
                    Message = $"Lỗi khi thực hiện sửa vai trò theo ID: {ex.Message}"
                });
            }
        }

        //xóa 
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRoleBy_ID(string id){
            try{
                var result = await _roleResposistory._DeleteRoleBy_ID(id);

                if(result == false)
                    return NotFound(new{
                        Status = "False",
                        Message = $"Lỗi ID: {id} không tồn tại."
                    });

                return Ok( new{
                    Status = "ok", 
                    Message = "Đã xóa vai trò thành công" 
                }); 
            }catch(Exception ex){
                return StatusCode(500,new {
                    Status = "False",
                    Message = $"Lỗi khi thực hiện sửa vai trò theo ID: {ex.Message}"
                });
            }
        }        
    }
}