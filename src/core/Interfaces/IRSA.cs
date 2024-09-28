using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.src.core.Interfaces
{
    public interface IRSA
    {
        public (string, string) GenrateKey();
        public byte[] Encrypt(string publicKey, string DataToEnCrypt);
        public string Decrypt(string privateKey, byte[] encryptBytes);
        public byte[] Sign(string privateKey, string dataToSign);
        public  bool verify(string publicKey, string dataToValidate, byte[] signature);
    }
}