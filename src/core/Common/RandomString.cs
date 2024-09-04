using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.src.core.Common
{
    
    public class RandomString
    {
        private static Random random = new Random();
        public static string ChuoiNgauNhien(int lenght){
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(
                Enumerable.Repeat(chars, lenght).
                Select(s=>s[random.Next(s.Length)]).ToArray()
            );
        }
    }
}