
using BackEnd.src.infrastructure.DataAccess.Context;

namespace BackEnd.src.infrastructure.Services
{
    public class VerifyBallotServices: IDisposable
    {
        private readonly DatabaseContext _context;

        //Khởi tạo
        public VerifyBallotServices(DatabaseContext context){
            _context = context;
        }
        //Hủy
        public void Dispose() => _context.Dispose();
        
        //Liên kết: lớp thông báo,

        //1. Cử tri yêu câu xác nhận phiếu bầu trên kỳ bầu cử cụ thể
        //2. Cán bộ duyệt xác nhận và gửi về biên nhận mở
        //3. Cử tri chứng thực phiếu bầu và kỳ mù lên biên nhận mở
    }
}