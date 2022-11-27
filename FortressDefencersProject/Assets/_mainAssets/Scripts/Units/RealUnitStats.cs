namespace FD.Units
{
    public class RealUnitStats
    {
        public RealBattleStats Battle { get; private set; }
        public RealCommonStats Common { get; private set; }


        public RealUnitStats()
        {
            Battle = new();
            Common = new();
        }

    }

}
