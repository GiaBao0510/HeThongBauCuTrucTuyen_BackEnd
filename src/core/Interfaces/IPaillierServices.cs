using System.Numerics;

namespace BackEnd.src.core.Interfaces
{
    public interface IPaillierServices
    {
        BigInteger gcd( BigInteger a, BigInteger b);
        BigInteger GiaTriToiDaCuaPhieuBau_M( int b,  int s);
        bool TinhNguyenToCungNhau( BigInteger a,  BigInteger b);
        BigInteger KetQuaKiemPhieuLonNhat_Tmax( int n,  int b,  int s);
        (BigInteger, BigInteger) TinhModulo_N( int n, int b, int s);
        (BigInteger, BigInteger, BigInteger, BigInteger) GenerateKey_public_private( int n, int b, int s);
        BigInteger Euclide_MoRong( BigInteger a, BigInteger b);
        BigInteger GetRanDomBigInteger_bytes(BigInteger minvalue, BigInteger maxvalue);
        BigInteger Encryption(BigInteger g, BigInteger N, BigInteger mi);
        BigInteger Decryption(BigInteger c, BigInteger N ,BigInteger lamda, BigInteger muy);

    }
}