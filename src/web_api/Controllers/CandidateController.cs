// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using BackEnd.core.Entities;
// using System.Data;
// using MySql.Data.MySqlClient;
// using System.Data.Common;
// using BackEnd.src.core.Interfaces;
// using BackEnd.infrastructure.config;
// using BackEnd.src.infrastructure.DataAccess.Repositories;
// using BackEnd.src.infrastructure.DataAccess.Context;
// using BackEnd.src.web_api.DTOs;

// namespace BackEnd.src.web_api.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class CandidateController: ControllerBase
//     {
//         private readonly CandidateReposistory _candidateReposistory;

//         //Khởi tạo
//         public CandidateController(CandidateReposistory CandidateReposistory) => _candidateReposistory = CandidateReposistory;

//         //Thêm
//         [HttpPost]
//         public async Task<IActionResult> CreateCandidate([FromBody] CandidateDto Candidate){
//             try{
//                 //Kiểm tra đầu vào
//                 if(Candidate == null || string.IsNullOrEmpty(Candidate.HoTen))
//                     return StatusCode(400,new{
//                         Status = "false",
//                         Message=$"Lỗi khi đầu vào không được rỗng"
//                     });
                
//                 //lấy kết quả thêm vào được hay không
//                 var result = await _candidateReposistory._AddCandidate(Candidate);
//                 if(result <= 0){
//                     string errorMessage = result switch{
//                         0 => "Số điện thoại đã bị trùng",
//                         -1 =>"Email thoại đã bị trùng",
//                         -2 => "Căn cước công dân thoại đã bị trùng",
//                         -3 =>"Vai trò người dùng không tồn tại",
//                         -4 =>"Đầu vào không hợp lệ hoặc bị để trống trường nào đó",
//                         _ => "Lỗi không xác định"
//                     };
//                     return BadRequest(new {Status = "False", Message = errorMessage});
//                 }
                    
//                 return Ok(new{
//                     Status = "OK", 
//                     Message = "Thêm tài khoản ứng cử viên thành công"
//                 });
//             }catch(Exception ex){
//                 return StatusCode(500, new{
//                     Status = "False", 
//                     Message = $"Lỗi khi thực hiện thêm tài khoản ứng cử viên: {ex.Message}"
//                 });
//             }
//         }

//         //Lấy all ứng cử viên
//         [HttpGet]
//         [Route("allUngCuVien")]
//         public async Task<IActionResult> GetListOfCandidate(){
//             try{
//                 var result = await _candidateReposistory._GetListOfCandidate();

//                 return Ok(new{
//                     Status = "Ok",
//                     Message = "null",
//                     Data = result
//                 });
//             }catch(Exception ex){
//                 return StatusCode(500,new{
//                     Status = "false",
//                     Message=$"Lỗi khi truy xuất danh sách các ứng cử viên: {ex.Message}"
//                 });
//             }
//         }

//         //Lấy all ứng cử viên
//         [HttpGet]
//         [Route("allUngCuVienandTaiKhoan")]
//         public async Task<IActionResult> GetListOfCandidateAndAccount(){
//             try{
//                 var result = await _candidateReposistory._GetListOfCandidateAndAccount();

//                 return Ok(new{
//                     Status = "Ok",
//                     Message = "null",
//                     Data = result
//                 });
//             }catch(Exception ex){
//                 return StatusCode(500,new{
//                     Status = "false",
//                     Message=$"Lỗi khi truy xuất danh sách các ứng cử viên: {ex.Message}"
//                 });
//             }
//         }

//         //Lấy theo ID
//         [HttpGet("{id}")]
//         public async Task<IActionResult> GetCandidateBy_ID(string id){
//             try{
//                 var District = await _candidateReposistory._GetCandidateBy_ID(id);
//                 if(District == null)
//                     return StatusCode(400, new{
//                     Status = "False", 
//                     Message = $"Lỗi ID_QH của ứng cử viên không tồn tại"
//                 });

//                 return Ok(new{
//                     Status = "Ok",
//                     Message = "null",
//                     Data = District
//                 });
//             }catch(Exception ex){
//                 return StatusCode(500, new{
//                     Status = "False", 
//                     Message = $"Lỗi khi thực hiện lấy thông tin ứng cử viên theo ID: {ex.Message}"
//                 });
//             }
//         }

