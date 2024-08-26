using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BackEnd.core.Entities;
using System.Data;
using MySql.Data.MySqlClient;
using System.Data.Common;
using BackEnd.src.core.Interfaces;
using BackEnd.infrastructure.config;
using BackEnd.src.infrastructure.DataAccess.Repositories;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController:ControllerBase
    {
        private readonly RoleReposistory _roleResposistory;
        public RoleController(RoleReposistory roleReposistory){
            _roleResposistory = roleReposistory;
        }

        public static List<Roles> ListOfRoles = new List<Roles>();

        //Liêt kê
        [HttpGet]
        public async Task<IActionResult> GetListOfRole(){
            try{
                var roles = await _roleResposistory._GetListOfRoles();
                return Ok(roles);
            }
            catch(Exception ex){
                return StatusCode(500,$"Lỗi khi truy xuất danh sách các vai trò: {ex.Message}");
            }
        }

        //Thêm
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] Roles role){
            try{
                //Kiểm tra đầu vào
                if(role == null || string.IsNullOrEmpty(role.TenVaiTro)){
                    return BadRequest("Dữ liệu đầu vào không hợp lệ");
                }
                await _roleResposistory._AddRole(role);
                return Ok(new{
                    Success = true, Data = role
                });
            }catch(Exception ex){
                return StatusCode(500,$"Lỗi khi thực hiện thêm vai trò: {ex.Message}");
            }
        }

        //lấy theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleBy_ID(string id){
            try{
                var role = await _roleResposistory._GetRoleBy_ID(id);

                //Nếu độ dài == 0 thì id không tồn tại => Báo lỗi
                if(role.ToList().Count == 0)
                    return StatusCode(404,new{
                        Description = $"Lỗi ID: {id} không tồn tại."
                    });    
                
                return Ok(role);
            }catch(Exception ex){
                return StatusCode(500,$"Lỗi khi thực hiện tìm kiếm vai trò theo ID: {ex.Message}");
            }
        }

        //Sửa
        [HttpPut("{id}")]
        public async Task<IActionResult> EditRoleBy_ID(string id, [FromBody] Roles role){
            try{
                if(role == null || string.IsNullOrEmpty(role.TenVaiTro))
                    return StatusCode(400,new{
                        Description = $"Lỗi đầu vào không hợp lệ."
                    }); 

                var result = await _roleResposistory._EditRoleBy_ID(id,role);

                if(result == false)
                    return NotFound(new{
                        Description = $"Lỗi ID: {id} không tồn tại."
                    });

                return Ok(role); 
            }catch(Exception ex){
                return StatusCode(500,new {
                    Description = $"Lỗi khi thực hiện sửa vai trò theo ID: {ex.Message}"
                });
            }
        }

        //xóa 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoleBy_ID(string id){
            try{
                var result = await _roleResposistory._DeleteRoleBy_ID(id);

                if(result == false)
                    return NotFound(new{
                        Description = $"Lỗi ID: {id} không tồn tại."
                    });

                return Ok( new{ Description = "Đã xóa vai trò thành công" }); 
            }catch(Exception ex){
                return StatusCode(500,new {
                    Description = $"Lỗi khi thực hiện sửa vai trò theo ID: {ex.Message}"
                });
            }
        }
        
        
    }
}