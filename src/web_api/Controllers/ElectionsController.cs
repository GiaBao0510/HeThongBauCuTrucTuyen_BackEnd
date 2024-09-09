using BackEnd.src.infrastructure.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using BackEnd.core.Entities;
using BackEnd.src.infrastructure.DataAccess.IRepository;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElectionsController : ControllerBase
    {
        private readonly IElectionsRepository _electionsReposistory;

        //Khởi tạo
        public ElectionsController(IElectionsRepository electionsReposistory) => _electionsReposistory = electionsReposistory;

        //Liệt kê
        [HttpGet]
        public async Task<IActionResult> GetListOfElections(){
            try{
                var result = await _electionsReposistory._GetListOfElections();
                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = result
                });
            }catch(Exception ex){
                Console.WriteLine($"Erro message: {ex.Message}");
                Console.WriteLine($"Erro Source: {ex.Source}");
                Console.WriteLine($"Erro InnerException: {ex.InnerException}");
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi truy xuất danh sách các kỳ bầu cử: {ex.Message}"
                });
            }
        }

        //Thêm
        [HttpPost]
        public async Task<IActionResult> CreateElections([FromBody] Elections Elections){
            try{
                //Kiểm tra đầu vào
                if(Elections == null || string.IsNullOrEmpty(Elections.TenKyBauCu))
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi khi đầu vào không được rỗng"
                    });
                
                //lấy kết quả thêm vào được hay không
                var result = await _electionsReposistory._AddElections(Elections);
                if(result == false)
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi ngayBD kỳ bầu cử đã bị trùng"
                    });
                
                return Ok(new{
                    Status = "OK", 
                    Message = "Thêm kỳ bầu cử thành công"
                });
            }catch(Exception ex){
                Console.WriteLine($"Erro message: {ex.Message}");
                Console.WriteLine($"Erro message: {ex.Source}");
                Console.WriteLine($"Erro InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy kỳ bầu cử theo ID: {ex.Message}"
                });
            }
        }
    
        //Lấy theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetElectionsBy_ID(string id){
            try{
                var Elections = await _electionsReposistory._GetElectionsBy_ID(id);
                if(Elections == null)
                    return StatusCode(400, new{
                    Status = "False", 
                    Message = $"Lỗi ngayBD của kỳ bầu cử không tồn tại"
                });

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = Elections
                });
            }catch(Exception ex){
                Console.WriteLine($"Erro message: {ex.Message}");
                Console.WriteLine($"Erro message: {ex.Source}");
                Console.WriteLine($"Erro InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm kỳ bầu cử: {ex.Message}"
                });
            }
        }

        //Sửa
        [HttpPut("{id}")]
        public async Task<IActionResult> EditElectionsBy_ID(string id, Elections Elections){
            try{
                if(Elections == null || string.IsNullOrEmpty(Elections.TenKyBauCu))
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không được để trống"
                    });

                var result = await _electionsReposistory._EditElectionsBy_ID(id, Elections);
                if(result == false)
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không hợp lệ"
                    });

                return Ok(new{
                    Status = "Ok",
                    Message = "Cập nhật thành công",
                    Data = Elections
                });
            }catch(Exception ex){
                Console.WriteLine($"Erro message: {ex.Message}");
                Console.WriteLine($"Erro message: {ex.Source}");
                Console.WriteLine($"Erro InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện sửa kỳ bầu cử: {ex.Message}"
                });
            }
        }

        //xóa
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteElectionsBy_ID(string id){
            try{
                var result = await _electionsReposistory._DeleteElectionsBy_ID(id);
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
                Console.WriteLine($"Erro message: {ex.Message}");
                Console.WriteLine($"Erro message: {ex.Source}");
                Console.WriteLine($"Erro InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện xóa kỳ bầu cử: {ex.Message}"
                });
            }
        }
    }
}