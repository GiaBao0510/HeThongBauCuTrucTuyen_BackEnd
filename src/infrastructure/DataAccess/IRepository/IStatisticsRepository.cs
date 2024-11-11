
namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IStatisticsRepository
    {
        //1.Số lượng các kỳ bầu cử trong năm
        Task<int> _countElectionsInYear(string year);
        //2.Số lượng cử tri tham gia bầu cử trong năm
        Task<int> _numberOfVotersParticipatingInElectionsByYear(string year);
        //3. Số lượng ứng cử viên đăng ký ghi danh kỳ bầu cử trong năm
        Task<int> _numberOfCandidatesParticipatingInElectionsByYear(string year);
        //4. Số lượng cán bộ tham dự bầu cử trong năm
        Task<int> _numberOfCadresParticipatingInElectionsByYear(string year);
        //5. Số lượng kỳ bầu cử được công bố trong năm
        Task<int> _numberOfElectionsWithAnnouncedResultsBasedOnYear(string year);
        //6.Số lượng tài khoản bị khóa
        Task<int> _countLockedAccounts();
        //7.Số lượng đơn vị bầu cử
        Task<int> _NumberOfConstituencies();
        //8. Số lượng danh mục ứng cử 
        Task<int> _NumberOfNominations();
        //9. Số lượng chức vụ
        Task<int> _NumberOfPositions();
        //10. Số lượng ban
        Task<int> _NumberOfBoards();
        
    }
}