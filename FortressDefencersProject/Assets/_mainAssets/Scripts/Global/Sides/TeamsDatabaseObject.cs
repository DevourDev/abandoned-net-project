using UnityEngine;

namespace FD.Global.Sides
{
    [CreateAssetMenu(menuName = "FD/Global/Sides/Team Objects Database")]
    public class TeamsDatabaseObject : DevourDev.MonoBase.GameDatabase<TeamObject>
    {
        private void OnValidate()
        {
            ManageElementsOnValidate();
        }
    }

}