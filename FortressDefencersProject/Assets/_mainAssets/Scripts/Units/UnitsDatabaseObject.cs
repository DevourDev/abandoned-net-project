using DevourDev.MonoBase;
using UnityEngine;

namespace FD.Units
{
    [CreateAssetMenu(menuName = "FD/Units/Database")]
    public class UnitsDatabaseObject : GameDatabase<UnitObject>
    {
        private void OnValidate()
        {
            ManageElementsOnValidate();
        }
    }

}
