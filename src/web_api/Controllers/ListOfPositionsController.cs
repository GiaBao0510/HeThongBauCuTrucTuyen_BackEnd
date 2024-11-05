using BackEnd.src.web_api.DTOs;
using Microsoft.AspNetCore.Mvc;
using BackEnd.core.Entities;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListOfPositionsController : ControllerBase
    {
        private readonly IListOfPositionRepository _listOfPositionReposistory;

        //Khởi tạo
        public ListOfPositionsController(IListOfPositionRepository listOfPositionReposistory) => _listOfPositionReposistory = listOfPositionReposistory;

        //Liệt kê
        [HttpGet]
        [Authorize(Roles= "1,2,5,8")]
        public async Task<IActionResult> GetListOfListOfPositions(){
            try{
                var result = await _listOfPositionReposistory._GetListOfListOfPositions();
                
                if(result.Count == 0)
                    return StatusCode(200, new{
                        Status = "Ok",
                        Message = "Danh sách rỗng",
                    });
                
                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = result
                });
            }catch(Exception ex){
                Console.WriteLine($"Erro message: {ex.Message}");
                Console.WriteLine($"Erro message: {ex.Source}");
                Console.WriteLine($"Erro InnerException: {ex.InnerException}");
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi truy xuất danh sách các danh mục ứng cử: {ex.Message}"
                });
            }
        }

        //Thêm
        [HttpPost]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> CreateListOfPositions([FromBody] ListOfPositions ListOfPositions){
            try{
                //Kiểm tra đầu vào
                if(ListOfPositions == null || string.IsNullOrEmpty(ListOfPositions.TenCapUngCu))
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi khi đầu vào không được rỗng"
                    });
                
                //lấy kết quả thêm vào được hay không
                var result = await _listOfPositionReposistory._AddListOfPositions(ListOfPositions);
                if(result == false)
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi ID_DonViBauCu không tồn tại."
                    });
                
                return Ok(new{
                    Status = "OK", 
                    Message = "Thêm danh mục ứng cử thành công"
                });
            }catch(Exception ex){
                Console.WriteLine($"Erro message: {ex.Message}");
                Console.WriteLine($"Erro message: {ex.Source}");
                Console.WriteLine($"Erro InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm danh mục ứng cử: {ex.Message}"
                });
            }
        }
    
        //Lấy theo ID
        [HttpGet("{id}")]
        [Authorize(Roles= "1,2,5,8")]
        public async Task<IActionResult> GetListOfPositionsBy_ID(string id){
            try{
                var ListOfPositions = await _listOfPositionReposistory._GetListOfPositionsBy_ID(id);
                if(ListOfPositions == null)
                    return StatusCode(400, new{
                    Status = "False", 
                    Message = $"Lỗi ID danh mục ứng cử của danh mục ứng cử không tồn tại"
                });

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = ListOfPositions
                });
            }catch(Exception ex){
                Console.WriteLine($"Erro message: {ex.Message}");
                Console.WriteLine($"Erro message: {ex.Source}");
                Console.WriteLine($"Erro InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy ID danh mục ứng cử: {ex.Message}"
                });
            }
        }

        //Sửa
        [HttpPut("{id}")]
        [Authorize(Roles= "1,2,5,8")]
        public async Task<IActionResult> EditListOfPositionsBy_ID(string id,[FromBody] ListOfPositionDTO ListOfPositions){
            try{
                if(ListOfPositions == null || string.IsNullOrEmpty(ListOfPositions.TenCapUngCu))
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không được để trống"
                    });

                var result = await _listOfPositionReposistory._EditListOfPositionsBy_ID(id, ListOfPositions);
                if(result == false)
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không hợp lệ"
                    });

                return Ok(new{
                    Status = "Ok",
                    Message = "Cập nhật thành công",
                    Data = ListOfPositions
                });
            }catch(Exception ex){
                Console.WriteLine($"Erro message: {ex.Message}");
                Console.WriteLine($"Erro message: {ex.Source}");
                Console.WriteLine($"Erro InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện sửa danh mục ứng cử: {ex.Message}"
                });
            }
        }

        //xóa
        [HttpDelete("{id}")]
        [Authorize(Roles= "1,2,5,8")]
        public async Task<IActionResult> DeleteListOfPositionsBy_ID(string id){
            try{
                var result = await _listOfPositionReposistory._DeleteListOfPositionsBy_ID(id);
                if(result == false)
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi ID_danh mục ứng cử không tồn tại"
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
                    Message = $"Lỗi khi thực hiện xóa danh mục ứng cử: {ex.Message}"
                });
            }
        }
    }
}