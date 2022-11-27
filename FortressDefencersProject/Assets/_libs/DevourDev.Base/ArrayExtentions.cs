using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevourDev.Base.SystemExtentions
{
    public static class ArrayExtentions
    {
        public static bool ListEqual<T>(this IList<T> first, IList<T> second)
        {
            if (first == null || second == null)
            {
                throw new NullReferenceException();
                //return false;
            }

            int length = first.Count;

            if (length != second.Count)
                return false;

            for (int i = 0; i < length; i++)
            {
                if (!first[i].Equals(second[i]))
                    return false;
            }

            return true;
        }
        public static bool ArrayEqual<T>(this T[] first, T[] second)
        {
            if (first == null || second == null)
            {
                throw new NullReferenceException();
                //return false;
            }

            int length = first.Length;

            if (length != second.Length)
                return false;

            for (int i = 0; i < length; i++)
            {
                if (!first[i].Equals(second[i]))
                    return false;
            }

            return true;
        }
        /// <summary>
        /// Возвращает индекс искомого элемента, 
        /// если он существует в коллекции.
        /// В противном случае - индекс ближайшего
        /// МЕНЬШЕГО элемента.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr">SORTED array</param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static SoftBinarySearchResult SoftBinarySearch<T>(this List<T> list, T item) where T : IComparable<T>
        {
            return list.ToArray().SoftBinarySearch(item);
        }
        /// <summary>
        /// Возвращает индекс искомого элемента, 
        /// если он существует в коллекции.
        /// В противном случае - индекс ближайшего
        /// МЕНЬШЕГО элемента.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr">SORTED array</param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static SoftBinarySearchResult SoftBinarySearch<T>(this T[] arr, T item) where T : IComparable<T>
        {
            if (arr == null)
                throw new ArgumentNullException();

            int index;

            var compareToFirst = item.CompareTo(arr[0]);
            if (arr.Length == 1 || compareToFirst <= 0)
            {
                index = 0;
                return new SoftBinarySearchResult(index, compareToFirst == 0
                    ? SoftBinarySearchResult.ResultType.Exact
                    : SoftBinarySearchResult.ResultType.Closest);
            }

            var compareToLast = item.CompareTo(arr[^1]);
            if (compareToLast >= 0)
            {
                index = arr.Length - 1;
                return new SoftBinarySearchResult(index, compareToLast == 0
                   ? SoftBinarySearchResult.ResultType.Exact
                   : SoftBinarySearchResult.ResultType.Closest);
            }

            var bsRes = Array.BinarySearch(arr, item);

            if (bsRes > 0)
            {
                index = bsRes;
                return new SoftBinarySearchResult(index, SoftBinarySearchResult.ResultType.Exact);
            }

            index = (bsRes * -1) - 2;
            return new SoftBinarySearchResult(index, SoftBinarySearchResult.ResultType.Lower);

        }

        public struct SoftBinarySearchResult
        {
            public int Index;
            public ResultType Type;

            public SoftBinarySearchResult(int index, ResultType type)
            {
                Index = index;
                Type = type;
            }

            public enum ResultType
            {
                /// <summary>
                /// array[Index] == value
                /// </summary>
                Exact,
                /// <summary>
                /// array[Index] < value < array[Index + 1]
                /// </summary>
                Lower,
                /// <summary>
                /// [Index == 0 || Index == array.Length-1
                /// </summary>
                Closest
            }
        }
    }
}
