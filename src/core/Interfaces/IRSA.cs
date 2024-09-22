using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.src.core.Interfaces
{
    public interface IRSA
    {
        public int gcd(int a, int b);
        public int Euclide_extension(int a, int b);
        public bool PrimeNumberTest(int n);
        public List<int> TwoDifferentRandomPrimeNumbers();
        public List<int> GenerateKey();
        public int RSA_PrivateKey(int E,int P, int N);
        public int RSA_PublicKey(int D,int C, int N);
        public string RSA_Encryption(string P, int E, int N);
        public string RSA_Decryption(string C, int D, int N);
    }
}