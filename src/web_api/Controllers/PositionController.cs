using BackEnd.src.infrastructure.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using BackEnd.core.Entities;
using BackEnd.src.core.Entities;
using BackEnd.src.infrastructure.DataAccess.IRepository;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionController : ControllerBase
    {
        private readonly IPositionsRepository _positionReposistory;

        //Khởi tạo
        public PositionController(IPositionsRepository positionReposistory) => _positionReposistory = positionReposistory;

        //Liệt kê
        [HttpGet]
        public async Task<IActionResult> GetListOfPosition(){
            try{
                var result = await _positionReposistory._GetListOfPosition();
                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = result
                });
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi truy xuất danh sách các chức vụ: {ex.Message}"
                });
            }
        }

        //Thêm
        [HttpPost]
        public async Task<IActionResult> CreatePosition([FromBody] Position Position){
            try{
                //Kiểm tra đầu vào
                if(Position == null || string.IsNullOrEmpty(Position.TenChucVu))
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi khi đầu vào không được rỗng"
                    });
                
                //lấy kết quả thêm vào được hay không
                var result = await _positionReposistory._AddPosition(Position);
                if(result == false)
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi ID_ChucVu chức vụ đã bị trùng"
                    });
                
                return Ok(new{
                    Status = "OK", 
                    Message = "Thêm chức vụ thành công"
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm chức vụ: {ex.Message}"
                });
            }
        }
    
        //Lấy theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPositionBy_ID(string id){
            try{
                var Position = await _positionReposistory._GetPositionBy_ID(id);
                if(Position == null)
                    return StatusCode(400, new{
                    Status = "False", 
                    Message = $"Lỗi ID_ChucVu của chức vụ không tồn tại"
                });

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = Position
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm chức vụ: {ex.Message}"
                });
            }
        }

        //Sửa
        [HttpPut("{id}")]
        public async Task<IActionResult> EditPositionBy_ID(string id, Position Position){
            try{
                if(Position == null || string.IsNullOrEmpty(Position.TenChucVu))
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không được để trống"
                    });

                var result = await _positionReposistory._EditPositionBy_ID(id, Position);
                if(result == false)
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không hợp lệ"
                    });

                return Ok(new{
                    Status = "Ok",
                    Message = "Cập nhật thành công",
                    Data = Position
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện sửa chức vụ: {ex.Message}"
                });
            }
        }

        //xóa
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePositionBy_ID(string id){
            try{
                var result = await _positionReposistory._DeletePositionBy_ID(id);
                if(result == false)
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không hợp lệ"
                    });

                return Ok(new{
                    Status = "Ok",
                    Message = "Xóa thành công"
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện xóa chức vụ: {ex.Message}"
                });
            }
        }
    }
}