

namespace BackEnd.src.core.Interfaces
{
    public interface IPaillier
    {
        //Tính gcd()
        public int gcd(int a, int b);
        //Tính ước chung lớn nhất
        public int UCLN(int a,int b);
        //Tính bội chung nhỏ nhất
        public int BCNN(int a, int b);
        //Tính nguyên tố cùng nhau
        public int TinhNguyenToCungNhau(int a, int b);
        //Tìm cơ số nền G
        //Tìm B
        //Tìm modulo N
    }
}