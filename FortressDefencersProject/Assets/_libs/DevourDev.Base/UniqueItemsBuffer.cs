using DevourDev.Base;
using System.Collections.Generic;

namespace DevourDev.Base
{
    public class UniqueItemsBuffer<T> where T : IUnique
    {
        private readonly HashSet<T> _buffer;

        public UniqueItemsBuffer(int capacity = 8)
        {
            _buffer = new(capacity);
        }


        public HashSet<T> Buffer => _buffer;
    }
}
