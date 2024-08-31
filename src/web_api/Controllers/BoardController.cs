using BackEnd.src.infrastructure.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using BackEnd.core.Entities;


namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardController : ControllerBase
    {
         private readonly BoardReposistory _boardReposistory;

        //Khởi tạo
        public BoardController(BoardReposistory boardReposistory) => _boardReposistory = boardReposistory;

        //Liệt kê
        [HttpGet]
        public async Task<IActionResult> GetListOfBoard(){
            try{
                var result = await _boardReposistory._GetListOfBoard();
                
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
                return StatusCode(500,new{
                    Status = "false",
                    Message=$"Lỗi khi truy xuất danh sách các ban: {ex.Message}"
                });
            }
        }

        //Thêm
        [HttpPost]
        public async Task<IActionResult> CreateBoard([FromBody] Board Board){
            try{
                //Kiểm tra đầu vào
                if(Board == null || string.IsNullOrEmpty(Board.TenBan))
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi khi đầu vào không được rỗng"
                    });
                
                //lấy kết quả thêm vào được hay không
                var result = await _boardReposistory._AddBoard(Board);
                if(result == false)
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi ID_DonViBauCu không tồn tại."
                    });
                
                return Ok(new{
                    Status = "OK", 
                    Message = "Thêm ban thành công"
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm ban: {ex.Message}"
                });
            }
        }
    
        //Lấy theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBoardBy_ID(string id){
            try{
                var Board = await _boardReposistory._GetBoardBy_ID(id);
                if(Board == null)
                    return StatusCode(400, new{
                    Status = "False", 
                    Message = $"Lỗi ID_Ban của ban không tồn tại"
                });

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = Board
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy ID ban: {ex.Message}"
                });
            }
        }

        //Sửa
        [HttpPut("{id}")]
        public async Task<IActionResult> EditBoardBy_ID(string id, Board Board){
            try{
                if(Board == null || string.IsNullOrEmpty(Board.TenBan))
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không được để trống"
                    });

                var result = await _boardReposistory._EditBoardBy_ID(id, Board);
                if(result == false)
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không hợp lệ"
                    });

                return Ok(new{
                    Status = "Ok",
                    Message = "Cập nhật thành công",
                    Data = Board
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện sửa ban: {ex.Message}"
                });
            }
        }

        //xóa
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoardBy_ID(string id){
            try{
                var result = await _boardReposistory._DeleteBoardBy_ID(id);
                if(result == false)
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi ID_Ban không tồn tại"
                    });

                return Ok(new{
                    Status = "Ok",
                    Message = "Xóa thành công"
                });
            }catch(Exception ex){
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện xóa ban: {ex.Message}"
                });
            }
        }

    }
}