using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElectionResultsController: ControllerBase
    {
        private readonly IElectionResultsRepository electionResultsRepository;
        public ElectionResultsController(IElectionResultsRepository _electionResultsRepository) => electionResultsRepository = _electionResultsRepository;

        //Lấy danh sách các kỳ bầu cử đã công bố kết quả cho cử tri
        [HttpGet("get-detailed-list-of-election-result-for-voter")]
        [Authorize(Roles= "1,5")]
        public async Task<IActionResult> GetDetailedListOfElectionResultForVoter([FromQuery] string ID_CuTri){
            try{
                if(string.IsNullOrEmpty(ID_CuTri) || string.IsNullOrEmpty(ID_CuTri))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền ID cử tri."});
                
                var result = await electionResultsRepository._getDetailedListOfElectionResultForVoter(ID_CuTri);
                if(result == null)
                    return BadRequest(new{Status = "False", Message = "ID cử tri không tồn tại"});
                
                return Ok(new{Status = true, Message = "", data = result});

            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy danh sách các kỳ bầu củ đã công bố cho cử tri: {ex.Message}"
                });
            }
        }

        //Lấy danh sách các kỳ bầu cử đã công bố kết quả cho ứng cử viên
        [HttpGet("get-detailed-list-of-election-result-for-candidate")]
        [Authorize(Roles= "1,2")]
        public async Task<IActionResult> GetDetailedListOfElectionResultForCandidate([FromQuery] string ID_ucv){
            try{
                if(string.IsNullOrEmpty(ID_ucv) || string.IsNullOrEmpty(ID_ucv))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền ID ứng cử viên."});
                
                var result = await electionResultsRepository._getDetailedListOfElectionResultForCandidate(ID_ucv);
                if(result == null)
                    return BadRequest(new{Status = "False", Message = "ID ứng cử viên không tồn tại"});
                
                return Ok(new{Status = true, Message = "", data = result});

            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy danh sách các kỳ bầu củ đã công bố cho ứng cử viên: {ex.Message}"
                });
            }
        }

        //Lấy danh sách các kỳ bầu cử đã công bố kết quả cho cán bộ
        [HttpGet("get-detailed-list-of-election-result-for-cadre")]
        [Authorize(Roles= "1,8")]
        public async Task<IActionResult> GetDetailedListOfElectionResultForCandre([FromQuery] string ID_CanBo){
            try{
                if(string.IsNullOrEmpty(ID_CanBo) || string.IsNullOrEmpty(ID_CanBo))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền ID cán bộ."});
                
                var result = await electionResultsRepository._getDetailedListOfElectionResultForCandre(ID_CanBo);
                if(result == null)
                    return BadRequest(new{Status = "False", Message = "ID cán bộ không tồn tại"});
                
                return Ok(new{Status = true, Message = "", data = result});

            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy danh sách các kỳ bầu củ đã công bố cho cán bộ: {ex.Message}"
                });
            }
        }

        //Lấy danh sách kết quả kỳ bầu cử đã công bố dựa trên ngày bầu cử
        [HttpGet("list-of-elections-result-announced")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> ListOfElectionsResultAnnounced([FromQuery] string ngayBD){
            try{
                if(string.IsNullOrEmpty(ngayBD))
                    return BadRequest(new{Status = "False", Message = "Vui lòng điền ngày bắt đầu."});
                
                var result = await electionResultsRepository._ListOfElectionsResultAnnounced(ngayBD);

                return Ok(new{Status = true, Message = "", data = result});

            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy danh sách các kỳ bầu củ đã công bố cho ứng cử viên: {ex.Message}"
                });
            }
        }
    }
}