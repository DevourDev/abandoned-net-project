namespace DevourDev.Base.SystemExtentions
{
    public static class StringExtentions
    {
        public static bool ContainsAny(this string v, params char[] symbols)
        {
            foreach (var s in symbols)
            {
                if (v.Contains(s))
                {
                    return true;
                }
            }

            return false;
        }
        public static bool ContainsAny(this string v, params string[] strings)
        {
            foreach (var s in strings)
            {
                if (v.Contains(s))
                {
                    return true;
                }
            }

            return false;
        }
        public static bool ContainsAll(this string v, params char[] symbols)
        {
            foreach (var s in symbols)
            {
                if (!v.Contains(s))
                {
                    return false;
                }
            }

            return true;
        }
        public static bool ContainsAll(this string v, params string[] strings)
        {
            foreach (var s in strings)
            {
                if (!v.Contains(s))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
