using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class FeedbackController: ControllerBase
    {
        private readonly IFeedbackRepository _feedbackRepository;
        public FeedbackController(IFeedbackRepository feedbackRepository) => _feedbackRepository = feedbackRepository;

        //Lấy danh sách phản hồi của cư tri
        [HttpGet("get-voter-feedback-list")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> GetVoterFeedbackList(){
            try{
                var result = await _feedbackRepository._getVoterFeedbackList();

                return Ok(new{Status = true, Message = "", data = result});

            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy danh sách phản hồi cử tri: {ex.Message}"
                });
            }
        }

        //Lấy danh sách phản hồi của cư tri
        [HttpGet("get-candidate-feedback-list")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> GetCandidateFeedbackList(){
            try{
                var result = await _feedbackRepository._getCandidateFeedbackList();

                return Ok(new{Status = true, Message = "", data = result});

            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy danh sách phản hồi cử tri: {ex.Message}"
                });
            }
        }

        //Lấy danh sách phản hồi của cán bộ
        [HttpGet("get-cadre-feedback-list")]
        [Authorize(Roles= "1")]
        public async Task<IActionResult> GetCadreFeedbackList(){
            try{
                var result = await _feedbackRepository._getCadreFeedbackList();

                return Ok(new{Status = true, Message = "", data = result});

            }catch(Exception ex){
                // Log lỗi và xuất ra chi tiết lỗi
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"Lỗi khi thực hiện lấy danh sách phản hồi cử tri: {ex.Message}"
                });
            }
        }
    }
}