using log4net;
using System.Numerics;
using BackEnd.src.core.Interfaces;
using System.Security.Cryptography;

namespace BackEnd.src.infrastructure.Services
{
    public class PaillierServices : IPaillierServices
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program)); 

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
                result += BigInteger.Pow(b,s);
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
            _log.Info($"\tKiểm tra giá kết quả kiểm phiếu lớn nhất = {Tmax}");

            //Xác định độ dài bit tối đa của Tmax
            int tmaxBitLength = (int)Tmax.GetBitLength() + 1;

            // Đặt độ dài bit mục tiêu cho N lớn hơn Tmax đúng 1 bit
            int targetNBitLength = tmaxBitLength + 1;

            _log.Info($"\tĐộ dài bit của Tmax = {tmaxBitLength}");
            _log.Info($"\tĐộ dài bit mục tiêu cho N = {targetNBitLength}");

            //Lấy độ dài của Tmax chia đôi để q và p khi nhân cho nhau có độ dài lớn hơn Tmax khoảng 1 đơn vị
            int pqBitLength = targetNBitLength /2;
            
            if(targetNBitLength % 2 != 0)
                pqBitLength += 1;
            
            _log.Info($"\tĐộ dài bit cho p và q = {pqBitLength}");

            //Đặt giới hạn độ dài khóa

            //Sinh số nguyên tố ngẫu nhiên q và p
            BigInteger p = GenerateLargePrime(pqBitLength);
            BigInteger q;
            do{
                q = GenerateLargePrime(pqBitLength);
            }while(q == p);

            _log.Info($"\t>> Q = {q}");
            _log.Info($"\t>> P = {p}");

            //Điều kiện là q và p phải lớn hơn Tmax
            if(p * q < Tmax)
                // Tăng độ dài bit và thử lại
                return TinhModulo_N_WithBitLengthAdjustment(n, b, s, pqBitLength + 1);
            else
                return (q,p);   
        }

        // Hàm hỗ trợ cho TinhModulo_N khi cần điều chỉnh độ dài bit
        private (BigInteger, BigInteger) TinhModulo_N_WithBitLengthAdjustment(int n, int b, int s, int pqBitLength)
        {
            BigInteger Tmax = KetQuaKiemPhieuLonNhat_Tmax(n, b, s);

            _log.Info($"\tĐiều chỉnh độ dài bit cho p và q = {pqBitLength}");

            // Sinh số nguyên tố ngẫu nhiên q và p
            BigInteger p = GenerateLargePrime(pqBitLength);
            BigInteger q;
            do
            {
                q = GenerateLargePrime(pqBitLength);
            } while (q == p);

            BigInteger N = p * q;

            // Kiểm tra nếu N đủ lớn hơn Tmax
            if (N > Tmax)
            {
                _log.Info($"\t>> N = {N}");
                _log.Info($"\tĐộ dài bit của N = {N.GetBitLength()}");
                return (q, p);
            }
            else
            {
                _log.Info("\tN vẫn không đủ lớn. Tăng độ dài bit của p và q thêm 1 và thử lại.");
                // Tăng độ dài bit và thử lại
                return TinhModulo_N_WithBitLengthAdjustment(n, b, s, pqBitLength + 1);
            }
        }

        //8. tạo khóa công khai - và khóa cá nhân
        public (BigInteger, BigInteger, BigInteger, BigInteger) GenerateKey_public_private( int n, int b, int s){
            _log.Info("Chuẩn bị tạo khóa công khai và khóa cá nhân");
            //1.Tính N
            var random = new Random();
            (BigInteger q, BigInteger p) = TinhModulo_N( n, b, s);
            _log.Info($"\t>> Q = {q}");
            _log.Info($"\t>> P = {p}");
            BigInteger N = p*q;
            _log.Info($"\t>> N = {N}");

            //2.Tính số camichael - lamda
            BigInteger lamda = lcm(q-1,p-1);
            _log.Info($"\t>> lamda = {lamda}");
            BigInteger Nx2 = N*N;
            _log.Info($"\t>> Nx2 = {Nx2}");
            BigInteger G = 0;

            //3. Chọn bãn ngẫu nhiên Semi-random - Chọn g 
            while(true){
                BigInteger g = GetRanDomBigInteger_bytes(0,Nx2) ;
                _log.Info($"\t>> Chọn đại g = {g}");
                
                //Nếu g là nguyên tố cùng nhau với n^2 thì làm bước sau đây
                if(TinhNguyenToCungNhau( g, Nx2) == true){
                    _log.Info($"\t>> Nguyên tố cùng nhau với N ...");
                    //3.1 tính u
                    BigInteger u = ModuloPower(g, lamda, Nx2);

                    //3.2 tính L(u)
                    BigInteger Lu = BigInteger.Divide(u-1,N);

                    //3.3 Kiểm tra nguyên tố cùng nhau
                    if(gcd( Lu, N) == 1){
                        G = g;
                        _log.Info($"\t>> G = {G}");
                        break;      //Dừng
                    } 
                }
            }

            //4.Tính muy            
                // -- 4.1 Tìm u = g^lamda % N^2
            BigInteger U = ModuloPower(G, lamda, Nx2);
            _log.Info($"\t>> U = {U}");
                
                // -- 4.2  TÍnh L(U) = (u-1)//n
            BigInteger LU = BigInteger.Divide(U-1,N);
            _log.Info($"\t>> LU = {LU}");

                // -- 4.2 Tính Muy = L(u)^-1 % N
            BigInteger muy = Euclide_MoRong( N, LU);
            _log.Info($"\t>> muy = {muy}");

            _log.Info("Tạo khóa công khai và khóa cá nhân thành công");
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

        //12. Tạo hàm Random cho kiểu BigInteger với Bytes
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

        //13. Mã hóa
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

        //14. Giải mã
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

        //12. Hàm tính luy thừa trực tiệp để giảm nguy cơ lỗi khi dùng Math.pow (Sử dụng thuật toán Right-to-left binary)
        public BigInteger ModuloPower(BigInteger baseNum, BigInteger exponent, BigInteger modulus)
        {
            if (modulus == 1) return 0;     //Nếu modulus là 1 thì trả về 0

            BigInteger result = 1;          //Bắt đầu kết quả từ 1
            baseNum = baseNum % modulus;    //Giảm xuống bằng modulus để tránh số quá lớn 

            //Vòng lặp chính - thuật toán bình phương nhân
            //Sử dụng biễu diễn nhị phân của số mũ để giảm số lần lặp
            while (exponent > 0)
            {
                //Nếu bit là 1, nhân kết quả với baseNum
                if((exponent & 1) == 1)
                    result = (result * baseNum) % modulus;
                
                exponent >>= 1;                             //Dịch bit phải
                baseNum = (baseNum * baseNum) % modulus;    //Bình phương baseNum
            }

            return result;
        }

        //16. Hàm sinh số nguyên tố
        public BigInteger GenerateLargePrime(int bitLength){
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[bitLength/ 8 + 1];
            BigInteger result;

            do{
                rng.GetBytes(bytes);
                bytes[bytes.Length - 1] &= (byte)0x7F; // Đảm bảo số dương
                result = new BigInteger(bytes);
            }while(!IsProbablyPrime(result, 20));

            return result;
        }

        //17. Hàm kiểm tra số nguyên tố sử dụng Miler-Rabin
        public bool IsProbablyPrime(BigInteger value, int witnesses){
            // Loại bỏ các trường hợp đặc biệt nhỏ hơn hoặc bằng 3
            if (value <= 1) return false; // Số <= 1 không phải nguyên tố
            if (value <= 3) return true;  // 2 và 3 là số nguyên tố

            // Phân tích số thành dạng: value - 1 = 2^s * d
            int s = 0;
            BigInteger d = value - 1;

            // Tìm số mũ của 2 trong việc phân tích value - 1
            while (d % 2 == 0)
            {
                d /= 2;  // Chia liên tục cho 2
                s += 1;  // Đếm số lần chia
            }

            // Tạo trình sinh số ngẫu nhiên an toàn
            RandomNumberGenerator rng = RandomNumberGenerator.Create();

            // Tạo mảng byte có độ dài bằng độ dài value
            byte[] bytes = new byte[value.GetByteCount()];

            // Kiểm tra với số lần thử (witnesses) được chỉ định
            for (int i = 0; i < witnesses; i++)
            {
                // Sinh số ngẫu nhiên a trong khoảng [2, value-2]
                BigInteger a;
                do
                {
                    rng.GetBytes(bytes);
                    a = new BigInteger(bytes);
                } while (a < 2 || a >= value - 2);

                // Tính x = a^d mod value
                BigInteger x = BigInteger.ModPow(a, d, value);
                
                // Nếu x là 1 hoặc value-1 thì bỏ qua và tiếp tục
                if (x == 1 || x == value - 1)
                    continue;

                // Cờ để kiểm tra có tiếp tục vòng lặp chứng minh không
                bool continueWitnessLoop = false;

                // Kiểm tra các lũy thừa bình phương
                for (int r = 1; r < s; r++)
                {
                    // Bình phương x và lấy modulo
                    x = BigInteger.ModPow(x, 2, value);

                    // Nếu x = 1 thì chắc chắn không phải số nguyên tố
                    if (x == 1)
                        return false;

                    // Nếu x = value-1 thì có thể là số nguyên tố
                    if (x == value - 1)
                    {
                        continueWitnessLoop = true;
                        break;
                    }
                }

                // Nếu thoát vòng lặp do tìm thấy value-1 thì tiếp tục kiểm tra
                if (continueWitnessLoop)
                    continue;

                // Nếu không tìm thấy điều kiện nguyên tố thì kết luận không phải
                return false;
            }

            // Sau khi kiểm tra hết các lần thử, xác suất là số nguyên tố rất cao
            return true;
        }
    }
}