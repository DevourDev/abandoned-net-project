using FD.Units.Stats;

namespace FD.Units
{
    public class RealCommonStats
    {
        public DynamicStatsCollection DynamicStats { get; set; }
        public DynamicStatObject LethalDynamicStat { get; set; }
        public float VisionRange { get; set; }
        public float MoveSpeed { get; set; }
        public float RotationSpeed { get; set; }
        public float Acceleration { get; set; }
    }

}
