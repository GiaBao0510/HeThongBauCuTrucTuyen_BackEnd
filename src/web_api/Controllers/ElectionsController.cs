using BackEnd.src.infrastructure.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using BackEnd.core.Entities;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.AspNetCore.Authorization;
using BackEnd.src.web_api.DTOs;

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
        [Authorize(Roles = "1,2,5,8")]
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
        public async Task<IActionResult> CreateElections([FromBody] ElectionTempDTO elections)
        {
            try
            {
                // Kiểm tra elections không null
                if (elections == null)
                {
                    return BadRequest(new
                    {
                        Status = "false",
                        Message = "Dữ liệu đầu vào không hợp lệ"
                    });
                }

                // Kiểm tra TenKyBauCu
                if (string.IsNullOrWhiteSpace(elections.TenKyBauCu))
                {
                    return BadRequest(new
                    {
                        Status = "false",
                        Message = "Tên kỳ bầu cử không được để trống"
                    });
                }

                // Log an toàn hơn
                Console.WriteLine($">>>.Thêm kỳ bầu cử: {elections.TenKyBauCu}");

                // Validate các trường bắt buộc khác
                if (elections.SoLuongToiDaCuTri <= 0 || 
                    elections.SoLuongToiDaUngCuVien <= 0 || 
                    elections.SoLuotBinhChonToiDa <= 0)
                {
                    return BadRequest(new
                    {
                        Status = "false",
                        Message = "Các giá trị số lượng phải lớn hơn 0"
                    });
                }

                // Validate ngày tháng
                if (elections.ngayBD == default || elections.ngayKT == default)
                {
                    return BadRequest(new
                    {
                        Status = "false",
                        Message = "Ngày bắt đầu và ngày kết thúc không hợp lệ"
                    });
                }

                if (elections.ngayBD > elections.ngayKT)
                {
                    return BadRequest(new
                    {
                        Status = "false",
                        Message = "Ngày bắt đầu kỳ bầu cử không được lớn hơn ngày kết thúc"
                    });
                }

                // Validate ID_Cap
                if (elections.ID_Cap <= 0)
                {
                    return BadRequest(new
                    {
                        Status = "false",
                        Message = "ID cấp không hợp lệ"
                    });
                }

                var result = await _electionsReposistory._AddElections(elections);
                if (result <= 0)
                {
                    string errorMessage = result switch
                    {
                        0 => "ID danh mục bầu cử không tồn tại",
                        -1 => "Không tìm thấy nơi lưu khóa cá nhân",
                        _ => "Lỗi không xác định"
                    };
                    int statusCode = result switch
                    {
                        0 => 400,
                        -1 => 400,
                        _ => 500
                    };
                    return StatusCode(statusCode, new { Status = "False", Message = errorMessage });
                }

                return Ok(new
                {
                    Status = "OK",
                    Message = "Thêm kỳ bầu cử thành công"
                });
            }
            catch (Exception ex)
            {
                // Log chi tiết lỗi
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Error StackTrace: {ex.StackTrace}");
                
                return StatusCode(500, new
                {
                    Status = "False",
                    Message = "Lỗi server khi thêm kỳ bầu cử"
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

        //Liệt kê
        [HttpGet("get-details-list-of-election-bassed-on-year")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetDetailsListOfElectionBassedOnYear([FromQuery] string year){
            try{
                var result = await _electionsReposistory._getDetailsListOfElectionBassedOnYear(year);
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
                    Message=$"Lỗi khi truy xuất danh sách chi tiết các kỳ bầu cử: {ex.Message}"
                });
            }
        }

        //Liệt kê danh sách cử tri chưa tham dự bầu cử
        [HttpGet("list-of-voters-who-have-not-yet-participated-election")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> ListOfVotersWhoHaveNotYetParticipatedElection([FromQuery] string ngayBD){
            try{
                var result = await _electionsReposistory._listOfVotersWhoHaveNotYetParticipatedElection(ngayBD);
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
                    Message=$"Lỗi khi truy xuất danh sách chi tiết các kỳ bầu cử: {ex.Message}"
                });
            }
        }

        //Liệt kê danh sách ứng cử viên chưa tham dự bầu cử
        [HttpGet("list-of-candidates-who-have-not-yet-participated-election")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> ListOfCandidatesWhoHaveNotYetParticipatedElection([FromQuery] string ngayBD){
            try{
                var result = await _electionsReposistory._listOfCandidatesWhoHaveNotYetParticipatedElection(ngayBD);
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
                    Message=$"Lỗi khi truy xuất danh sách chi tiết các kỳ bầu cử: {ex.Message}"
                });
            }
        }

        //Liệt kê danh sách cán bộ chưa tham dự bầu cử
        [HttpGet("list-of-cadres-who-have-not-yet-participated-election")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> ListOfCadresWhoHaveNotYetParticipatedElection([FromQuery] string ngayBD){
            try{
                var result = await _electionsReposistory._listOfCadresWhoHaveNotYetParticipatedElection(ngayBD);
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
                    Message=$"Lỗi khi truy xuất danh sách chi tiết các kỳ bầu cử: {ex.Message}"
                });
            }
        }
    }
}