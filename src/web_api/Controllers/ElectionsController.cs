using BackEnd.src.infrastructure.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using BackEnd.core.Entities;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Roles = "1,2,5")]
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

        //Liệt kê các kỳ bầu cử trong tương lai
        [HttpGet("get-list-of-future-elections")]
        [Authorize(Roles = "1,2,5")]
        public async Task<IActionResult> GetListOfFutureElections(){
            try{
                var result = await _electionsReposistory._GetListOfFutureElections();
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
        [Authorize(Roles = "1")]
        public async Task<IActionResult> CreateElections([FromBody] Elections Elections){
            try{
                //Kiểm tra đầu vào
                if(Elections == null || string.IsNullOrEmpty(Elections.TenKyBauCu))
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi khi đầu vào không được rỗng"
                    });

                //Nếu ngày bắt đầu lớn hơn ngày kết thúc thì trả về lỗi
                if(Elections.ngayBD > Elections.ngayKT)
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi ngày bắt đầu kỳ bầu cử không được lớn hơn ngày kết thúc"
                    });
                
                //lấy kết quả thêm vào được hay không
                var result = await _electionsReposistory._AddElections(Elections);
                if(result <= 0){
                    string errorMessage = result switch{
                        0 => "ID danh mục bầu cử không tồn tại",
                        -1 =>"Không tìm thấy nơi lưu khóa cá nhân",
                        _ => "Lỗi không xác định"
                    };
                    int statusCode = result switch{
                        0 => 400, -1 =>400,  _ => 500
                    };
                    Console.WriteLine($"Kết quả: {result}");
                    return StatusCode(statusCode ,new {Status = "False", Message = errorMessage});
                }
                
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
        [Authorize(Roles = "1,2,5,8")]
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
        [Authorize(Roles = "1")]
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
        [Authorize(Roles = "1")]
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

        //Lấy danh sách hợ tên và ID ứng cử viên theo kỳ bầu cử
        [HttpGet("get-a-list-of-candidate-name-basedon-elctions")]
        [Authorize(Roles = "1,2,3,4,5,8")]
        public async Task<IActionResult> GetListCandidateNamesBasedOnElections([FromQuery] DateTime ngayBD){
            try{
                if(string.IsNullOrEmpty(ngayBD.ToString()))
                    return StatusCode(500, new{
                        Status = "False", 
                        Message = $"Lỗi vui lòng điền ngày bắt đầu bầu cử để lấy danh sách ứng cử viên"
                    });
                var result = await _electionsReposistory._GetListCandidateNamesBasedOnElections(ngayBD);
                if(result == null)
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi ngày bắt đầu bầu cử không tồn tại"
                    });

                return Ok(new{
                    Status = "Ok",
                    Message = "",
                    Data = result
                });
            }catch(Exception ex){
                Console.WriteLine($"Erro message: {ex.Message}");
                Console.WriteLine($"Erro message: {ex.Source}");
                Console.WriteLine($"Erro InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện Lấy danh sách tên ứng cử viên cử dựa trên ngày bầu cử: {ex.Message}"
                });
            }
        }
    }
}