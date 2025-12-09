using System.Security.Cryptography;
using System.Text;

namespace PadelApp.Services;

public static class AuthKeyGenerator
{
    // Use a constant encryption key for all operations
    private static readonly byte[] EncryptionKey = SHA256.HashData(Encoding.UTF8.GetBytes("PadelApp2025FixedKey"));

    public static string GenerateEncryptedKey(string pass)
    {
        using var aes = Aes.Create();
        aes.Key = EncryptionKey;
        aes.GenerateIV();

        using var ms = new MemoryStream();
        // Write the IV first
        ms.Write(aes.IV, 0, aes.IV.Length);

        // Encrypt the data
        using (var encryptor = aes.CreateEncryptor())
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs, Encoding.UTF8, leaveOpen: true))
        {
            sw.Write(pass);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public static string Decrypt(string authKey)
    {
        var fullCipher = Convert.FromBase64String(authKey);
        using var aes = Aes.Create();
        aes.Key = EncryptionKey;

        // Get the IV from the start of the cipher
        var iv = new byte[aes.BlockSize / 8];
        Array.Copy(fullCipher, 0, iv, 0, iv.Length);
        aes.IV = iv;

        // Get the ciphertext (everything after the IV)
        var cipherText = new byte[fullCipher.Length - iv.Length];
        Array.Copy(fullCipher, iv.Length, cipherText, 0, cipherText.Length);

        // Decrypt
        using var ms = new MemoryStream(cipherText);
        using var decryptor = aes.CreateDecryptor();
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs, Encoding.UTF8);
        
        return sr.ReadToEnd();
    }
}