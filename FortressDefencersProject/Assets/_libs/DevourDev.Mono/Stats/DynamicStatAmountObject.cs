using System;
using UnityEngine;

namespace DevourDev.Base
{
    [Serializable]
    public class DynamicStatAmountObject<DynamicStatFamily> : IDynamicStatAmount<DynamicStatFamily>
        where DynamicStatFamily : ScriptableObject
    {
        [SerializeField] private DynamicStatFamily _stat;
        [SerializeField] private float _amount;


        public DynamicStatFamily Stat { get => _stat; set => _stat = value; }
        public float Amount { get => _amount; set => _amount = value; }
    }

    public struct DynamicStatAmount<DynamicStatFamily> : IDynamicStatAmount<DynamicStatFamily>
        where DynamicStatFamily : ScriptableObject
    {
        public DynamicStatAmount(DynamicStatFamily stat, float amount)
        {
            Stat = stat;
            Amount = amount;
        }


        public DynamicStatFamily Stat { get; set; }
        public float Amount { get; set; }
    }

    public interface IDynamicStatAmount<DynamicStatFamily>
        where DynamicStatFamily : ScriptableObject
    {
        public DynamicStatFamily Stat { get; }
        public float Amount { get; set; }
    }

}
