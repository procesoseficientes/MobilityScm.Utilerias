using System;
using System.Security.Cryptography;
using System.Text;

namespace MobilityScm.Utilerias
{
    public static class Encrypt
    {
        public static string KeyEncryptAndDecrypt = "M0b1l1tySCM-RD";

        public static string EncryptText(string text)
        {
            var fixToEncrypt = Encoding.UTF8.GetBytes(text);

            var hashed5 = new MD5CryptoServiceProvider();

            var keyArray = hashed5.ComputeHash(Encoding.UTF8.GetBytes(KeyEncryptAndDecrypt));

            hashed5.Clear();

            var teds = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };


            var cTransform = teds.CreateEncryptor();

            var resultArrangement = cTransform.TransformFinalBlock(fixToEncrypt, 0, fixToEncrypt.Length);

            teds.Clear();

            text = Convert.ToBase64String(resultArrangement, 0, resultArrangement.Length);

            return text;
        }

        public static string DecryptText(string text)
        {
            var fixToDecrypt = Convert.FromBase64String(text);

            var hashed5 = new MD5CryptoServiceProvider();

            var keyArray = hashed5.ComputeHash(Encoding.UTF8.GetBytes(KeyEncryptAndDecrypt));

            hashed5.Clear();

            var teds = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = teds.CreateDecryptor();

            var resultArray = cTransform.TransformFinalBlock(fixToDecrypt, 0, fixToDecrypt.Length);

            teds.Clear();
            text = Encoding.UTF8.GetString(resultArray);

            return text;
        }
    }
}
