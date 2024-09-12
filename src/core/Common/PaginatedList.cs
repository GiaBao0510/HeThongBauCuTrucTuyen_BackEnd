using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.src.core.Common
{
    //Trả về các dòng theo từng trang
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex{set;get;}   //Đứng tại trang nào
        public int TotalPages {set;get;} //Tổng số lượng trang

        public PaginatedList(List<T> items, int count ,int pageIndex ,int pageSize){
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count/ (double)pageSize);        //Tính tổng số trang = cách lấy tổng phần tử/ kích thước trang và làm tròn lên 
            AddRange(items);                                                //Trả về list đổi tượng
        }

        public static PaginatedList<T> Create(IQueryable<T> source, int pageSize, int pageIndex){
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize) //Tính toán để lấy số lượng phần tử vừa đủ để hiển thị
                .Take(pageSize).ToList();

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

    }
}