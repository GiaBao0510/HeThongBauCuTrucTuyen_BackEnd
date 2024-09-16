using Microsoft.AspNetCore.Mvc;
using BackEnd.core.Entities;
using BackEnd.src.infrastructure.DataAccess.Repositories;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvinceController: ControllerBase
    {
        private readonly IProvinceRepository _ProvinceReposistory;

        //Khởi tạo
        public ProvinceController(IProvinceRepository provinceReposistory) => _ProvinceReposistory = provinceReposistory;

        //Liệt kê
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetListOfProvince(){
            try{
                var result = await _ProvinceReposistory._GetListOfProvice();
                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = result
                });
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi truy xuất danh sách các tỉnh thành: {ex.Message}"
                });
            }
        }

        //Thêm
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProvince([FromBody] Province province){
            try{
                //Kiểm tra đầu vào
                if(province == null || string.IsNullOrEmpty(province.TenTinhThanh))
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi khi đầu vào không được rỗng"
                    });
                
                //lấy kết quả thêm vào được hay không
                var result = await _ProvinceReposistory._AddProvince(province);
                if(result == false)
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi STT tỉnh thành đã bị trùng"
                    });
                
                return Ok(new{
                    Status = "OK", 
                    Message = "Thêm tỉnh thành thành công"
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm tỉnh thành: {ex.Message}"
                });
            }
        }
    
        //Lấy theo ID
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetProvinceBy_ID(string id){
            try{
                var province = await _ProvinceReposistory._GetProvinceBy_ID(id);
                if(province == null)
                    return StatusCode(400, new{
                    Status = "False", 
                    Message = $"Lỗi STT của tinh thành không tồn tại"
                });

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = province
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm tỉnh thành: {ex.Message}"
                });
            }
        }

        //Sửa
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> EditProvinceBy_ID(string id, Province province){
            try{
                if(province == null || string.IsNullOrEmpty(province.TenTinhThanh))
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không được để trống"
                    });

                var result = await _ProvinceReposistory._EditProvinceBy_ID(id, province);
                if(result == false)
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không hợp lệ"
                    });

                return Ok(new{
                    Status = "Ok",
                    Message = "Cập nhật thành công",
                    Data = province
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện sửa tỉnh thành: {ex.Message}"
                });
            }
        }

        //xóa
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProvinceBy_ID(string id){
            try{
                var result = await _ProvinceReposistory._DeleteProvinceBy_ID(id);
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
                    Message = $"Lỗi khi thực hiện xóa tỉnh thành: {ex.Message}"
                });
            }
        }
    
    }
}