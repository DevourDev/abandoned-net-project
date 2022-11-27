using FD.Units.Stats;

namespace FD.Units
{
    public class RealBattleStats
    {
        public RealOffence Offensive { get; private set; }
        public RealDefence Defensive { get; private set; }


        public RealBattleStats()
        {
            Offensive = new();
            Defensive = new();
        }


        public class RealOffence
        {
            public float AttackDamage { get; set; }
            public DamageTypeObject AttackDamageType { get; set; }
            public float AttackSpeed { get; set; }
            public float AttackRange { get; set; }
            public bool IsRanged { get; set; }
            public float ProjectileSpeed { get; set; }
        }


        public class RealDefence
        {
            public float ArmorValue { get; set; }
            public ArmorTypeObject ArmorType { get; set; }
        }
    }

}