//         //Sửa
//         [HttpPut("foradmin/{id}")]
//         public async Task<IActionResult> EditCandidateBy_ID_Admin(string id, CandidateDto UngCuVien){
//             try{
//                 if(UngCuVien == null || string.IsNullOrEmpty(UngCuVien.HoTen))
//                     return StatusCode(400, new{
//                         Status = "False", 
//                         Message = $"Lỗi đầu vào không được để trống"
//                     });

//                 var result = await _candidateReposistory._EditCandidateBy_ID_Admin(id, UngCuVien);
//                 if(result <= 0){
//                     string errorMessage = result switch{
//                         0 =>"Đã trùng số điện thoại",
//                         -1 => "Đã trùng Email",
//                         -2 =>"Đã trùng CCCD",
//                         -3 => "Vai trò người dùng không tìm thấy",
//                         -4 =>"Đầu vào không hợp lệ hoặc bị để trống trường nào đó",
//                         -5 => "Lỗi khi thực hiện cập nhật thông tin ứng cử viên",
//                         -6 => "Lỗi khi thực hiện cập nhật tài khoản ứng cử viên",
//                         _ => "Lỗi không xác định"
//                     };
//                     return BadRequest(new {Status = "False", Message = errorMessage});
//                 }

//                 return Ok(new{
//                     Status = "Ok",
//                     Message = "Cập nhật thành công",
//                     Data = UngCuVien
//                 });
//             }catch(Exception ex){
//                 // Log lỗi và xuất ra chi tiết lỗi
//                 Console.WriteLine($"Exception Message: {ex.Message}");
//                 Console.WriteLine($"Stack Trace: {ex.StackTrace}");
//                 return StatusCode(500, new{
//                     Status = "False", 
//                     Message = $"Lỗi khi thực hiện sửa thông tin ứng cử viên: {ex.Message}"
//                 });
//             }
//         }

//         //Sửa cho ứng cử viên
//         [HttpPut("forvoter/{id}")]
//         public async Task<IActionResult> EditCandidateBy_ID_Voter(string id, CandidateDto UngCuVien){
//             try{
//                 if(UngCuVien == null || string.IsNullOrEmpty(UngCuVien.HoTen))
//                     return StatusCode(400, new{
//                         Status = "False", 
//                         Message = $"Lỗi đầu vào không được để trống"
//                     });

//                 var result = await _candidateReposistory._EditCandidateBy_ID_Candidate(id, UngCuVien);
//                 if(result <= 0){
//                     string errorMessage = result switch{
//                         0 =>"Đã trùng số điện thoại",
//                         -1 => "Đã trùng Email",
//                         -2 =>"Đã trùng CCCD",
//                         -3 => "Vai trò người dùng không tìm thấy",
//                         -4 =>"Đầu vào không hợp lệ hoặc bị để trống trường nào đó",
//                         -5 => "Lỗi khi thực hiện cập nhật thông tin ứng cử viên",
//                         -6 => "Lỗi khi thực hiện cập nhật tài khoản ứng cử viên",
//                         -7 => "Lỗi mật khẩu cũ không khớp, không thể cập nhập được",
//                         _ => "Lỗi không xác định"
//                     };
//                     return BadRequest(new {Status = "False", Message = errorMessage});
//                 }

//                 return Ok(new{
//                     Status = "Ok",
//                     Message = "Cập nhật thành công",
//                     Data = UngCuVien
//                 });
//             }catch(Exception ex){
//                 // Log lỗi và xuất ra chi tiết lỗi
//                 Console.WriteLine($"Exception Message: {ex.Message}");
//                 Console.WriteLine($"Stack Trace: {ex.StackTrace}");
//                 return StatusCode(500, new{
//                     Status = "False", 
//                     Message = $"Lỗi khi thực hiện sửa thông tin ứng cử viên: {ex.Message}"
//                 });
//             }
//         }

//         //xóa
//         [HttpDelete("{id}")]
//         public async Task<IActionResult> DeleteVoterBy_ID(string id){
//             try{
//                 var result = await _candidateReposistory._DeleteCandidateBy_ID(id);
//                 if(result == false)
//                     return StatusCode(400, new{
//                         Status = "False", 
//                         Message = $"Lỗi không tìm thấy ID_UngCuVien để xóa"
//                     });

//                 return Ok(new{
//                     Status = "Ok",
//                     Message = "Xóa thành công"
//                 });
//             }catch(Exception ex){
//                 return StatusCode(500, new{
//                     Status = "False", 
//                     Message = $"Lỗi khi thực hiện xóa tài khoản ứng cử viên: {ex.Message}"
//                 });
//             }
//         }
//     }
// }