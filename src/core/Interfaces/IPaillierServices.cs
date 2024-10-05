using System.Numerics;

namespace BackEnd.src.core.Interfaces
{
    public interface IPaillierServices
    {
        int GiaTriToiDaCuaPhieuBau_M( int b,  int s);
        BigInteger gcd( BigInteger a, BigInteger b);
        BigInteger UCLN( BigInteger a, BigInteger b);
        BigInteger BCNN( BigInteger a,  BigInteger b);
        bool TinhNguyenToCungNhau( BigInteger a,  BigInteger b);
        BigInteger KetQuaKiemPhieuLonNhat_Tmax( int n,  int b,  int s);
        (BigInteger, BigInteger) TinhModulo_N( int n, int b, int s);
        (BigInteger, BigInteger, BigInteger) TinhCoSoNEn_g( int n, int b, int s);
        BigInteger Euclide_MoRong( BigInteger a, BigInteger b);
        BigInteger xgcd(BigInteger a, BigInteger m);
        BigInteger Carmichanel(BigInteger q, BigInteger p);
        BigInteger GetRanDomBigInteger_bytes(BigInteger minvalue, BigInteger maxvalue);
        (BigInteger, BigInteger, BigInteger, BigInteger) GenerateKey(int n,int b, int s);
        BigInteger Encryption(BigInteger g, BigInteger N, BigInteger mi);
        BigInteger Decryption(BigInteger c, BigInteger N ,BigInteger lamda, BigInteger muy);

    }
}