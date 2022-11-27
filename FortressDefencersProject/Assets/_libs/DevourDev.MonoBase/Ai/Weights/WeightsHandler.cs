using System.Linq;
using System.Collections.Generic;

namespace DevourDev.Ai.Weights
{
    public abstract class WeightsHandler<WItem> where WItem : IWeightItem
    {
        private Dictionary<WItem, float> _weightsItems;


        public WeightsHandler(WItem defaultItem)
        {
            _weightsItems = new();
            TryAddItem(defaultItem);
        }


        public WItem Winner
        {
            get
            {
                var winner = _weightsItems.First();
                foreach (var item in _weightsItems)
                {
                    if (item.Value > winner.Value)
                    {
                        winner = item;
                    }
                }

                return winner.Key;
            }
        }


        public bool TryAddItem(WItem item) => _weightsItems.TryAdd(item, 0);
        public bool AddWeights(WItem item, float weights)
        {
            if (_weightsItems.ContainsKey(item))
            {
                _weightsItems[item] += weights;
                return true;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
        public void ResetWeights()
        {
            foreach (var item in _weightsItems)
            {
                _weightsItems[item.Key] = 0;
            }
        }
        public void ResetItems()
        {
            _weightsItems.Clear();
        }
    }
}