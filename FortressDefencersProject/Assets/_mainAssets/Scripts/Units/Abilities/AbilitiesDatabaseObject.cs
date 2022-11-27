using DevourDev.MonoBase;
using UnityEngine;

namespace FD.Units.Abilities
{
    [CreateAssetMenu(menuName = "FD/Units/Abilities/Database")]
    public class AbilitiesDatabaseObject : GameDatabase<UnitAbilityObject>
    {
        private void OnValidate()
        {
            ManageElementsOnValidate();
        }
    }
}
