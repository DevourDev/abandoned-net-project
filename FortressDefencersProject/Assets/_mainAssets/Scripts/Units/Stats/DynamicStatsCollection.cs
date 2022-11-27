using DevourDev.Base;

namespace FD.Units.Stats
{
    public class DynamicStatsCollection : DynamicStatsCollectionBase<DynamicStatObject>
    {
        public DynamicStatsCollection(int dictionarySize = 8) : base (dictionarySize)
        {

        }
    }
}
