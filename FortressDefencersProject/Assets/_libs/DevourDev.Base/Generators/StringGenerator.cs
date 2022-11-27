using System.Runtime.CompilerServices;
using System.Text;

namespace DevourDev.Base.Generators
{
    public class StringGenerator
    {
        private readonly HeavyRandom.DevourRandom _rnd = new DevourDev.Base.HeavyRandom.DevourRandom();

        private Character[] _availableCharacters;

        private char[] _charMap;

        public StringGenerator(params Character[] availableCharacters)
        {
            SetCharacters(availableCharacters);
        }
        public void SetCharacters(params Character[] availableCharacters)
        {
            _availableCharacters = availableCharacters;
            GenerateCharMap();
        }

        public string GenerateString(int minLength, int maxLength)
        {
            int length = _rnd.RandInt(minLength, maxLength + 1);
            return GenerateString(length);
        }
        public string GenerateString(int length)
        {
            var sb = GenerateStringBuilder(length);

            return sb.ToString();
        }

        public StringBuilder GenerateStringBuilder(int minLength, int maxLength)
        {
            int length = _rnd.RandInt(minLength, maxLength + 1);
            return GenerateStringBuilder(length);
        }
        public StringBuilder GenerateStringBuilder(int length)
        {
            var sb = new StringBuilder(length);
            AppendRandomSymbolsToStringBuilder(sb, length);
            return sb;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendRandomSymbolsToStringBuilder(StringBuilder sb, int minLength, int maxLength)
        {
            int length = _rnd.RandInt(minLength, maxLength + 1);
            AppendRandomSymbolsToStringBuilder(sb, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendRandomSymbolsToStringBuilder(StringBuilder sb, int length)
        {
            for (int i = 0; i < length; i++)
            {
                sb.Append(GetRandomCharFromMap());
            }
        }

        private void GenerateCharMap()
        {
            int mapLength = 0;
            foreach (var c in _availableCharacters)
            {
                mapLength += c.Chance;
            }
            _charMap = new char[mapLength];
            int index = 0;
            foreach (var c in _availableCharacters)
            {
                for (int i = 0; i < c.Chance; i++)
                {
                    _charMap[index++] = c.Char;
                }
            }
        }
        private char GetRandomCharFromMap()
        {
            return _rnd.RandElement(_charMap);
        }

        public static string FastGenerateString(int length)
        {
            return FastGenerateString(length, Character.EnglishAndRussianAlphabetAndNumbers);
        }
        public static string FastGenerateString(int length, params Character[] characters)
        {
            StringGenerator generator = new(characters);
            return generator.GenerateString(length);
        }
        public static string FastGenerateString(int minLength, int maxLength)
        {
            StringGenerator generator = new(Character.EnglishAndRussianAlphabetAndNumbers);
            return generator.GenerateString(minLength, maxLength);
        }
        public static string FastGenerateString(int minLength, int maxLength, params Character[] characters)
        {
            StringGenerator generator = new(characters);
            return generator.GenerateString(minLength, maxLength);
        }
        public static string 
            FastGenerateEnglishString(int length)
        {
            return FastGenerateString(length, Character.EnglishAlphabet);
        }

    }


}
