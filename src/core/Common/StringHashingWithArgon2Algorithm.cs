using System;
using System.Text;
using Konscious.Security.Cryptography;
using System.Security.Cryptography;

namespace BackEnd.src.core.Common
{
    public class StringHashingWithArgon2Algorithm
    {
        //Thuộc tính tĩnh
        private static int NumberOfIterations = 4,
                        memorySize = 65536,
                        numberOfThread = 8;

        //Hàm tạo salt ngẫu nhiên
        public static byte[] GenerateRandomSalt(int length)
        {
            var salt = new byte[length];

            //Phương thức này dùng để tạo các số ngẫu nhiên và điền vào mảng salt
            RandomNumberGenerator.Fill(salt);   //Tạo salt ngẫu nhiên an toàn
            return salt;
        }

        //Băm chuỗi với Argon2id
        public static string StringEncoding(string str, byte[] salt)
        {
            var PassWordBytes = Encoding.UTF8.GetBytes(str);

            using (var argon2 = new Argon2id(PassWordBytes))
            {
                argon2.Salt = salt;             //Thêm giá trị ngẫu nhiên vào mật khẩu trước khi băm. Để ngăn chặn tắn công RainbowTable
                argon2.DegreeOfParallelism = numberOfThread;        //Xác định số lượng luồng cho CPU để tính toán đồng thời, nhiều luồng thì tăng tốc quá trình băm. Nhưng tốn chi phí 
                argon2.MemorySize = memorySize;                     //Xác định lượng bộ nhớ được sử dụng cho quá trình băm (64MB)
                argon2.Iterations = NumberOfIterations;             //Xác định số lần lặp lại trong quá trình băm, Càng nhiều vòng lặp càng tốn thời gian.Nhưng tăng độ bảo mật

                return Convert.ToBase64String(argon2.GetBytes(16));
            }//Tự động giải phóng đối tượng khi kết thúc
        }

        //So sánh mật khẩu
        public static bool VerifyPassword(string text, string hashedString, byte[] salt)
        {
            var newHashedPwd = StringEncoding(text, salt);
            return hashedString == newHashedPwd;
        }
    }
}