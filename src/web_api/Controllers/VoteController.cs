using Microsoft.AspNetCore.Mvc;
using BackEnd.src.web_api.DTOs;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("FixedWindowLimiter")]
    public class VoteController : ControllerBase
    {
        private readonly IVoteRepository _voteReposistory;

        //Khởi tạo
        public VoteController(IVoteRepository voteReposistory) => _voteReposistory = voteReposistory;

        //Liệt kê
        [HttpGet]
        [Authorize(Roles= "1,8")]
        public async Task<IActionResult> GetListOfVote(){
            try{
                var result = await _voteReposistory._GetListOfVote();
                
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
                    Message=$"Lỗi khi truy xuất danh sách các phiếu bầu: {ex.Message}"
                });
            }
        }

        //Thêm phiếu bởi số lượng
        [HttpPost]
        [Route("add-vote-by-number")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> CreateVoteByNumber([FromQuery] int number,[FromBody] VoteDto Vote){
            try{
                //Kiểm tra number
                if(number < 0)
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi số lượng thêm vào không được âm"
                    });

                //Kiểm tra đầu vào
                if(Vote == null || string.IsNullOrEmpty(Vote.ngayBD.ToString()))
                    return StatusCode(400,new{
                        Status = "false",
                        Message=$"Lỗi khi đầu vào không được rỗng"
                    });
                
                //lấy kết quả thêm vào được hay không
                var result = await _voteReposistory._CreateVoteByNumber(number,Vote);
                if(result <  1){
                    string ErrorMessage =  result switch{
                        0 => "Lỗi số lượng phiếu tại kỳ bầu cử đã được tạo đủ rồi",
                        -2 => "Lỗi ngayBD không tồn tại.",
                        -1 => "Lỗi dữ liệu đầu vào rỗng.",
                        -3 => "Lỗi số lượng phiều đầu vào cần tạo không được lớn hơn số lượng cử tri tối đa",
                        _=> "Lỗi không xác định"  
                    };
                    return BadRequest(new{
                        Status = "False",
                        Message = ErrorMessage
                    });
                }
                
                return Ok(new{
                    Status = "OK", 
                    Message = "Thêm phiếu bầu thành công"
                });
            }catch(Exception ex){
                Console.WriteLine($"Erro message: {ex.Message}");
                Console.WriteLine($"Erro message: {ex.Source}");
                Console.WriteLine($"Erro InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện thêm phiếu bầu: {ex.Message}"
                });
            }
        }
    
        //Lấy theo ID
        [HttpGet("get-by-id/{id}")]
        [Authorize(Roles= "1,8")]
        public async Task<IActionResult> GetVoteBy_ID(string id){
            try{
                var Vote = await _voteReposistory._GetVoteBy_ID(id);
                if(Vote == null)
                    return StatusCode(400, new{
                    Status = "False", 
                    Message = $"Lỗi ID phiếu bầu của phiếu bầu không tồn tại"
                });

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = Vote
                });
            }catch(Exception ex){
                Console.WriteLine($"Erro message: {ex.Message}");
                Console.WriteLine($"Erro message: {ex.Source}");
                Console.WriteLine($"Erro InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy ID phiếu bầu: {ex.Message}"
                });
            }
        }

        //Lấy theo ID
        [HttpGet("get-by-time/{time}")]
        [Authorize(Roles= "1,8")]
        public async Task<IActionResult> GetVoteBy_Time(string time){
            try{
                var Vote = await _voteReposistory._GetVoteBy_Time(time);
                if(Vote == null)
                    return StatusCode(400, new{
                    Status = "False", 
                    Message = $"Lỗi time phiếu bầu của phiếu bầu không tồn tại"
                });

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = Vote
                });
            }catch(Exception ex){
                Console.WriteLine($"Erro message: {ex.Message}");
                Console.WriteLine($"Erro message: {ex.Source}");
                Console.WriteLine($"Erro InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy ID phiếu bầu: {ex.Message}"
                });
            }
        }

        //Sửa
        [HttpPut("get-by-id/{id}")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> EditVoteBy_ID(string id, VoteDto Vote){
            try{
                if(Vote == null || string.IsNullOrEmpty(Vote.ngayBD.ToString()))
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không được để trống"
                    });

                var result = await _voteReposistory._EditVoteBy_ID(id, Vote);
                if(result == false)
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi đầu vào không hợp lệ"
                    });

                return Ok(new{
                    Status = "Ok",
                    Message = "Cập nhật thành công",
                    Data = Vote
                });
            }catch(Exception ex){
                Console.WriteLine($"Erro message: {ex.Message}");
                Console.WriteLine($"Erro message: {ex.Source}");
                Console.WriteLine($"Erro InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện sửa phiếu bầu: {ex.Message}"
                });
            }
        }

        //xóa
        [HttpDelete("delete-by-id/{id}")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> DeleteVoteBy_ID(string id){
            try{
                var result = await _voteReposistory._DeleteVoteBy_ID(id);
                if(result == false)
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi ID_phiếu bầu không tồn tại"
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
                    Message = $"Lỗi khi thực hiện xóa phiếu bầu: {ex.Message}"
                });
            }
        }

        //xóa dựa trên thời điểm
        [HttpDelete("delete-by-time/{thoidiem}")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> DeleteVoteBy_Time(string thoidiem){
            try{
                var result = await _voteReposistory._DeleteVoteBy_Time(thoidiem);
                if(result == false)
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi thời điểm phiếu bầu không tồn tại"
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
                    Message = $"Lỗi khi thực hiện xóa phiếu bầu: {ex.Message}"
                });
            }
        }

        //Lấy thông tin chi tiết phiếu bầu dựa trên ngày bầu cử
        [HttpGet("get-details-about-votes-based-on-election-date")]
        [Authorize(Roles= "1,8")]
        [EnableRateLimiting("SlidingWindowLimiter")]
        public async Task<IActionResult> GetDetailsAboutVotesBasedOnElectionDate([FromQuery]DateTime ngayBD){
            try{
                if(ngayBD.ToString().IsNullOrEmpty()){
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi ngày bắt đầu không được để trống"
                    });
                }

                var Vote = await _voteReposistory._getDetailsAboutVotesBasedOnElectionDate(ngayBD);
                if(Vote == null)
                    return StatusCode(400, new{
                    Status = "False", 
                    Message = $"Lỗi ngày bắt đầu không tồn tại"
                });

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = Vote
                });
            }catch(Exception ex){
                Console.WriteLine($"Erro message: {ex.Message}");
                Console.WriteLine($"Erro message: {ex.Source}");
                Console.WriteLine($"Erro InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy thông tin các phiếu bầu dựa trên ngày bầu cử: {ex.Message}"
                });
            }
        }

        //Lấy thông tin chi tiết phiếu bầu dựa trên năm
        [HttpGet("get-details-about-votes-based-on-year")]
        [Authorize(Roles= "1,8")]
        [EnableRateLimiting("SlidingWindowLimiter")]
        public async Task<IActionResult> GetDetailsAboutVotesBasedOnElectionYear([FromQuery]string year){
            try{
                if(year.ToString().IsNullOrEmpty()){
                    return StatusCode(400, new{
                        Status = "False", 
                        Message = $"Lỗi năm không được để trống"
                    });
                }

                var Vote = await _voteReposistory._getDetailsAboutVotesBasedOnElectionYear(year);
                if(Vote == null)
                    return StatusCode(400, new{
                    Status = "False", 
                    Message = $"Lỗi năm không tồn tại"
                });

                return Ok(new{
                    Status = "Ok",
                    Message = "null",
                    Data = Vote
                });
            }catch(Exception ex){
                Console.WriteLine($"Erro message: {ex.Message}");
                Console.WriteLine($"Erro message: {ex.Source}");
                Console.WriteLine($"Erro InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy thông tin các phiếu bầu dựa trên năm: {ex.Message}"
                });
            }
        }
    }
}