using System;
using UnityEngine;

namespace DevourDev.Base
{
    [Serializable]
    public class DynamicStatCreator<DynamicStatFamily> where DynamicStatFamily : ScriptableObject
    {
        [SerializeField] private DynamicStatFamily _stat;
        [SerializeField] private float _max;
        [SerializeField] private float _min = 0f;
        [SerializeField] private float _regeneration;


        public bool CreateDynamicStat(DynamicStatsCollectionBase<DynamicStatFamily> collection)
        {
            var ds = new DynamicStat(_max, _regeneration, _min);
            return collection.Stats.TryAdd(_stat, ds);
        }
    }

}
