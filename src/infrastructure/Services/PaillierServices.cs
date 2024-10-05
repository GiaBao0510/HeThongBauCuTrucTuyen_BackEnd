using System;
using System.Numerics;
using System.Security.Cryptography;
using BackEnd.src.core.Interfaces;
namespace BackEnd.src.infrastructure.Services
{
    public class PaillierServices : IPaillierServices
    {
        //1.Tính gcd()
        public BigInteger gcd( BigInteger a, BigInteger b){
            if(a>b){
                BigInteger r = a%b;
                if(r == 1){
                    return 1;
                }else if(r == 0){
                    return 0;
                }else if(r > 0){
                    return gcd( r,  b);
                }
            }
            return 0;
        }
        
        //2.Tính ước chung lớn nhất
        public BigInteger UCLN( BigInteger a, BigInteger b){
            //Phải khác 0
            if(a == 0 || b == 0) return -1;

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
        public BigInteger BCNN( BigInteger a,  BigInteger b){
            //Phải khác 0
            if(a == 0 || b == 0) return -1;

            BigInteger aCopy = a, bCopy =b;
            return a*b/UCLN( aCopy, bCopy );
        }

        //4.Tính nguyên tố cùng nhau
        public bool TinhNguyenToCungNhau( BigInteger a,  BigInteger b){
            if(UCLN( a, b) == 1)
                return true;
            return false;
        }

        //5. Tính giá trị tối đa của phiếu bầu
        public int GiaTriToiDaCuaPhieuBau_M( int b,  int s){
            int result = 0;
            
            while(s >0){
                result += (int)Math.Pow(b,(int)s);
                s--;
            }
            return result;
        }

        //6.Tính kết quả kiểm phiếu lớn nhất
        public BigInteger KetQuaKiemPhieuLonNhat_Tmax( int n,  int b,  int s){
            return GiaTriToiDaCuaPhieuBau_M( b, s) * n;
        }

        //7. Tính Modulo N
        public (BigInteger, BigInteger) TinhModulo_N( int n, int b, int s){
            BigInteger Tmax = KetQuaKiemPhieuLonNhat_Tmax( n, b, s);

            //Đường dẫn 
            string filePath = "../Config/SoNguyenTo.txt";

            //Đọc nội dung tệp tin
            string context = File.ReadAllText(filePath);

            //Tách các số nguyên tó dựa ra khỏi chuỗi dựa trên dấu ","
            BigInteger[] primeNumber = context.Split(',').Select(n => BigInteger.Parse(n.Trim())).ToArray();

            //Random Q,P
            var random = new Random();
            BigInteger q = primeNumber[random.Next(0, primeNumber.Length -1)];
            BigInteger p = 0;
            while(true){
                p = primeNumber[random.Next(0, primeNumber.Length -1)];
                if(p != q && (p*q) > Tmax)
                    break;
            }
            return (q,p);
        }

        //8. Tính cơ số nền g
        public (BigInteger, BigInteger, BigInteger) TinhCoSoNEn_g( int n, int b, int s){
            //1.Tính N
            var random = new Random();
            (BigInteger q, BigInteger p) = TinhModulo_N( n, b, s);
            BigInteger N = p*q;
            q-=1;
            p-=1;
            //2.Tính số camichael
            BigInteger lamda = Carmichanel(q,p);
            BigInteger Nx2 = N*N;
            Console.WriteLine($">> N = {N}, N^2 = {Nx2}");

            //3. Chọn bãn ngẫu nhiên Semi-random
            while(true){
                BigInteger g = GetRanDomBigInteger_bytes(0,Nx2) ;
                
                //Nếu g là nguyên tố cùng nhau với n^2 thì làm bước sau đây
                if(TinhNguyenToCungNhau( g, Nx2) == true){
                    //3.1 tính u
                    BigInteger u = ModuloPower(g, lamda, Nx2);

                    //3.2 tính L(u)
                    BigInteger Lu = (BigInteger)((u-1)/N);

                    //3.3 Kiểm tra nguyên tố cùng nhau
                    if(UCLN( Lu, N) == 1){
                        return (g,N, lamda);
                    }
                }
            }
        }

        //9.Hàm Euclide mở rộng
        public BigInteger Euclide_MoRong( BigInteger a, BigInteger b){
            BigInteger temp = a;
            BigInteger x0 = 0,    x1 = 1,
                        y0 = 1,    y1 = 0,
                        X = 0,      Y =0;
            
            while(true){
                BigInteger r = a%b;
                if(r == 0) break; //Điều kiện dừng
                BigInteger q = a/b;
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

        //10.
        public BigInteger xgcd(BigInteger a, BigInteger m){
            BigInteger temp = m;
            BigInteger x0 = 0,    x1 = 1,
                y0 = 1,    y1 = 0;
            while(m != 0){
                BigInteger q = (BigInteger)(a/m);
                a = m;
                m = a%m;
                x0 = x1;
                x1 = x0 - q * x1;
                y0 = y1;
                y1 = y0-q*y1;
            }
            if(x0 < 0) x0 = temp + x0;
            return x0;
        }

        //11. Hàm Carmichanel
        public BigInteger Carmichanel(BigInteger q, BigInteger p){
            return ((p-1)*(q-1) )/UCLN(p-1,q-1);
        }

        //12. Hàm tính luy thừa trực tiệp để giảm nguy cơ lỗi khi dùng Math.pow
        public BigInteger ModuloPower(BigInteger g, BigInteger lamda, BigInteger mod)
        {
            if (lamda.Equals(0)) return 1;

            BigInteger result = 1;
            BigInteger baseNum = g % mod;

            while (lamda > 0)
            {
                if (lamda % 2 == 1)
                {
                    result = (result * baseNum) % mod;
                }
                baseNum = (baseNum * baseNum) % mod;
                lamda /= 2;
            }

            return result;
        }

        //13. Tạo hàm Random cho kiểu BigInteger với Bytes
        public BigInteger GetRanDomBigInteger_bytes(BigInteger minvalue, BigInteger maxvalue){
            Random random = new Random();
            byte[] bytes =  maxvalue.ToByteArray();
            BigInteger result;

            do{
                random.NextBytes(bytes);
                result = new BigInteger(bytes);
            }while(result < minvalue || result >= maxvalue);
            return result;
        }

        //14.Tạo khóa (public[N,g] và private[lamda,muy])
        public (BigInteger, BigInteger, BigInteger, BigInteger) GenerateKey(int n,int b, int s){
            
            (BigInteger g, BigInteger N, BigInteger lamda) = TinhCoSoNEn_g(n,b,s);
            
            //Trước khi tìm muy thì tính U và L(U)
            BigInteger U = ModuloPower(g, lamda, N*N);
            BigInteger h = (U-1) / N;

            BigInteger muy = Euclide_MoRong(N, h);
            return(N,g,lamda,muy);
        }

        public BigInteger Encryption(BigInteger g, BigInteger N, BigInteger mi){
            
            //Chọn ri ngẫu nhiên và nguyên tố cùng nhau với N
            BigInteger ri = 0;
            while (true){
                ri = GetRanDomBigInteger_bytes(1, N);
                if(TinhNguyenToCungNhau(ri,N))
                    break;
            }

            //Mã hóa 
            BigInteger c = BigInteger.Pow(g, (int)mi) * BigInteger.Pow(ri, (int)N) % (N * N);
            return c;
        }

        //15. Giải mã
        public BigInteger Decryption(BigInteger c, BigInteger N ,BigInteger lamda, BigInteger muy){
            BigInteger u = ModuloPower(c,lamda, N*N);
            BigInteger Lu = (u-1)/N;
            BigInteger D = Lu*muy % N;
            return D;
        }
    }
}