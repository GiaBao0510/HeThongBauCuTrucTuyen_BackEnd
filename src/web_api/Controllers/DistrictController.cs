
using Microsoft.AspNetCore.Mvc;
using BackEnd.src.web_api.DTOs;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.AspNetCore.Authorization;

namespace HeThongBauCuTrucTuyen_BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistrictController: ControllerBase
    {
        private readonly IDistrictRepository _districtReposistory;
        public DistrictController(IDistrictRepository districtReposistory) => _districtReposistory = districtReposistory;

        //Liệt kê
        [HttpGet]
        [Route("all")]
        [Authorize(Roles = "1,2,5,8")]
        public async Task<IActionResult> GetListOfDistrict(){
            try{
                var result = await _districtReposistory._GetListOfDistrict();
                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = result
                });
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi truy xuất danh sách các quận huyện: {ex.Message}"
                });
            }
        }

        //Thêm
        [HttpPost]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> CreateDistrict([FromBody] DistrictDto District){
            try{
                //Kiểm tra đầu vào
                if(District == null || string.IsNullOrEmpty(District.TenQH))
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi khi đầu vào không được rỗng"
                    });
                
                //lấy kết quả thêm vào được hay không
                var result = await _districtReposistory._AddDistrict(District);
                if(result == false)
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi STT không tồn tại"
                    });
                
                return Ok(new{
                    Status = "OK", 
                    Message = "Thêm quận huyện thành công"
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm quận huyện: {ex.Message}"
                });
            }
        }
    
        //Lấy theo ID
        [HttpGet("{id}")]
        [Authorize(Roles = "1,2,5,8")]
        public async Task<IActionResult> GetDistrictBy_ID(string id){
            try{
                var District = await _districtReposistory._GetDistrictBy_ID(id);
                if(District == null)
                    return StatusCode(400, new{
                    Status = "False", 
                    Message = $"Lỗi ID_QH của quận huyện không tồn tại"
                });

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = District
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm quận huyện: {ex.Message}"
                });
            }
        }

        //Sửa
        [HttpPut("{id}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> EditDistrictBy_ID(string id,[FromBody] DistrictDto District){
            try{
                if(District == null || string.IsNullOrEmpty(District.TenQH))
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không được để trống"
                    });

                var result = await _districtReposistory._EditDistrictBy_ID(id, District);
                if(result == false)
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không hợp lệ"
                    });

                return Ok(new{
                    Status = "Ok",
                    Message = "Cập nhật thành công",
                    Data = District
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện sửa quận huyện: {ex.Message}"
                });
            }
        }

        //xóa
        [HttpDelete("{id}")]
        [Authorize(Roles = "1,2,5,8")]
        public async Task<IActionResult> DeleteDistrictBy_ID(string id){
            try{
                var result = await _districtReposistory._DeleteDistrictBy_ID(id);
                if(result == false)
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi không tìm thấy ID_QH để xóa"
                    });

                return Ok(new{
                    Status = "Ok",
                    Message = "Xóa thành công"
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện xóa quận huyện: {ex.Message}"
                });
            }
        }

        //Lấy theo danh sách quận huyên theo STT 
        [HttpGet]
        [Route("byTinhThanh")]
        [Authorize(Roles = "1,2,5,8")]
        public async Task<IActionResult> GetListOfDistrictBy_STT([FromQuery] string stt){
            try{
                var District = await _districtReposistory._GetListOfDistrictBy_STT(stt);
                if(District == null)
                    return StatusCode(400, new{
                    Status = "False", 
                    Message = $"Lỗi STT của tỉnh thành không tồn tại"
                });

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = District
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy danh sách quận huyện theo STT: {ex.Message}"
                });
            }
        }

    }
}