using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.src.core.Common
{
    
    public class RandomString
    {
        private static readonly Random random = new Random();
        private static readonly char[] digits = "0123456789".ToCharArray();

        //Tạo chuỗi ngẫu nhiên

        public static string DaySoNgauNhien(int length)
        {
            var buffer = new char[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = digits[random.Next(digits.Length)];
            }
            return new string(buffer);
        }

        public static string ChuoiNgauNhien(int lenght){
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(
                Enumerable.Repeat(chars, lenght).
                Select(s=>s[random.Next(s.Length)]).ToArray()
            );
        }

        //Tạo ID cho các lớp cử tri, cán bộ, ứng cử viên
        public static string CreateID(){
            DateTime current = DateTime.Now;
            string CurrentTime = current.ToString("yyMMddHHmmssff");
            return CurrentTime;
        }

        //Tạo ID cho người dùng
        public static string CreateID_User(){
            DateTime current = DateTime.Now;
            string CurrentTime = ChuoiNgauNhien(2)+current.ToString("yyyyMMddHHmmss");
            return CurrentTime;
        }
    }
}