using System.Security.Cryptography;
using System.Text;
using BackEnd.src.core.Interfaces;

namespace BackEnd.src.infrastructure.Services
{
    public class RSA_services:IRSA
    {
        //Tạo khóa private key và public key
        public (string, string) GenrateKey()
        {
            using (RSA rsa = RSA.Create()){
                string publicKey = rsa.ToXmlString(false);
                string privateKey = rsa.ToXmlString(true);

                return (publicKey, privateKey);
            }
        }

        //Mã hóa
        public byte[] Encrypt(string publicKey, string DataToEnCrypt){
            using(RSA rsa = RSA.Create()){
                rsa.FromXmlString(publicKey);
                byte[] mesageBytes = Encoding.UTF8.GetBytes(DataToEnCrypt);
                byte[] encryptdBytes = rsa.Encrypt(mesageBytes, RSAEncryptionPadding.Pkcs1);
                return encryptdBytes;
            }
        }

        //Giải mã 
        public string Decrypt(string privateKey, byte[] encryptBytes){
            using(RSA rsa = RSA.Create()){
                rsa.FromXmlString(privateKey);
                byte[] decryptBytes = rsa.Decrypt(encryptBytes, RSAEncryptionPadding.Pkcs1);
                string decryptMessage = Encoding.UTF8.GetString(decryptBytes);
                return decryptMessage;
            }
        }

        //Ký
        public byte[] Sign(string privateKey, string dataToSign){
            using (RSA rsa = RSA.Create()){
                rsa.FromXmlString(privateKey);
                byte[] messageBytes = Encoding.UTF8.GetBytes(dataToSign);
                byte[] signatureBytes = rsa.SignData(messageBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return signatureBytes;
            }
        }

        //Xác thực
        public bool verify(string publicKey, string dataToValidate, byte[] signature){
            using(RSACryptoServiceProvider rsa = new RSACryptoServiceProvider()){
                rsa.FromXmlString(publicKey);
                byte[] messageBytes = Encoding.UTF8.GetBytes(dataToValidate);
                return rsa.VerifyData(messageBytes, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }
    
    }
}