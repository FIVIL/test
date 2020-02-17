using System;
using System.Security.Cryptography;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            var c = new CryptoServiceProvider();
            while (true)
            {
                c.Set();
                var s = Console.ReadLine();
                var (cipher, tag) = c.Encrypt(s);
                Console.WriteLine($"{Convert.ToBase64String(cipher)}-{Convert.ToBase64String(tag)}");
                if (s == c.Decrypt(cipher, tag))
                    Console.WriteLine(c.Decrypt(cipher, tag));
                else Console.WriteLine("fuck");
            }
        }
    }
    public class CryptoServiceProvider
    {
        public byte[] Key { get; private set; }
        public byte[] Nonce { get; private set; }
        private readonly int keySize = 32;
        private readonly int nonceSize = 12;
        private readonly int tagSize = 16;

        public void Set()
        {

            using var rng = new RNGCryptoServiceProvider();
            Key = new byte[keySize];
            Nonce = new byte[nonceSize];
            rng.GetBytes(Key);
            rng.GetBytes(Nonce);
        }
        public void Reset(byte[] key, byte[] nonce)
        {
            if (nonce.Length == nonceSize) Nonce = nonce;
            if (key.Length == keySize) Key = key;
        }
        public (byte[] cipher, byte[] tag) Encrypt(byte[] plainText)
        {
            byte[] tag = new byte[tagSize];
            byte[] cipher = new byte[plainText.Length];
            using var aesGcm = new AesGcm(Key);
            aesGcm.Encrypt(Nonce, plainText, cipher, tag);
            return (cipher, tag);
        }
        public byte[] DecryptByte(byte[] cipher, byte[] tag)
        {
            if (tag.Length != tagSize) throw new Exception("tag length not suported");
            byte[] plain = new byte[cipher.Length];
            using var aesGcm = new AesGcm(Key);
            aesGcm.Decrypt(Nonce, cipher, tag, plain);
            return plain;
        }

        public (byte[] cipher, byte[] tag) Encrypt(string plainText)
            => Encrypt(System.Text.Encoding.UTF8.GetBytes(plainText));

        public string Decrypt(byte[] cipher, byte[] tag)
            => System.Text.Encoding.UTF8.GetString(DecryptByte(cipher, tag));
    }
}
