using UnityEngine;

namespace FD.Units.Ai
{
    [CreateAssetMenu(menuName = "FD/Units/Ai/States Database")]
    public class UnitStatesDatabaseObject : DevourDev.MonoBase.GameDatabase<UnitState>
    {
        private void OnValidate()
        {
            ManageElementsOnValidate();
        }
    }

}
