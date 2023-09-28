using System;
using System.Security.Cryptography;
using System.Text;

namespace TreeDocDb.Core;

public class TextEncryption : IDisposable
{
    private readonly Aes _aes;
    private readonly ICryptoTransform _enc;
    private readonly ICryptoTransform _dec; 
    
    public TextEncryption(string token, string iv)
    {
        var iv1 = Encoding.UTF8.GetBytes(iv);
        if (iv1.Length != 16)
            throw new ArgumentException("IV must be 16 bytes long", nameof(iv));

        _aes = Aes.Create();
        var legalKeySizes = _aes.LegalKeySizes;
        var legalBlockSizes = _aes.LegalBlockSizes;
          
        _aes.Key = ? // TODO: implement
        _aes.IV = iv1;

        _aes.Mode = CipherMode.CBC;
        _aes.Padding = PaddingMode.PKCS7;

        _enc = _aes.CreateEncryptor(_aes.Key, _aes.IV);
        _dec = _aes.CreateDecryptor(_aes.Key, _aes.IV);
    }

    public string Encrypt(string raw)
    {
        return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(raw)));
    }

    public byte[] Encrypt(byte[] raw)
    {
        return _enc.TransformFinalBlock(raw, 0, raw.Length);
    }

    public string Decrypt(string encrypted)
    {
        return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(encrypted)));
    }

    public byte[] Decrypt(byte[] encrypted)
    {
        return _dec.TransformFinalBlock(encrypted, 0, encrypted.Length);
    }

    public void Dispose()
    {
        _aes.Dispose();
        _enc.Dispose();
        _dec.Dispose();
    }
}