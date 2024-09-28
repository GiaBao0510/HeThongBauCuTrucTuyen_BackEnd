using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.src.infrastructure.Services
{
    public class PaillierServices
    {
        //1.Tính gcd()
        public int gcd(ref int a,ref int b){
            if(a>b){
                int r = a%b;
                if(r == 1){
                    return 1;
                }else if(r == 0){
                    return 0;
                }else if(r > 0){
                    return gcd(ref r,ref  b);
                }
            }
            return 0;
        }
        
        //2.Tính ước chung lớn nhất
        public int UCLN(ref int a,ref int b){
            while(a != b){
                if(a > b){
                    a -=b;
                }else{
                    b -=a;
                }
            }
            return a;
        }

        //3.Tính bội chung nhỏ nhất
        public int BCNN(ref int a,ref  int b){
            int aCopy =a, bCopy = b;
            return a*b/UCLN(ref aCopy,ref bCopy);
        }

        //4.Tính nguyên tố cùng nhau
        public bool TinhNguyenToCungNhau(ref int a,ref  int b){
            if(UCLN(ref a,ref b) == 1)
                return true;
            return false;
        }

        //5. Tính giá trị tối đa của phiếu bầu
        public int GiaTriToiDaCuaPhieuBau_M(ref int b,ref  int s){
            int result = 0;
            
            while(s >=0){
                result += (int)Math.Pow(b,s);
                s--;
            }
            return result;
        }

        //6.Tính kết quả kiểm phiếu lớn nhất
        public int KetQuaKiemPhieuLonNhat_Tmax(ref int n,ref  int b,ref  int s){
            return (int)(GiaTriToiDaCuaPhieuBau_M(ref b,ref s) * n);
        }

        //7. Tính Modulo N
        public (int, int) TinhModulo_N(ref int n,ref int b,ref  int s){
            int Tmax = KetQuaKiemPhieuLonNhat_Tmax(ref n,ref b,ref s);

            //Đường dẫn 
            string filePath = "../Config/SoNguyenTo.txt";

            //Đọc nội dung tệp tin
            string context = File.ReadAllText(filePath);

            //Tách các số nguyên tó dựa ra khỏi chuỗi dựa trên dấu ","
            int[] primeNumber = context.Split(',').Select(n => int.Parse(n.Trim())).ToArray();

            //Random Q,P
            var random = new Random();
            int q = primeNumber[random.Next(0, primeNumber.Length -1)];
            int p = 0;
            while(true){
                p = primeNumber[random.Next(0, primeNumber.Length -1)];
                if(p != q && (p*q) > Tmax)
                    break;
            }
            return (q,p);
        }

        //8. Tính cơ số nền g
        public int TinhCoSoNEn_g(ref int n,ref int b,ref int s){

            //1.Tính N
            var random = new Random();
            (int q, int p) = TinhModulo_N(ref n,ref b,ref s);
            int N = p*q;
            q-=1;
            p-=1;
            //2.Tính số camichael
            int phi = BCNN(ref p,ref q);
            int Nx2 = N*N;

            //3. Chọn bãn ngẫu nhiên Semi-random
            while(true){
                int g = random.Next(0, N*N);

                //Nếu g là nguyên tố cùng nhau với n^2 thì làm bước sau đây
                if(TinhNguyenToCungNhau(ref g,ref Nx2) == true){
                    //3.1 tính u
                    int u = (int)Math.Pow(g, phi) % Nx2;

                    //3.2 tính L(u)
                    int Lu = (int)((u-1)/N);

                    //3.3 Kiểm tra gcd
                    if(gcd(ref Lu,ref N) == 1){
                        return g;
                    }
                }
            }
        }

        //9.Hàm Euclide mở rộng
        public int Euclide_MoRong(ref int a,ref int b){
            int temp = a;
            int x0 = 0,    x1 = 1,
                y0 = 1,    y1 = 0,
                X = 0,      Y =0;
            
            while(true){
                int r = a%b;
                if(r == 0) break; //Điều kiện dừng
                int q = a/b;
                X = x1 - q*x0;
                Y = y1 - q*y0;

                //Hoán đổi giá trị
                a = b;
                b = r;
                x1 = x0;
                y1 = y0;
                x0 = X;
                y0 = Y;
            }

            return temp + Y;
        }

    }
}