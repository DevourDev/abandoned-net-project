using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DevourDev.Base.Security
{
    public static class Hasher
    {
        public static string ReplaceChar(string sourceString, char charToReplace, char charReplaceWith)
        {
            StringBuilder sb = new(sourceString.Length);

            foreach (var c in sourceString)
            {
                if (c == charToReplace)
                {
                    sb.Append(charReplaceWith);
                    continue;
                }

                sb.Append(c);
            }

            return sb.ToString();
        }
        public static string ReplaceChars(string sourceString, char[] charsToReplace, char charReplaceWith)
        {
            StringBuilder sb = new(sourceString.Length);

            foreach (var c in sourceString)
            {
                if (charsToReplace.Contains(c))
                {
                    sb.Append(charReplaceWith);
                    continue;
                }

                sb.Append(c);
            }

            return sb.ToString();
        }
        public static string RemoveChar(string sourceString, char charToRemove)
        {
            StringBuilder sb = new(sourceString.Length);

            foreach (var c in sourceString)
            {
                if (c == charToRemove)
                {
                    continue;
                }

                sb.Append(c);
            }

            return sb.ToString();
        }
        public static string RemoveChars(string sourceString, char[] charsToRemove)
        {
            StringBuilder sb = new(sourceString.Length);

            foreach (var c in sourceString)
            {
                if (charsToRemove.Contains(c))
                {
                    continue;
                }

                sb.Append(c);
            }

            return sb.ToString();
        }
        public static string HashString(string stringToHash, bool removeHyphen = true)
        {
            byte[] unhashedBytes = Encoding.UTF8.GetBytes(stringToHash);
            var sha512 = SHA512.Create();
            var hashedBytes = sha512.ComputeHash(unhashedBytes);
            string hashedString = BitConverter.ToString(hashedBytes);

            if (removeHyphen)
                hashedString = RemoveChar(hashedString, '-');

            return hashedString;
        }
    }
}
