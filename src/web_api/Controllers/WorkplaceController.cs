using BackEnd.src.core.Entities;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using BackEnd.src.web_api.DTOs;
using System;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkplaceController : ControllerBase
    {
        private readonly IWorkPlaceRepository _IWorkPlaceRepository;

        //Khởi tạo
        public WorkplaceController(IWorkPlaceRepository IWorkPlaceRepository) => _IWorkPlaceRepository = IWorkPlaceRepository;

        //1. Liệt kê các ID ở bảng hoạt động
        [HttpGet]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> GetWorkplaces(){
            try{
                var result = await _IWorkPlaceRepository._GetWorkplaces();

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = result
                });
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi truy xuất danh sách các hoạt động: {ex.Message}"
                });
            }
        }

        //1. Liệt kê các ID ở bảng hoạt động
        [HttpGet("Details")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> GetListOfCadre(){
            try{
                var result = await _IWorkPlaceRepository._GetWorkplacesDetail();

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = result
                });
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi truy xuất danh sách các hoạt động: {ex.Message}"
                });
            }
        }

        //3. Thêm
        [HttpPost]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> AddDataToTheWorkplace([FromBody] WorkPlace workplace){
            try{
                var result = await _IWorkPlaceRepository._AddDataToTheWorkplace(workplace);

                if(result <=0){
                    string errorMessage = result switch{
                      0 => "Mã chức vụ không tồn tại",
                      -1 => "Mã cán bộ không tồn tại",
                      -2 => "Mã ban không tồn tại", 
                      _ => "Lỗi không xác định" 
                    };
                    return BadRequest(new{Status = "false",Message=errorMessage});
                }
                
                return Ok(new{
                    Status = "Ok",
                    Message = "Thêm thông tin hoạt động thành công"
                });
            }
            catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi thêm thông tin hoạt động: {ex.Message}"
                });
            }
        }

        //4. Cập nhật dữ liệu nơi công tác- mã cán bộ
        [HttpPut("ChangeTheBoardWhereCadreWork")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> UpdateWorkplaceBy_IDcadre([FromBody] WorkPlaceDto workPlace){
            try{
                if(string.IsNullOrEmpty(workPlace.ID_canbo) || string.IsNullOrEmpty(workPlace.ID_Ban.ToString())){
                    return BadRequest(new{status = false, Message = "Vui lòng điền đầy đủ thông tin cần thay đổi"});
                }

                var result = await _IWorkPlaceRepository._UpdateWorkplaceBy_IDcadre(workPlace);
                if(result == false)
                    return BadRequest(new {Status=false, Mesage="Không thể cập nhật vì thông tin đầu vào không hợp lệ"});
                
                return Ok(new {status = true, Message="Cập nhật ban mà cán bộ làm việc thành công"}); 
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi thay đổi thông tin hoạt động: {ex.Message}"
                });
            }
        }

        //5. Cập nhật dữ liệu nơi công tác- mã chức vụ
        [HttpPut("ChangeTheWorkplaceByPosition")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> UpdateWorkplaceBy_IDposition([FromBody] WorkPlaceDto workPlace){
            try{
                if(string.IsNullOrEmpty(workPlace.ID_ChucVu.ToString()) || string.IsNullOrEmpty(workPlace.ID_Ban.ToString())){
                    return BadRequest(new{status = false, Message = "Vui lòng điền đầy đủ thông tin cần thay đổi"});
                }

                var result = await _IWorkPlaceRepository._UpdateWorkplaceBy_IDposition(workPlace);
                if(result == false)
                    return BadRequest(new {Status=false, Mesage="Không thể cập nhật vì thông tin đầu vào không hợp lệ"});

                return Ok(new {status = true, Message="Cập nhật ban theo chức vụ thành công"}); 
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi thay đổi thông tin hoạt động: {ex.Message}"
                });
            }
        }

        //6. Cập nhật dữ liệu chức vụ theo mã ban
        [HttpPut("ChangeThePositionByWorkplace")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> _UpdateWorkplaceBy_IDboard([FromBody] WorkPlaceDto workPlace){
            try{
                if(string.IsNullOrEmpty(workPlace.ID_ChucVu.ToString()) || string.IsNullOrEmpty(workPlace.ID_Ban.ToString())){
                    return BadRequest(new{status = false, Message = "Vui lòng điền đầy đủ thông tin cần thay đổi"});
                }

                var result = await _IWorkPlaceRepository._UpdateWorkplaceBy_IDboard(workPlace);
                if(result == false)
                    return BadRequest(new {Status=false, Mesage="Không thể cập nhật vì thông tin đầu vào không hợp lệ"});

                return Ok(new {status = true, Message="Cập nhật chức vụ theo ban thành công"}); 
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi thay đổi thông tin hoạt động: {ex.Message}"
                });
            }
        }

        //7. Xóa dữ liệu nơi công tác theo mã cán bộ
        [HttpDelete("DeleteByCadre/{id}")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> DeleteWorkplaceBy_IDcadre(string id){
            try{
                var result = await _IWorkPlaceRepository._DeleteWorkplaceBy_IDcadre(id);
                if(result == false)
                    return BadRequest(new {Status=false, Mesage="Không thể xóa vì không tìm thấy ID"});

                return Ok(new {status = true, Message="Xóa cán bộ khỏi ban làm việc thành công"}); 
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi thay đổi thông tin hoạt động: {ex.Message}"
                });
            }
        }

        //8. Xóa dữ liệu nơi công tác theo mã chức vụ
        [HttpDelete("DeleteByPosition/{id}")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> DeleteWorkplaceBy_IDposition(string id){
            try{
                var result = await _IWorkPlaceRepository._DeleteWorkplaceBy_IDposition( Convert.ToInt32( id));
                if(result == false)
                    return BadRequest(new {Status=false, Mesage="Không thể xóa vì không tìm thấy ID"});

                return Ok(new {status = true, Message="Xóa chức vụ tại phòng ban tại chỗ hoạt động thành công"}); 
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi thay đổi thông tin hoạt động: {ex.Message}"
                });
            }
        }

        //9. Xóa dữ liệu nơi công tác theo mã ban
        [HttpDelete("DeleteByBoard/{id}")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> DeleteWorkplaceBy_IDboard(string id){
            try{
                var result = await _IWorkPlaceRepository._DeleteWorkplaceBy_IDboard( Convert.ToInt32( id));
                if(result == false)
                    return BadRequest(new {Status=false, Mesage="Không thể xóa vì không tìm thấy ID"});

                return Ok(new {status = true, Message="Xóa phòng ban làm việc tại chỗ hoạt động thành công"}); 
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi thay đổi thông tin hoạt động: {ex.Message}"
                });
            }
        }

        //10. Đặt lại ban mà cán bộ đã làm việc theo -ID cán bộ
        [HttpPut("SetPositionForCadre")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> SetPositionForCadre([FromBody] WorkPlaceDto workPlace){
            try{
                if(string.IsNullOrEmpty(workPlace.ID_canbo) || string.IsNullOrEmpty(workPlace.ID_ChucVu.ToString())){
                    return BadRequest(new{status = false, Message = "Vui lòng điền đầy đủ thông tin cần đặt lại vị trí cho cán bộ"});
                }

                var result = await _IWorkPlaceRepository._UpdatePositionBy_IDcadre(workPlace.ID_canbo,workPlace.ID_ChucVu);
                if(result == false)
                    return BadRequest(new {Status=false, Mesage="Không thể cập nhật vì thông tin đầu vào không hợp lệ"});
                
                return Ok(new {status = true, Message="Cập nhật chức vụ cho cán bộ thành công"});
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi thay đổi thông tin hoạt động: {ex.Message}"
                });
            }
        }

        //11. Đặt lại phòng ban làm việc cho cán bộ
        [HttpPut("SetTheOfficeForCadre")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> SetTheOfficeForCadre([FromBody] WorkPlaceDto workPlace){
            try{
                if(string.IsNullOrEmpty(workPlace.ID_canbo) || string.IsNullOrEmpty(workPlace.ID_Ban.ToString())){
                    return BadRequest(new{status = false, Message = "Vui lòng điền đầy đủ thông tin cần đặt lại phòng ban làm việc cho cán bộ"});
                }

                var result = await _IWorkPlaceRepository._UpdateBoardBy_IDcadre(workPlace.ID_canbo,workPlace.ID_Ban);
                if(result == false)
                    return BadRequest(new {Status=false, Mesage="Không thể cập nhật vì thông tin đầu vào không hợp lệ"});
                
                return Ok(new {status = true, Message="Cập nhật phòng ban làm việc cho cán bộ thành công"});
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi thay đổi thông tin hoạt động: {ex.Message}"
                });
            }
        }
    }
}