using BackEnd.src.infrastructure.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using BackEnd.core.Entities;

namespace HeThongBauCuTrucTuyen_BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConstituencyController: ControllerBase
    {
        private readonly ConstituencyReposistory _constituencyReposistory;

        //Khởi tạo
        public ConstituencyController(ConstituencyReposistory constituencyReposistory) => _constituencyReposistory = constituencyReposistory;

        //Liệt kê
        [HttpGet]
        public async Task<IActionResult> GetListOfConstituency(){
            try{
                var result = await _constituencyReposistory._GetListOfConstituency();
                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = result
                });
            }catch(Exception ex){
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi truy xuất danh sách các Đơn vị bầu cử: {ex.Message}"
                });
            }
        }

        //Thêm
        [HttpPost]
        public async Task<IActionResult> CreateConstituency([FromBody] Constituency Constituency){
            try{
                //Kiểm tra đầu vào
                if(Constituency == null || string.IsNullOrEmpty(Constituency.TenDonViBauCu))
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi khi đầu vào không được rỗng"
                    });
                
                //lấy kết quả thêm vào được hay không
                var result = await _constituencyReposistory._AddConstituency(Constituency);
                if(result == false)
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi STT Đơn vị bầu cử đã bị trùng"
                    });
                
                return Ok(new{
                    Status = "OK", 
                    Message = "Thêm Đơn vị bầu cử thành công"
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm Đơn vị bầu cử: {ex.Message}"
                });
            }
        }
    
        //Lấy theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetConstituencyBy_ID(string id){
            try{
                var Constituency = await _constituencyReposistory._GetConstituencyBy_ID(id);
                if(Constituency == null)
                    return StatusCode(400, new{
                    Status = "False", 
                    Message = $"Lỗi STT của đơn vị bầu cử không tồn tại"
                });

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = Constituency
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm Đơn vị bầu cử: {ex.Message}"
                });
            }
        }

        //Sửa
        [HttpPut("{id}")]
        public async Task<IActionResult> EditConstituencyBy_ID(string id, Constituency Constituency){
            try{
                if(Constituency == null || string.IsNullOrEmpty(Constituency.TenDonViBauCu))
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không được để trống"
                    });

                var result = await _constituencyReposistory._EditConstituencyBy_ID(id, Constituency);
                if(result == false)
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không hợp lệ"
                    });

                return Ok(new{
                    Status = "Ok",
                    Message = "Cập nhật thành công",
                    Data = Constituency
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện sửa Đơn vị bầu cử: {ex.Message}"
                });
            }
        }

        //xóa
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConstituencyBy_ID(string id){
            try{
                var result = await _constituencyReposistory._DeleteConstituencyBy_ID(id);
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
                    Message = $"Lỗi khi thực hiện xóa Đơn vị bầu cử: {ex.Message}"
                });
            }
        }

    }
}