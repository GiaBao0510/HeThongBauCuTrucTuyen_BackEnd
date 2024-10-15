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
            while (b != 0)
            {
                BigInteger temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        //2. Tính bộ chung nhỏ nhất
        public BigInteger lcm(BigInteger a, BigInteger b)
        {
            return BigInteger.Divide(BigInteger.Multiply(a, b), gcd(a, b));
        }

        //4.Tính nguyên tố cùng nhau
        public bool TinhNguyenToCungNhau( BigInteger a,  BigInteger b){
            if(gcd( a, b) == 1)
                return true;
            return false;
        }

        //5. Tính giá trị tối đa của phiếu bầu
        public BigInteger GiaTriToiDaCuaPhieuBau_M( int b,  int s){
            BigInteger result = 0;
            
            while(s >0){
                result += (BigInteger)Math.Pow(b,(int)s);
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
            string filePath = "src/infrastructure/Config/SoNguyenTo.txt";

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

        //8. tạo khóa công khai - và khóa cá nhân
        public (BigInteger, BigInteger, BigInteger, BigInteger) GenerateKey_public_private( int n, int b, int s){
            //1.Tính N
            var random = new Random();
            (BigInteger q, BigInteger p) = TinhModulo_N( n, b, s);
            BigInteger N = p*q;

            //2.Tính số camichael - lamda
            BigInteger lamda = lcm(q-1,p-1);
            BigInteger Nx2 = N*N;
            BigInteger G = 0;

            //3. Chọn bãn ngẫu nhiên Semi-random - Chọn g 
            while(true){
                BigInteger g = GetRanDomBigInteger_bytes(0,Nx2) ;
                
                //Nếu g là nguyên tố cùng nhau với n^2 thì làm bước sau đây
                if(TinhNguyenToCungNhau( g, Nx2) == true){
                    //3.1 tính u
                    BigInteger u = ModuloPower(g, lamda, Nx2);

                    //3.2 tính L(u)
                    BigInteger Lu = BigInteger.Divide(u-1,N);

                    //3.3 Kiểm tra nguyên tố cùng nhau
                    if(gcd( Lu, N) == 1){
                        G = g;
                        break;      //Dừng
                    }
                }
            }

            //4.Tính muy            
                // -- 4.1 Tìm u = g^lamda % N^2
            BigInteger U = ModuloPower(G, lamda, Nx2);
                
                // -- 4.2  TÍnh L(U) = (u-1)//n
            BigInteger LU = BigInteger.Divide(U-1,N);

                // -- 4.2 Tính Muy = L(u)^-1 % N
            BigInteger muy = Euclide_MoRong( N, LU);

            // Console.WriteLine($"\t>> N = {N}");
            // Console.WriteLine($"\t>> G = {G}");
            // Console.WriteLine($"\t>> lamda = {lamda}");
            // Console.WriteLine($"\t>> muy = {muy}");

            return (N,G,lamda,muy);
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
                BigInteger q = BigInteger.Divide(a,b);
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

        //14. Mã hóa
        public BigInteger Encryption(BigInteger g, BigInteger N, BigInteger mi){
            
            //Chọn ri ngẫu nhiên và nguyên tố cùng nhau với N
            BigInteger ri = 0;
            while (true){
                ri = GetRanDomBigInteger_bytes(1, N);
                if(TinhNguyenToCungNhau(ri,N))
                    break;
            }
            Console.WriteLine($"\t>> ri = {ri}");
            //Mã hóa C = g^mi * r^n mod n^2
            BigInteger Nx2 = N*N;
            BigInteger gMiMod = BigInteger.ModPow(g, mi, Nx2);
            BigInteger rNMod = BigInteger.ModPow(ri, N, Nx2);
            BigInteger c =  (gMiMod * rNMod) % Nx2;
            return c;
        }

        //15. Giải mã
        public BigInteger Decryption(BigInteger c, BigInteger N ,BigInteger lamda, BigInteger muy){
            BigInteger Nx2 = N*N;

            //Tính U = C^lamda mod N^2
            BigInteger U = BigInteger.ModPow(c, lamda, Nx2);

            // Kiểm tra để tránh lỗi chia không đúng
            if ((U - 1) % N != 0) {
                throw new ArgumentException("U - 1 is not divisible by N. The decryption may fail.");
            }

            //Tính Lu = (U-1)/N
            BigInteger Lu = BigInteger.Divide(U-1,N);

            //Tính D = (Lu * muy) mod N
            BigInteger D = (Lu * muy) % N;
            
            return D;
        }
    }
}