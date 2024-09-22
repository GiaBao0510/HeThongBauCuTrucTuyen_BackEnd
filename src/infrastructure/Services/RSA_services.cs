using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.src.core.Interfaces;

namespace BackEnd.src.infrastructure.Services
{
    public class RSA_services:IRSA
    {
        //1. Tính ước chung lớn nhất
        public int gcd(int a, int b){
            if(a> b){
                int r = a%b;
                if(r == 1){
                    return 1;
                }else if(r == 0){
                    return 0;
                }else if(r>0){
                    return gcd(b,r);
                }
            }
            return 0;
        }
        public int Euclide_extension(int a, int b){
            int tempQ = a;
            
            //Khởi tạo biến
            int x0 = 0, x1 = 1, y0= 1, y1= 0;
            int X,Y;
            while (true){
                int r = a%b;
                
                if(r == 0)  //Đieu kiện dừng
                    break;
                int q = (int)(a/b);
                X = x1 - q*x0;
                Y = y1 - q*y0;

                //Nếu chưa dừng thì tiếp tục gán và thay đổi giá trị
                x0 = X;
                y0 = Y;
                x1 = x0;
                y1 = y0;
                a = b;
                b = r;
            }
            
            return tempQ+Y;
        }

        //Kiểm tra số nguyên tố
        public bool PrimeNumberTest(int n){
            if(n < 2){
                return false;
            }

            int j = n -1;
            for(int i = 2; i<= Math.Sqrt(n); i++){
                if(n%i == 0 || n%j == 0){
                    return false;
                }
            }
            return true;
        }

        //Lấy 2 số nguyên tố khác nhau từ tệp tin
        public List<int> TwoDifferentRandomPrimeNumbers(){
            
            //Tạo danh sách
            List<int> list = new List<int>();
            
            //Đường dẫn
            string filePath = "../Config/SoNguyenTo.txt";
            
            //Đọc file  
            string content = File.ReadAllText(filePath);

            //Tách các số nguyên tố
            int[] primeNumber = content.Split(',').Select(n => int.Parse(n.Trim())).ToArray();

            //Sử dung Random để lấy số nguyên tố
            Random random = new Random();
            int q_index = random.Next(0, primeNumber.Length - 1);
            int p_index;
            
            //Lấy số nguyên tố ngẫu nhiên khác q
            while(true){
                p_index = random.Next(0, primeNumber.Length - 1);
                if(p_index != q_index){
                    break;
                }
            }

            list.Add(primeNumber[p_index]);
            list.Add(primeNumber[q_index]);

            return list;
        }

        //Tạo khóa RSA
        public List<int> GenerateKey(){
            //Lấy số nguyên tố ngẫu nhiên Q<P
            List<int> list = TwoDifferentRandomPrimeNumbers();
            int q = list[0],
                p = list[1];

            //Tính n
            int n = q*p;

            //Tính phi
            int phi = (q-1)*(p-1);

            //Tạo danh sách số ngẫu nhiên từ 1 đên phi. Sau đó chọn e ngẫu nhiên
            List<int> ListE = new List<int>();
            for(int i = 1; i <= phi; i ++){
                if(gcd(phi,i) == 1)
                    ListE.Add(i);
            }

            //Lấy e ngẫu nhiên từ ListE
            Random random = new Random();
            int e = ListE[random.Next(0, ListE.Count -1)];

            //Tính D
            int D = Euclide_extension(phi, e);
            return new List<int>{e,D,n};
        }

        //Khóa bí mật
        public int RSA_PrivateKey(int E,int P, int N){
            int C = (int)(Math.Pow(P,E) % N);
            return C;
        }

        //Khóa công khai
        public int RSA_PublicKey(int D,int C, int N){
            int P = (int)(Math.Pow(C,D) % N);
            return P;
        }

        //Mã hóa RSA
        public string RSA_Encryption(string P, int E, int N){
            
        }

        //Giải mã RSA
        public string RSA_Decryption(string C, int D, int N);
    
    }
}