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
using BackEnd.src.infrastructure.DataAccess.Context;
using BackEnd.src.web_api.DTOs;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VouterController: ControllerBase
    {
        private readonly VouterReposistory _vouterReposistory;

        //Khởi tạo
        public VouterController(VouterReposistory vouterReposistory) => _vouterReposistory = vouterReposistory;

        //Thêm
        [HttpPost]
        public async Task<IActionResult> CreateVouter([FromBody] VouterDto vouter){
            try{
                //Kiểm tra đầu vào
                if(vouter == null || string.IsNullOrEmpty(vouter.HoTen))
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi khi đầu vào không được rỗng"
                    });
                
                //lấy kết quả thêm vào được hay không
                var result = await _vouterReposistory._AddVouter(vouter);
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
                    Message = "Thêm tài khoản cử tri thành công"
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm tài khoản cử tri: {ex.Message}"
                });
            }
        }

        //Lấy all cử tri
        [HttpGet]
        [Route("allCuTri")]
        public async Task<IActionResult> GetListOfVouter(){
            try{
                var result = await _vouterReposistory._GetListOfVouter();

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

        //Lấy all cử tri
        [HttpGet]
        [Route("allCuTriandTaiKhoan")]
        public async Task<IActionResult> GetListOfVouterAndAccount(){
            try{
                var result = await _vouterReposistory._GetListOfVouterAndAccount();

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

        //Lấy theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVouterBy_ID(string id){
            try{
                var District = await _vouterReposistory._GetVouterBy_ID(id);
                if(District == null)
                    return StatusCode(400, new{
                    Status = "False", 
                    Message = $"Lỗi ID_QH của cử tri không tồn tại"
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

        //Sửa
        [HttpPut("foradmin/{id}")]
        public async Task<IActionResult> EditVouterBy_ID_Admin(string id, VouterDto cutri){
            try{
                if(cutri == null || string.IsNullOrEmpty(cutri.HoTen))
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không được để trống"
                    });

                var result = await _vouterReposistory._EditVouterBy_ID_Admin(id, cutri);
                if(result <= 0){
                    string errorMessage = result switch{
                        0 =>"Đã trùng số điện thoại",
                        -1 => "Đã trùng Email",
                        -2 =>"Đã trùng CCCD",
                        -3 => "Vai trò người dùng không tìm thấy",
                        -4 =>"Đầu vào không hợp lệ hoặc bị để trống trường nào đó",
                        -5 => "Lỗi khi thực hiện cập nhật thông tin cử tri",
                        -6 => "Lỗi khi thực hiện cập nhật tài khoản cử tri",
                        _ => "Lỗi không xác định"
                    };
                    return BadRequest(new {Status = "False", Message = errorMessage});
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

        //Sửa cho cử tri
        [HttpPut("forvoter/{id}")]
        public async Task<IActionResult> EditVouterBy_ID_Voter(string id, VouterDto cutri){
            try{
                if(cutri == null || string.IsNullOrEmpty(cutri.HoTen))
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không được để trống"
                    });

                var result = await _vouterReposistory._EditVouterBy_ID_Voter(id, cutri);
                if(result <= 0){
                    string errorMessage = result switch{
                        0 =>"Đã trùng số điện thoại",
                        -1 => "Đã trùng Email",
                        -2 =>"Đã trùng CCCD",
                        -3 => "Vai trò người dùng không tìm thấy",
                        -4 =>"Đầu vào không hợp lệ hoặc bị để trống trường nào đó",
                        -5 => "Lỗi khi thực hiện cập nhật thông tin cử tri",
                        -6 => "Lỗi khi thực hiện cập nhật tài khoản cử tri",
                        -7 => "Lỗi mật khẩu cũ không khớp, không thể cập nhập được",
                        _ => "Lỗi không xác định"
                    };
                    return BadRequest(new {Status = "False", Message = errorMessage});
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

        //Xóa
        //xóa
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVoterBy_ID(string id){
            try{
                var result = await _vouterReposistory._DeleteVoterBy_ID(id);
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


    }
}