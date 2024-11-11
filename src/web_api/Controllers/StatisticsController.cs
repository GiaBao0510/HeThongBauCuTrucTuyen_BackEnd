using BackEnd.src.core.Models;
using BackEnd.src.infrastructure.DataAccess.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.src.web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles= "1")]
    public class StatisticsController: ControllerBase
    {
        private readonly IStatisticsRepository _statisticsRepository;
        
        public StatisticsController(IStatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
        }

        //-1. Triển khai tầng controller
        private async Task<IActionResult> ImplermentControllerLayer<T>(
            Func<Task<T>> query,
            string errorMessage,
            string queryDescription
        ){
            try{
                var result = await query();
                return Ok(new ApiRespons{
                    Success = true,
                    Message = "",
                    Data = new {
                        count = result,
                    },
                });
            }catch(Exception ex){
                Console.WriteLine($"Error message: {ex.Message}");
                Console.WriteLine($"Error TargetSite: {ex.TargetSite}");
                Console.WriteLine($"Error Source: {ex.Source}");
                Console.WriteLine($"Error HResult: {ex.HResult}");
                Console.WriteLine($"Error in {queryDescription}: {ex.Message}");
                return StatusCode(500, new{
                    Status = "False", 
                    Message = $"${errorMessage}: {ex.Message}"
                });
            }
        }

        //0. Kiểm tra năm đầu vào
        private IActionResult ValidateYear(string year)
        {
            if (string.IsNullOrEmpty(year))
            {
                return BadRequest(new ApiRespons
                {
                    Success = false,
                    Message = "Vui lòng điền năm."
                });
            }

            if (!int.TryParse(year, out int yearInt) || yearInt < 1900 || yearInt > DateTime.Now.Year + 10)
            {
                return BadRequest(new ApiRespons
                {
                    Success = false,
                    Message = "Năm không hợp lệ."
                });
            }

            return null;
        }

        //1.Số lượng các kỳ bầu cử trong năm
        [HttpGet("count-elections-in-year")]
        public async Task<IActionResult> CountElectionsInYear([FromQuery]string year){
            var validationResult = ValidateYear(year);
            if (validationResult != null) return validationResult;

            return await ImplermentControllerLayer(
                () => _statisticsRepository._countElectionsInYear(year),
                "Lỗi khi lấy số lượng",
                "CountElectionsInYear"
            );
        }

        //2.Số lượng các kỳ bầu cử trong năm
        [HttpGet("number-of-voters-participating-in-elections-by-year")]
        public async Task<IActionResult> NumberOfVotersParticipatingInElectionsByYear([FromQuery]string year){
            var validationResult = ValidateYear(year);
            if (validationResult != null) return validationResult;

            return await ImplermentControllerLayer(
                () => _statisticsRepository._numberOfVotersParticipatingInElectionsByYear(year),
                "Lỗi khi lấy số lượng",
                "CountElectionsInYear"
            );
        }

        //3.Số lượng các kỳ bầu cử trong năm
        [HttpGet("number-of-candidates-participating-in-elections-by-year")]
        public async Task<IActionResult> NumberOfCandidatesParticipatingInElectionsByYear([FromQuery]string year){
            var validationResult = ValidateYear(year);
            if (validationResult != null) return validationResult;

            return await ImplermentControllerLayer(
                () => _statisticsRepository._numberOfCandidatesParticipatingInElectionsByYear(year),
                "Lỗi khi lấy số lượng",
                "CountElectionsInYear"
            );
        }

        //4. Số lượng cán bộ tham dự bầu cử trong năm
        [HttpGet("number-of-cadres-participating-in-elections-by-year")]
        public async Task<IActionResult> NumberOfCadresParticipatingInElectionsByYear([FromQuery]string year){
            var validationResult = ValidateYear(year);
            if (validationResult != null) return validationResult;

            return await ImplermentControllerLayer(
                () => _statisticsRepository._numberOfCadresParticipatingInElectionsByYear(year),
                "Lỗi khi lấy số lượng",
                "CountElectionsInYear"
            );
        }

        //5. Số lượng kỳ bầu cử được công bố trong năm
        [HttpGet("number-of-elections-with-announced-results-based-on-year")]
        public async Task<IActionResult> NumberOfElectionsWithAnnouncedResultsBasedOnYear([FromQuery]string year){
            var validationResult = ValidateYear(year);
            if (validationResult != null) return validationResult;

            return await ImplermentControllerLayer(
                () => _statisticsRepository._numberOfElectionsWithAnnouncedResultsBasedOnYear(year),
                "Lỗi khi lấy số lượng",
                "CountElectionsInYear"
            );
        }

        //6. Số lượng
        [HttpGet("count-locked-accounts")]
        public async Task<IActionResult> CountLockedAccounts(){
            return await ImplermentControllerLayer(
                () => _statisticsRepository._countLockedAccounts(),
                "Lỗi khi lấy số lượng",
                "counting locked accounts");
        }

        //7.Số lượng đơn vị bầu cử
        [HttpGet("number-of-constituencies")]
        public async Task<IActionResult> NumberOfConstituencies(){
            return await ImplermentControllerLayer(
                () => _statisticsRepository._NumberOfConstituencies(),
                "Lỗi khi lấy số lượng",
                "counting locked accounts");
        }

        //8. Số lượng danh mục ứng cử 
        [HttpGet("number-of-nominations")]
        public async Task<IActionResult> NumberOfNominations(){
            return await ImplermentControllerLayer(
                () => _statisticsRepository._NumberOfNominations(),
                "Lỗi khi lấy số lượng",
                "counting locked accounts");
        }

        //9. Số lượng chức vụ 
        [HttpGet("number-of-positions")]
        public async Task<IActionResult> NumberOfPositions(){
            return await ImplermentControllerLayer(
                () => _statisticsRepository._NumberOfPositions(),
                "Lỗi khi lấy số lượng ",
                "counting locked accounts");
        }

        //10.  Số lượng ban
        [HttpGet("number-of-boards")]
        public async Task<IActionResult> NumberOfBoards(){
             return await ImplermentControllerLayer(
                () => _statisticsRepository._NumberOfBoards(),
                "Lỗi khi lấy số lượng ",
                "counting locked accounts");
        }

    }
}