using System.Security.Cryptography;
using System.Text;

namespace WalletManagement2.EncryptDecrypt;

public class EncryptionDecryption
{
    //encrypt
    private const int keySize = 10;
    private const int iterations = 350000;
    //hashing512 is fast but less secure as compared to hashing256 which is slow but more secure
    private HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
    public string HashPasword(string password, out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(keySize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations,
            hashAlgorithm,
            keySize);
        return Convert.ToHexString(hash);
    }
    //decrypt
    public bool VerifyPassword(string password, string hash, byte[] salt)
    {
        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);

        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
    }
}
