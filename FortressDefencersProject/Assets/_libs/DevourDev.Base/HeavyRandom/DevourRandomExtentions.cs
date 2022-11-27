using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevourDev.Base.HeavyRandom
{
    public static class DevourRandomExtentions
    {
        public static T RandElement<T>(this ICollection<T> collection)
        {
            return new DevourRandom().RandElement(collection);
        }
        public static T RandElementExcept<T>(this ICollection<T> collection, params int[] exceptedIndexes)
        {
            return new DevourRandom().RandElementExcept(collection, exceptedIndexes);
        }
    }
}
