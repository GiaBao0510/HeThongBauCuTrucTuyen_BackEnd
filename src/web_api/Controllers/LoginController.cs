
using BackEnd.src.core.Models;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginHistoryController: ControllerBase
    {    
        private ILoginHistoryRepository _loginHistoryRepository;

        public LoginHistoryController(ILoginHistoryRepository loginHistoryRepository) => _loginHistoryRepository = loginHistoryRepository; 

        [HttpGet("login-list-history")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> GetLoginHistoryList(){
            try{
                var list = await _loginHistoryRepository._GetLoginHistoryList();
                return Ok(new ApiRespons{
                    Success = true,
                    Message = "null",
                    Data = list
                });
            }catch(Exception ex){
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Error Data: {ex.Data}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine($"Error InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "false",
                    Message=$"Error Message: {ex.Message}"
                });
            }
        }

        [HttpGet("login-list-history-by-voter")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> GetListOfLoginHistory_byVote(){
            try{
                var list = await _loginHistoryRepository._GetListOfLoginHistory_byVote();
                return Ok(new ApiRespons{
                    Success = true,
                    Message = "null",
                    Data = list
                });
            }catch(Exception ex){
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Error Data: {ex.Data}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine($"Error InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "false",
                    Message=$"Error Message: {ex.Message}"
                });
            }
        }

        [HttpGet("login-list-history-by-cadre")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> GetListOfLoginHistory_byCadre(){
            try{
                var list = await _loginHistoryRepository._GetListOfLoginHistory_byCadre();
                return Ok(new ApiRespons{
                    Success = true,
                    Message = "null",
                    Data = list
                });
            }catch(Exception ex){
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Error Data: {ex.Data}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine($"Error InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "false",
                    Message=$"Error Message: {ex.Message}"
                });
            }
        }

        [HttpGet("login-list-history-by-candidate")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> GetListOfLoginHistory_byCandidate(){
            try{
                var list = await _loginHistoryRepository._GetListOfLoginHistory_byCandidate();
                return Ok(new ApiRespons{
                    Success = true,
                    Message = "null",
                    Data = list
                });
            }catch(Exception ex){
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Error Data: {ex.Data}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine($"Error InnerException: {ex.InnerException}");
                return StatusCode(500, new{
                    Status = "false",
                    Message=$"Error Message: {ex.Message}"
                });
            }
        }
    }
}