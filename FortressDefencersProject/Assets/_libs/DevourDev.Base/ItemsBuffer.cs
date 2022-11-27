using System.Collections.Generic;

namespace DevourDev.Base
{
    public class ItemsBuffer<T> 
    {
        private readonly List<T> _buffer;

        public ItemsBuffer(int capacity = 8)
        {
            _buffer = new(capacity);
        }


        public List<T> Buffer => _buffer;
    }
}
