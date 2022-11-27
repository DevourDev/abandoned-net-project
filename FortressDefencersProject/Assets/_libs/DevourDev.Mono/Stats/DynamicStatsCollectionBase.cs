using System;
using System.Collections.Generic;
using UnityEngine;

namespace DevourDev.Base
{
    [Serializable]
    public class DynamicStatsCollectionBase<DynamicStatsFamily> where DynamicStatsFamily : ScriptableObject
    {
        private readonly Dictionary<DynamicStatsFamily, DynamicStat> _stats;


        public DynamicStatsCollectionBase(int dictionarySize = 8)
        {
            _stats = new(dictionarySize);
        }


        public Dictionary<DynamicStatsFamily, DynamicStat> Stats => _stats;


        public bool TryGetDynamicStat(DynamicStatsFamily f, out DynamicStat s)
        {
            return _stats.TryGetValue(f, out s);
        }

        public bool CanSpend(IDynamicStatAmount<DynamicStatsFamily> dsa)
        {
            if (!TryGetDynamicStat(dsa.Stat, out var ds))
            {
                return false;
            }

            return ds.CanSpend(dsa.Amount);
        }
        public bool TrySpend(IDynamicStatAmount<DynamicStatsFamily> dsa)
        {
            if (TryGetDynamicStat(dsa.Stat, out var s))
            {
                return s.TrySpend(dsa.Amount);
            }

            return false;
        }
    }

}
