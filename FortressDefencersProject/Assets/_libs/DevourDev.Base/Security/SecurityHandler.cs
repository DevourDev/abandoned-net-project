using System;
using System.Security.Cryptography;

namespace DevourDev.Base.Security
{
    public class SecurityHandler
    {
        public const int DEFAULT_SECURITY_LEVEL = 2048;

        private readonly int _securityLevel;

        private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();


        public SecurityHandler(int securityLevel = DEFAULT_SECURITY_LEVEL)
        {
            _securityLevel = securityLevel;
        }

        public byte[] GenerateKey(long encodingValue)
        {
            return GenerateKey(encodingValue, _securityLevel - 8);
        }
        public byte[] GenerateKey(long encodingValue, int startEncodingIndex)
        {
            byte[] k = new byte[_securityLevel];
            _rng.GetBytes(k);
            byte[] encodedValue = BitConverter.GetBytes(encodingValue);


            if (startEncodingIndex + encodedValue.Length > _securityLevel)
            {
                throw new IndexOutOfRangeException($"{nameof(startEncodingIndex)} + 8 > {nameof(_securityLevel)} ({_securityLevel})");
            }

            for (int i = startEncodingIndex, j = 0; i < startEncodingIndex + encodedValue.Length; i++, j++)
            {
                k[i] = encodedValue[j];
            }

            return k;
        }


        public static long GetEncodedValue(byte[] key)
            => GetEncodedValue(key, key.Length - 8);
        public static long GetEncodedValue(byte[] key, int startDecodingIndex)
        {
            return BitConverter.ToInt64(key, startDecodingIndex);
        }

    }


}
