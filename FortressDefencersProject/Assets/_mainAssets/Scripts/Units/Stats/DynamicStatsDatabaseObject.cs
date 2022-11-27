using UnityEngine;

namespace FD.Units.Stats
{
    [CreateAssetMenu(menuName = "FD/Units/Stats/Dynamics/New Database")]
    public class DynamicStatsDatabaseObject : DevourDev.MonoBase.GameDatabase<DynamicStatObject>
    {
        private void OnValidate()
        {
            ManageElementsOnValidate();
        }
    }
}
